using System.Collections.Generic;
using Server.Network;
using Server.World.Entities;
using Server.Util;
using Core.Const;

namespace Server.World
{
	public enum RegionMode
	{
		Permanent,
		RemoveWhenEmpty,
	}

	public abstract class Region
	{
		public const int VisibleRange = 3000;

		public static readonly Region Limbo = new Limbo();

		protected Dictionary<long, Creature> _creatures;

		protected HashSet<Client> _clients;

		public string Name { get; protected set; }
		public int Id { get; protected set; }

		public bool IsDynamic { get { return Math2.Between(this.Id, CoreId.DynamicRegions, CoreId.DynamicRegions + 5000); } }
		public bool IsDungeon { get { return Math2.Between(this.Id, CoreId.DungeonRegions, CoreId.DungeonRegions + 10000); } }
		public bool IsTemp { get { return (this.IsDynamic || this.IsDungeon); } }

		protected Region(int regionId)
		{

		}
	}

	public class Limbo : Region
	{
		public Limbo() : base(0) {}
	}
}