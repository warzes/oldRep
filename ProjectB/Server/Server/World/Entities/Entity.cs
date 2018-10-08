using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.World.Entities
{
	public abstract class Entity
	{
		public long EntityId { get; set; }
		public abstract int RegionId { get; set; }
		public Region Region
		{
			get { return _region ?? Region.Limbo; }
			set { _region = value ?? Region.Limbo; }
		}
		private Region _region;

		public abstract Position GetPosition();
		
		public Location GetLocation()
		{
			return new Location(this.RegionId, this.GetPosition());
		}
	}
}
