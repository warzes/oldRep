using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Shared.Database;
using Core.Const;
using Shared.Util;

namespace Login.Database
{
	public class Account
	{
		public string Name { get; set; }
		public string Password { get; set; }
		public long SessionKey { get; set; }

		public byte Authority { get; set; }

		public DateTime Creation { get; set; }
		public DateTime LastLogin { get; set; }
		public string LastIp { get; set; }

		public string BannedReason { get; set; }
		public DateTime BannedExpiration { get; set; }

		public List<Character> Characters { get; set; }

		public Account()
		{
			this.Creation = DateTime.Now;
			this.LastLogin = DateTime.Now;

			this.Characters = new List<Character>();
		}
		
		/// <summary>
		/// Returns character with id, or null if it doesn't exist.
		/// </summary>
		public Character GetCharacter(long id)
		{
			return this.Characters.FirstOrDefault(a => a.EntityId == id);
		}

		public bool CreateCharacter(Character character, List<Item> items)
		{
			if (!LoginServer.Instance.Database.CreateCharacter(this.Name, character, items))
				return false;

			this.Characters.Add(character);

			return true;
		}
	}
}