using Server.World.Entities;

namespace Server.Database.CoreData
{
	public class Character : PlayerCreature
	{
		public long CreatureId { get; set; }
		public string Name { get; set; }
	}
}
