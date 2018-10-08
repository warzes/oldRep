using System;
using System.Collections.Generic;
using Login.Database;
using Shared.Database;
using Core;
using Core.Const;
using Shared.Network;
using Shared.Network.Sending.Helpers;
using Core.Network;

namespace Login.Network
{
	public static partial class Send
	{
		/// <summary>
		/// Sends ClientIdentR to client.
		/// </summary>
		public static void CheckIdentR(LoginClient client, bool success)
		{
			var packet = new Packet(Op.ClientIdentR, CoreId.Login);
			packet.PutByte(success);
			packet.PutLong(DateTime.Now);

			client.Send(packet);
		}

		/// <summary>
		/// Sends message as response to login (LoginR).
		/// </summary>
		public static void LoginR_Msg(LoginClient client, string format, params object[] args)
		{
			var packet = new Packet(Op.LoginR, CoreId.Login);
			packet.PutByte((byte)LoginResult.Message);
			packet.PutInt((int)LoginResultMessage.Custom);
			packet.PutString(format, args);

			client.Send(packet);
		}

		/// <summary>
		/// Sends (negative) LoginR to client.
		/// </summary>
		public static void LoginR_Fail(LoginClient client, LoginResult result)
		{
			var packet = new Packet(Op.LoginR, CoreId.Login);
			packet.PutByte((byte)result);

			client.Send(packet);
		}

		/// <summary>
		/// Sends positive response to login.
		/// </summary>
		public static void LoginR(LoginClient client, Account account, long sessionKey, ICollection<ServerInfo> serverList)
		{
			var packet = new Packet(Op.LoginR, CoreId.Login);
			packet.PutByte((byte)LoginResult.Success);
			packet.PutString(account.Name);
			packet.PutLong(sessionKey);

			// Servers
			// --------------------------------------------------------------
			packet.AddServerList(serverList, ServerInfoType.Client);

			// Account Info
			// --------------------------------------------------------------
			packet.Add(account);

			client.Send(packet);
		}

		/// <summary>
		/// Sends negative ChannelInfoRequestR to client.
		/// </summary>
		public static void ChannelInfoRequestR_Fail(LoginClient client)
		{
			ChannelInfoRequestR(client, null, 0);
		}

		/// <summary>
		/// Sends ChannelInfoRequestR to client.
		/// </summary>
		public static void ChannelInfoRequestR(LoginClient client, ChannelInfo info, long characterId)
		{
			var packet = new Packet(Op.ChannelInfoRequestR, CoreId.Channel);
			packet.PutByte(info != null);

			if (info != null)
			{
				packet.PutString(info.ServerName);
				packet.PutString(info.Name);
				packet.PutString(info.Host);
				packet.PutShort((short)info.Port);
				packet.PutLong(characterId);
			}

			client.Send(packet);
		}

		/// <summary>
		/// Sends server/channel status update to all connected players.
		/// </summary>
		public static void ChannelStatus(ICollection<ServerInfo> serverList)
		{
			var packet = new Packet(Op.ChannelStatus, CoreId.Login);
			packet.AddServerList(serverList, ServerInfoType.Client);

			LoginServer.Instance.BroadcastPlayers(packet);
		}

		/// <summary>
		/// Sends shutdown request to all channels.
		/// </summary>
		/// <param name="time">Seconds until shutdown.</param>
		public static void ChannelShutdown(int time)
		{
			var packet = new Packet(Op.Internal.ChannelShutdown, CoreId.Channel);
			packet.PutInt(time);
			LoginServer.Instance.BroadcastChannels(packet);
		}

		/// <summary>
		/// Sends server/channel status update to all connected channels.
		/// </summary>
		public static void Internal_ChannelStatus(ICollection<ServerInfo> serverList)
		{
			var packet = new Packet(Op.Internal.ChannelStatus, CoreId.Login);
			packet.AddServerList(serverList, ServerInfoType.Internal);

			LoginServer.Instance.BroadcastChannels(packet);
		}

		/// <summary>
		/// Sends account disconnect request to all channels.
		/// </summary>
		public static void Internal_RequestDisconnect(string accountName)
		{
			var packet = new Packet(Op.Internal.RequestDisconnect, CoreId.Login);
			packet.PutString(accountName);

			LoginServer.Instance.BroadcastChannels(packet);
		}

		/// <summary>
		/// Sends Internal.ServerIdentifyR  to channel client.
		/// </summary>
		public static void Internal_ServerIdentifyR(LoginClient client, bool success)
		{
			var packet = new Packet(Op.Internal.ServerIdentifyR, 0);
			packet.PutByte(success);

			client.Send(packet);
		}
				
		private static void Add(this Packet packet, Account account)
		{
			packet.PutLong(DateTime.Now);   // Last Login
			packet.PutLong(DateTime.Now);   // Last Logout

			// Characters
			// --------------------------------------------------------------
			packet.PutShort((short)account.Characters.Count);
			foreach (var character in account.Characters)
			{
				packet.PutString(character.Server);
				packet.PutLong(character.EntityId);
				packet.PutString(character.Name);
				packet.PutByte((byte)character.DeletionFlag);
			}
		}
	}
}