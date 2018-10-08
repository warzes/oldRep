using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Server.Util;
using Server.Database.CoreData;
using Server.World.Entities;

namespace Server.Database
{
	public partial class ServerDb
	{
		public void SetAccountLoggedIn(string accountId, bool loggedIn)
		{
			using (var conn = this.Connection)
			using (var mc = new MySqlCommand("UPDATE `accounts` SET `loggedIn` = @loggedIn WHERE `accountId` = @accountId", conn))
			{
				mc.Parameters.AddWithValue("@accountId", accountId);
				mc.Parameters.AddWithValue("@loggedIn", loggedIn);

				mc.ExecuteNonQuery();
			}
		}

		public bool AccountIsLoggedIn(string accountId)
		{
			using (var conn = this.Connection)
			{
				var mc = new MySqlCommand("SELECT `accountId` FROM `accounts` WHERE `accountId` = @accountId AND `loggedIn` = true", conn);
				mc.Parameters.AddWithValue("@accountId", accountId);

				using (var reader = mc.ExecuteReader())
					return reader.HasRows;
			}
		}

		public bool AccountExists(string accountId)
		{
			using (var conn = this.Connection)
			{
				var mc = new MySqlCommand("SELECT `accountId` FROM `accounts` WHERE `accountId` = @accountId", conn);
				mc.Parameters.AddWithValue("@accountId", accountId);

				using (var reader = mc.ExecuteReader())
					return reader.HasRows;
			}
		}

		public void CreateAccount(string accountId, string password)
		{
			password = Password.Hash(password);

			using (var conn = this.Connection)
			using (var cmd = new InsertCommand("INSERT INTO `accounts` {0}", conn))
			{
				cmd.Set("accountId", accountId);
				cmd.Set("password", password);
				cmd.Set("creation", DateTime.Now);

				cmd.Execute();
			}
		}

		public long CreateSession(string accountId)
		{
			using (var conn = this.Connection)
			using (var mc = new MySqlCommand("UPDATE `accounts` SET `sessionKey` = @sessionKey WHERE `accountId` = @accountId", conn))
			{
				var sessionKey = RandomProvider.Get().NextInt64();

				mc.Parameters.AddWithValue("@accountId", accountId);
				mc.Parameters.AddWithValue("@sessionKey", sessionKey);

				mc.ExecuteNonQuery();

				return sessionKey;
			}
		}

		// TODO: это логин
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

					return account;
				}
			}
		}

		public Account GetAccountWorld(string accountId)
		{
			var account = new Account();

			using (var conn = this.Connection)
			{
				// Account
				// ----------------------------------------------------------
				using (var mc = new MySqlCommand("SELECT * FROM `accounts` WHERE `accountId` = @accountId", conn))
				{
					mc.Parameters.AddWithValue("@accountId", accountId);

					using (var reader = mc.ExecuteReader())
					{
						if (!reader.HasRows)
							return null;

						reader.Read();

						account.Id = reader.GetStringSafe("accountId");
						account.SessionKey = reader.GetInt64("sessionKey");
					}
				}
				
				// Characters
				// ----------------------------------------------------------
				var creatureId = 0L;
				try
				{
					using (var mc = new MySqlCommand("SELECT * FROM `characters` WHERE `accountId` = @accountId", conn))
					{
						mc.Parameters.AddWithValue("@accountId", accountId);

						using (var reader = mc.ExecuteReader())
						{
							while (reader.Read())
							{
								creatureId = reader.GetInt64("entityId");
								var character = this.GetCharacter<Character>(account, creatureId, "characters");
								if (character == null)
									continue;

								account.Characters.Add(character);
							}
						}
					}

				}
				catch (Exception ex)
				{
					Log.Exception(ex, "Problem while loading character '{0}'.", creatureId);
				}
			}

			return account;
		}

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

		public List<Character> GetCharacters(string accountId)
		{
			using (var conn = this.Connection)
			{
				var result = new List<Character>();
				this.GetCharacters(accountId, "characters", ref result, conn);

				return result;
			}
		}

		private void GetCharacters(string accountId, string table, ref List<Character> result, MySqlConnection conn)
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
						
						result.Add(character);
					}
				}
			}
		}

		private TCreature GetCharacter<TCreature>(Account account, long entityId, string table) where TCreature : PlayerCreature, new()
		{
			var character = new TCreature();

			using (var conn = this.Connection)
			using (var mc = new MySqlCommand("SELECT * FROM `" + table + "` AS c INNER JOIN `creatures` AS cr ON c.creatureId = cr.creatureId WHERE `entityId` = @entityId", conn))
			{
				mc.Parameters.AddWithValue("@entityId", entityId);

				using (var reader = mc.ExecuteReader())
				{
					if (!reader.Read())
						return null;

					character.EntityId = reader.GetInt64("entityId");
					character.CreatureId = reader.GetInt64("creatureId");
					character.Name = reader.GetStringSafe("name");
					var r = reader.GetInt32("region");
					var x = reader.GetInt32("x");
					var y = reader.GetInt32("y");
					character.SetLocation(r, x, y);
					character.Direction = reader.GetByte("direction");
				}

				character.LoadDefault();
			}
			
			return character;
		}

		public void UpdateOnlineStatus(long creatureId, bool isOnline)
		{
			using (var conn = this.Connection)
			using (var cmd = new UpdateCommand("UPDATE `creatures` SET {0} WHERE `creatureId` = @creatureId", conn))
			{
				cmd.AddParameter("@creatureId", creatureId);
				cmd.Set("online", isOnline);

				cmd.Execute();
			}
		}
	}
}
