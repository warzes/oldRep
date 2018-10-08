using System;
using System.Text;
using Core.Network;
using Core.Const;
using Server.Database.CoreData;
using Server.World.Entities;
using Server.Util;

namespace Server.Network.Sending
{
	public static partial class Send
	{
		public static void SystemMessage(Creature creature, string format, params object[] args)
		{
			SystemMessageFrom(creature, Localization.Get("<SYSTEM>"), format, args);
		}

		public static void ServerMessage(Creature creature, string format, params object[] args)
		{
			SystemMessageFrom(creature, Localization.Get("<SERVER>"), format, args);
		}

		public static void SystemMessageFrom(Creature creature, string from, string format, params object[] args)
		{
			var packet = new Packet(Op.Chat, creature.EntityId);

			foreach (var msg in string.Format(format, args).Chunkify(100))
			{
				packet.PutByte(0);
				packet.PutString(from);
				packet.PutString(msg);
				packet.PutByte(true);
				packet.PutUInt(0xFFFF8080);
				packet.PutInt(0);
				packet.PutByte(0);

				creature.Client.Send(packet);

				packet.Clear(packet.Op, packet.Id);
			}
		}
	}
}
