using Shared.Database;
using Core.Const;
using Shared.Util;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace Login.Database
{
	public enum CharacterType { Character }

	public class LoginDb : SharedDb
	{
		/// <summary>
		/// Checks whether the SQL update file has already been applied.
		/// </summary>
		public bool CheckUpdate(string updateFile)
		{
			using (var conn = this.Connection)
			using (var mc = new MySqlCommand("SELECT * FROM `updates` WHERE `path` = @path", conn))
			{
				mc.Parameters.AddWithValue("@path", updateFile);

				using (var reader = mc.ExecuteReader())
					return reader.Read();
			}
		}

		/// <summary>
		/// Executes SQL update file.
		/// </summary>
		public void RunUpdate(string updateFile)
		{
			try
			{
				using (var conn = this.Connection)
				{
					// Run update
					using (var cmd = new MySqlCommand(File.ReadAllText(Path.Combine("sql", updateFile)), conn))
						cmd.ExecuteNonQuery();

					// Log update
					using (var cmd = new InsertCommand("INSERT INTO `updates` {0}", conn))
					{
						cmd.Set("path", updateFile);
						cmd.Execute();
					}

					Log.Info("Successfully applied '{0}'.", updateFile);
				}
			}
			catch (Exception ex)
			{
				Log.Error("RunUpdate: Failed to run '{0}': {1}", updateFile, ex.Message);
				CliUtil.Exit(1);
			}
		}

		/// <summary>
		/// Returns account or null if account doesn't exist.
		/// </summary>
		public Account GetAccount(string accountId)
		{
			using (var conn = this.Connection)
			using (var mc = new MySqlCommand("SELECT * FROM `accounts` WHERE `accountId` = @accountId", conn))
			{
				mc.Parameters.AddWithValue("@accountId", accountId);

				using (var reader = mc.ExecuteReader())
				{
					if (!reader.Read())
						return null;

					var account = new Account();
					account.Name = reader.GetStringSafe("accountId");
					account.Password = reader.GetStringSafe("password");
					account.SessionKey = reader.GetInt64("sessionKey");
					account.BannedExpiration = reader.GetDateTimeSafe("banExpiration");
					account.BannedReason = reader.GetStringSafe("banReason");

					return account;
				}
			}
		}
		
		/// <summary>
		/// Updates lastLogin and loggedIn from the account.
		/// </summary>
		public void UpdateAccount(Account account)
		{
			using (var conn = this.Connection)
			using (var cmd = new UpdateCommand("UPDATE `accounts` SET `lastLogin` = @lastLogin WHERE `accountId` = @accountId", conn))
			{
				cmd.Set("accountId", account.Name);
				cmd.Set("lastLogin", account.LastLogin);

				cmd.Execute();
			}
		}

		/// <summary>
		/// Returns a list of all characters on this account.
		/// </summary>
		public List<Character> GetCharacters(string accountId)
		{
			using (var conn = this.Connection)
			{
				var result = new List<Character>();
				this.GetCharacters(accountId, "characters", CharacterType.Character, ref result, conn);

				return result;
			}
		}

		/// <summary>
		/// Queries characters/pets/partners and adds them to result.
		/// </summary>
		private void GetCharacters(string accountId, string table, CharacterType type, ref List<Character> result, MySqlConnection conn)
		{
			using (var mc = new MySqlCommand(
				"SELECT * " +
				"FROM `" + table + "` AS c " +
				"INNER JOIN `creatures` AS cr ON c.creatureId = cr.creatureId " +
				"WHERE `accountId` = @accountId "
			, conn))
			{
				mc.Parameters.AddWithValue("@accountId", accountId);

				using (var reader = mc.ExecuteReader())
				{
					while (reader.Read())
					{
						var character = new Character();
						character.EntityId = reader.GetInt64("entityId");
						character.CreatureId = reader.GetInt64("creatureId");
						character.Name = reader.GetStringSafe("name");
						character.Server = reader.GetStringSafe("server");
						character.DeletionTime = reader.GetDateTimeSafe("deletionTime");

						result.Add(character);
					}
				}
			}
		}
		
		/// <summary>
		/// Creates creature:character combination for the account.
		/// Returns false if either failed, true on success. character's ids are set to the new ids.
		/// </summary>
		public bool CreateCharacter(string accountId, Character character, List<Item> items)
		{
			return this.Create("characters", accountId, character, items);
		}

		/// <summary>
		/// Creates creature:x combination for the account.
		/// Returns false if either failed, true on success. character's ids are set to the new ids.
		/// </summary>
		private bool Create(string table, string accountId, Character character, List<Item> items)
		{
			using (var conn = this.Connection)
			using (var transaction = conn.BeginTransaction())
			{
				try
				{
					// Creature
					character.CreatureId = this.CreateCreature(character, conn, transaction);

					// Character
					using (var cmd = new InsertCommand("INSERT INTO `" + table + "` {0}", conn, transaction))
					{
						cmd.Set("accountId", accountId);
						cmd.Set("creatureId", character.CreatureId);

						cmd.Execute();

						character.EntityId = cmd.LastId;
					}

					// Items
					if (items != null)
						this.AddItems(character.CreatureId, items, conn, transaction);

					transaction.Commit();

					return true;
				}
				catch (Exception ex)
				{
					character.EntityId = character.CreatureId = 0;

					Log.Exception(ex);

					return false;
				}
			}
		}

		/// <summary>
		/// Creatures creature based on character and returns its id.
		/// </summary>
		private long CreateCreature(Character creature, MySqlConnection conn, MySqlTransaction transaction)
		{
			using (var cmd = new InsertCommand("INSERT INTO `creatures` {0}", conn, transaction))
			{
				cmd.Set("server", creature.Server);
				cmd.Set("name", creature.Name);				
				cmd.Set("lifeMax", creature.Life);
				cmd.Set("manaMax", creature.Mana);
				cmd.Set("staminaMax", creature.Stamina);
				cmd.Set("str", creature.Str);
				cmd.Set("int", creature.Int);
				cmd.Set("dex", creature.Dex);
				cmd.Set("will", creature.Will);
				cmd.Set("luck", creature.Luck);
				cmd.Set("defense", creature.Defense);
				cmd.Set("protection", creature.Protection);
				cmd.Set("ap", creature.AP);
				cmd.Set("creationTime", DateTime.Now);
				cmd.Set("lastAging", DateTime.Now);
				cmd.Set("lastRebirth", DateTime.Now);

				cmd.Execute();

				return cmd.LastId;
			}
		}

		/// <summary>
		/// Adds items for creature.
		/// </summary>
		private void AddItems(long creatureId, List<Item> items, MySqlConnection conn, MySqlTransaction transaction)
		{
			foreach (var item in items)
			{
				
			}
		}
		
		/// <summary>
		/// Updates deletion time for character, or deletes it.
		/// </summary>
		public void UpdateDeletionTime(Character character)
		{
			using (var conn = this.Connection)
			{
				if (character.DeletionFlag == DeletionFlag.Delete)
				{
					using (var mc = new MySqlCommand("DELETE FROM `creatures` WHERE `creatureId` = @creatureId", conn))
					{
						mc.Parameters.AddWithValue("@creatureId", character.CreatureId);
						mc.ExecuteNonQuery();
					}
				}
				else
				{
					using (var cmd = new UpdateCommand("UPDATE `creatures` SET {0} WHERE `creatureId` = @creatureId", conn))
					{
						cmd.AddParameter("@creatureId", character.CreatureId);
						cmd.Set("deletionTime", character.DeletionTime);

						cmd.Execute();
					}
				}
			}
		}
	}
}