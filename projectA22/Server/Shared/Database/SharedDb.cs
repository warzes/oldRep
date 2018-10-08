using Core.Const;
using Shared.Util;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Shared.Database
{
	public class SharedDb
	{
		private string _connectionString;

		private Regex _nameCheckRegex = new Regex(@"^[a-zA-Z][a-z0-9]{2,15}$", RegexOptions.Compiled);

		/// <summary>
		/// Returns a valid connection.
		/// </summary>
		public MySqlConnection Connection
		{
			get
			{
				if (_connectionString == null)
					throw new Exception("SharedDb has not been initialized.");

				var result = new MySqlConnection(_connectionString);
				result.Open();
				return result;
			}
		}

		/// <summary>
		/// Sets connection string and calls TestConnection.
		/// </summary>
		public void Init(string host, int port, string user, string pass, string db)
		{
			_connectionString = string.Format("server={0}; port={1}; database={2}; uid={3}; password={4}; pooling=true; min pool size=0; max pool size=100; ConvertZeroDateTime=true", host, port, db, user, pass);
			this.TestConnection();
		}

		/// <summary>
		/// Tests connection, throws on error.
		/// </summary>
		public void TestConnection()
		{
			MySqlConnection conn = null;
			try
			{
				conn = this.Connection;
			}
			finally
			{
				if (conn != null)
					conn.Close();
			}
		}

		// ------------------------------------------------------------------

		/// <summary>
		/// Returns whether the account exists.
		/// </summary>
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

		/// <summary>
		/// Returns whether the account is marked as being logged in.
		/// </summary>
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

		/// <summary>
		/// Set's account's loggedIn field.
		/// </summary>
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

		/// <summary>
		/// Adds new account to the database.
		/// </summary>
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
		
		/// <summary>
		/// Resets password for account to its name.
		/// </summary>
		public void SetAccountPassword(string accountName, string password)
		{
			using (var conn = this.Connection)
			using (var mc = new MySqlCommand("UPDATE `accounts` SET `password` = @password WHERE `accountId` = @accountId", conn))
			{
				mc.Parameters.AddWithValue("@accountId", accountName);
				mc.Parameters.AddWithValue("@password", Password.HashRaw(password));

				mc.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Sets new randomized session key for the account and returns it.
		/// </summary>
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

		/// <summary>
		/// Changes auth level of account.
		/// </summary>
		public bool ChangeAuth(string accountId, int level)
		{
			using (var conn = this.Connection)
			using (var cmd = new UpdateCommand("UPDATE `accounts` SET {0} WHERE `accountId` = @accountId", conn))
			{
				cmd.AddParameter("@accountId", accountId);
				cmd.Set("authority", level);

				return (cmd.Execute() > 0);
			}
		}

		/// <summary>
		/// Unsets creature's Initialized creature state flag.
		/// </summary>
		public void UninitializeCreature(long creatureId)
		{
			using (var conn = this.Connection)
			using (var mc = new MySqlCommand("UPDATE `creatures` SET `state` = `state` & ~1 WHERE `creatureId` = @creatureId", conn))
			{
				mc.Parameters.AddWithValue("@creatureId", creatureId);
				mc.ExecuteNonQuery();
			}
		}
	}

	/// <summary>
	/// Extensions for the MySqlDataReader.
	/// </summary>
	public static class MySqlDataReaderExtension
	{
		/// <summary>
		/// Returns true if value at index is null.
		/// </summary>
		private static bool IsDBNull(this MySqlDataReader reader, string index)
		{
			return reader.IsDBNull(reader.GetOrdinal(index));
		}

		/// <summary>
		/// Same as GetString, except for a is null check. Returns null if NULL.
		/// </summary>
		public static string GetStringSafe(this MySqlDataReader reader, string index)
		{
			if (IsDBNull(reader, index))
				return null;
			else
				return reader.GetString(index);
		}

		/// <summary>
		/// Returns DateTime of the index, or DateTime.MinValue, if value is null.
		/// </summary>
		public static DateTime GetDateTimeSafe(this MySqlDataReader reader, string index)
		{
			return reader[index] as DateTime? ?? DateTime.MinValue;
		}
	}
}