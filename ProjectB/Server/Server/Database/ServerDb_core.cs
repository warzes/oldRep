using System;
using System.IO;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Server.Util;

namespace Server.Database
{
	public partial class ServerDb
	{
		private string _connectionString;

		private Regex _nameCheckRegex = new Regex(@"^[a-zA-Z][a-z0-9]{2,15}$", RegexOptions.Compiled);

		public MySqlConnection Connection
		{
			get
			{
				if (_connectionString == null)
					throw new Exception("ServerDb has not been initialized.");

				var result = new MySqlConnection(_connectionString);
				result.Open();
				return result;
			}
		}

		public void Init(string host, int port, string user, string pass, string db)
		{
			_connectionString = string.Format("server={0}; port={1}; database={2}; uid={3}; password={4}; pooling=true; min pool size=0; max pool size=100; ConvertZeroDateTime=true", host, port, db, user, pass);
			
			MySqlConnection conn = null;
			try { conn = this.Connection; }
			finally { if (conn != null) conn.Close(); }
		}

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
	}

	public static class MySqlDataReaderExtension
	{
		private static bool IsDBNull(this MySqlDataReader reader, string index)
		{
			return reader.IsDBNull(reader.GetOrdinal(index));
		}

		public static string GetStringSafe(this MySqlDataReader reader, string index)
		{
			if (IsDBNull(reader, index))
				return null;
			else
				return reader.GetString(index);
		}

		public static DateTime GetDateTimeSafe(this MySqlDataReader reader, string index)
		{
			return reader[index] as DateTime? ?? DateTime.MinValue;
		}
	}
}
