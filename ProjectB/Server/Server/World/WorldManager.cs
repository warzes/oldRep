using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Server.Util;

namespace Server.World
{
	public class WorldManager
	{
		private Dictionary<int, Region> _regions;
		
		public int Count { get { return _regions.Count; } }


		public const int HeartbeatTime = 500;
		public const int Second = 1000, Minute = Second * 60, Hour = Minute * 60;

		private Timer _heartbeatTimer;

		public WorldManager()
		{
			_regions = new Dictionary<int, Region>();
		}

		public void Initialize()
		{
			this.AddRegionsFromData();
			this.SetUpHeartbeat();
		}

		private void AddRegionsFromData()
		{
		}

		private void SetUpHeartbeat()
		{
			var now = DateTime.Now;

			// Start timer on the next HeartbeatTime (eg on the next full 500 ms) and run it regularly afterwards.
			_heartbeatTimer = new Timer(Heartbeat, null, HeartbeatTime - (now.Ticks / 10000 % HeartbeatTime), HeartbeatTime);
		}


		private void Heartbeat(object _)
		{
			try
			{
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "Exception during world hearbeat.");
			}
		}

		private void UpdateEntities()
		{
		}

		public void AddRegion(int regionId)
		{
			lock (_regions)
			{
				if (_regions.ContainsKey(regionId))
				{
					Log.Warning("Region '{0}' already exists.", regionId);
					return;
				}
			}

			var region = new NormalRegion(regionId);
			lock (_regions)
				_regions.Add(regionId, region);
		}

		public void AddRegion(Region region)
		{
			lock (_regions)
			{
				if (_regions.ContainsKey(region.Id))
				{
					Log.Warning("Region '{0}' already exists.", region.Id);
					return;
				}

				_regions.Add(region.Id, region);
			}
		}

		public void RemoveRegion(int regionId)
		{
			lock (_regions)
				_regions.Remove(regionId);
		}

		public Region GetRegion(int regionId)
		{
			Region result;
			lock (_regions)
				_regions.TryGetValue(regionId, out result);
			return result;
		}

		public Region GetRegion(string regionName)
		{
			lock (_regions)
				return _regions.Values.FirstOrDefault(a => a.Name == regionName);
		}

		public bool HasRegion(int regionId)
		{
			lock (_regions)
				return _regions.ContainsKey(regionId);
		}
	}
}
