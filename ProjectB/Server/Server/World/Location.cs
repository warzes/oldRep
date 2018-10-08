using System;

namespace Server.World
{
	public struct Location
	{
		public int RegionId;
		public int X, Y;

		public Position Position { get { return new Position(X, Y); } }

		public Location(int regionId, int x, int y)
		{
			this.RegionId = regionId;
			this.X = x;
			this.Y = y;
		}

		public Location(int regionId, Position pos)
			: this(regionId, pos.X, pos.Y)
		{
		}
		
		public static bool operator ==(Location loc1, Location loc2)
		{
			return (loc1.RegionId == loc2.RegionId && loc1.X == loc2.X && loc1.Y == loc2.Y);
		}

		public static bool operator !=(Location loc1, Location loc2)
		{
			return !(loc1 == loc2);
		}

		public override int GetHashCode()
		{
			return this.RegionId.GetHashCode() ^ this.X.GetHashCode() ^ this.Y.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is Location && this == (Location)obj;
		}

		public override string ToString()
		{
			return string.Format("(RegionId: {0}, X: {1}, Y: {2})", this.RegionId, this.X, this.Y);
		}
	}
}