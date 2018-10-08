using System;
using Core.Network;
using Core.Const;
using Server.Database.CoreData;

namespace Server.Network.Sending
{
	public static partial class Send
	{
		public static void ChannelLoginR(Client client, long creatureId)
		{
			var packet = new Packet(Op.ChannelLoginR, CoreId.Channel);
			packet.PutByte(true);
			packet.PutLong(creatureId);
			packet.PutLong(DateTime.Now);
			packet.PutInt((int)DateTime.Now.DayOfWeek);
			client.Send(packet);
		}
	}
}
