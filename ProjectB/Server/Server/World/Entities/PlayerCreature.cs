using System;
using System.Collections.Generic;
using Server.Util;
using Server.Network.Sending;
using Core.Const;
using Server.World.Dungeons;
using Server.Util;

namespace Server.World.Entities
{
	public abstract class PlayerCreature : Creature
	{
		private List<Entity> _visibleEntities = new List<Entity>();
		private object _lookAroundLock = new Object();
		public long CreatureId { get; set; }

		public PlayerCreature()
		{
		}

		public override bool Warp(int regionId, int x, int y)
		{
			var targetRegion = ServerApp.Instance.World.GetRegion(regionId);
			if (targetRegion == null)
			{
				Send.ServerMessage(this, "Warp failed, region doesn't exist.");
				Log.Error("PC.Warp: Region '{0}' doesn't exist.", regionId);
				return false;
			}

			var currentRegionId = this.RegionId;
			var loc = new Location(currentRegionId, this.GetPosition());

			this.LastLocation = loc;
			this.WarpLocation = new Location(regionId, x, y);
			this.Warping = true;
			this.Lock(Locks.Default, true);

			// Dynamic Region warp
			var dynamicRegion = targetRegion as DynamicRegion;
			if (dynamicRegion != null)
			{
				if (!this.Region.IsTemp)
					this.FallbackLocation = loc;

				Send.EnterDynamicRegion(this, currentRegionId, targetRegion, x, y);

				return true;
			}

			// Dungeon warp
			var dungeonRegion = targetRegion as DungeonRegion;
			if (dungeonRegion != null)
			{
				if (!this.Region.IsTemp)
				{
					this.FallbackLocation = loc;
					this.DungeonSaveLocation = this.WarpLocation;
				}

				Send.DungeonInfo(this, dungeonRegion.Dungeon);

				return true;
			}

			// Normal warp
			Send.EnterRegion(this, regionId, x, y);

			return true;
		}
	}
}
