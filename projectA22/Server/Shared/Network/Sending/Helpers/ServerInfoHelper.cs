using Core.Network;
using System;
using System.Collections.Generic;

namespace Shared.Network.Sending.Helpers
{
	public enum ServerInfoType
	{
		Client, Internal
	}

	public static class ServerInfoHelper
	{
		/// <summary>
		/// Adds list of server and channel information to packet.
		/// </summary>
		public static void AddServerList(this Packet packet, ICollection<ServerInfo> serverList, ServerInfoType type)
		{
			packet.PutByte((byte)serverList.Count);
			foreach (var server in serverList)
				packet.AddServerInfo(server, type);
		}

		/// <summary>
		/// Adds server and channel information to packet.
		/// </summary>
		public static void AddServerInfo(this Packet packet, ServerInfo server, ServerInfoType type)
		{
			packet.PutString(server.Name);

			// Channels
			packet.PutInt((int)server.Channels.Count);
			foreach (var channel in server.Channels.Values)
				packet.AddChannelInfo(channel, type);
		}

		/// <summary>
		/// Adds channel information to packet.
		/// </summary>
		public static void AddChannelInfo(this Packet packet, ChannelInfo channel, ServerInfoType type)
		{
			packet.PutString(channel.Name);
			packet.PutInt((int)channel.State);
			packet.PutShort(channel.Stress);

			// Channels need more information
			if (type == ServerInfoType.Internal)
			{
				packet.PutString(channel.Host);
				packet.PutInt(channel.Port);
				packet.PutInt(channel.Users);
				packet.PutInt(channel.MaxUsers);
			}
		}
	}
}