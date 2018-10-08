namespace Server.World.Dungeons
{
	public abstract class DungeonRegion : Region
	{
		public Dungeon Dungeon { get; private set; }

		protected DungeonRegion(int regionId, Dungeon dungeon)
			: base(regionId)
		{
		}
	}
}
