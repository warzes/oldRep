namespace Server.World
{
	public class DynamicRegion : Region
	{
		public DynamicRegion(int baseRegionId, string variationFile = "", RegionMode mode = RegionMode.RemoveWhenEmpty)
			: base(baseRegionId)
		{
		}
	}
}
