using System;
using Core.Network;
using Core.Const;
using Server.Database.CoreData;

namespace Server.Network.Sending
{
	public static partial class Send
	{
		public static void CheckIdentR(Client client, bool success)
		{
			var packet = new Packet(Op.ClientIdentR, CoreId.Login);
			packet.PutByte(success);
			packet.PutLong(DateTime.Now);

			client.Send(packet);
		}

		public static void LoginR_Fail(Client client, LoginResult result)
		{
			var packet = new Packet(Op.LoginR, CoreId.Login);
			packet.PutByte((byte)result);			
			client.Send(packet);
		}

		public static void LoginR(Client client, Account account, long sessionKey)
		{
			var packet = new Packet(Op.LoginR, CoreId.Login);
			packet.PutByte((byte)LoginResult.Success);
			packet.PutString(account.Name);	
			packet.PutLong(sessionKey);
			
			// Account Info
			// --------------------------------------------------------------
			packet.Add(account);

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
				packet.PutLong(character.EntityId);
				packet.PutString(character.Name);
			}
		}
	}
}