using System;
using Core.Network;
using Core.Const;
using Server.Database.CoreData;
using Server.World.Entities;
using Server.World;

namespace Server.Network.Sending
{
	public static partial class Send
	{
		public static void CharacterLock(Creature creature, Locks type)
		{
			var packet = new Packet(Op.CharacterLock, creature.EntityId);
			packet.PutUInt((uint)type);
			creature.Client.Send(packet);
		}

		public static void CharacterUnlock(Creature creature, Locks type)
		{
			var packet = new Packet(Op.CharacterUnlock, creature.EntityId);
			packet.PutUInt((uint)type);
			creature.Client.Send(packet);
		}

		public static void EnterRegion(Creature creature, int regionId, int x, int y)
		{
			var pos = creature.GetPosition();

			var packet = new Packet(Op.EnterRegion, CoreId.Channel);
			packet.PutLong(creature.EntityId);
			packet.PutInt(regionId);
			packet.PutInt(x);
			packet.PutInt(y);

			creature.Client.Send(packet);
		}

		public static void EnterDynamicRegion(Creature creature, int warpFromRegionId, Region warpToRegion, int x, int y)
		{
			// TODO:
		}
	}
}