using System;
using System.Collections.Generic;
using System.Linq;
using Server.World.Entities;
using Server.Util;

namespace Server.Database.CoreData
{
	public class Account
	{
		public string Id { get; set; }

		public string Name { get; set; }
		public string Password { get; set; }
		public long SessionKey { get; set; }
		public DateTime Creation { get; set; }
		public DateTime LastLogin { get; set; }

		public List<Character> Characters { get; set; }

		public Account()
		{
			this.Creation = DateTime.Now;
			this.LastLogin = DateTime.Now;

			this.Characters = new List<Character>();
		}

		public PlayerCreature GetCharacter(long entityId)
		{
			PlayerCreature result = this.Characters.FirstOrDefault(a => a.EntityId == entityId);
			return result;
		}

		public PlayerCreature GetCharacterSafe(long entityId)
		{
			var r = this.GetCharacter(entityId);
			if (r == null)
				throw new SevereViolation("Account doesn't contain character 0x{0:X}", entityId);

			return r;
		}
	}
}
