using System;
using System.IO;
using Data;
using Shared.Database;
using Shared.Util.Configuration;
using Shared.Util;
using Core;

namespace Shared
{
	/// <summary>
	/// General methods needed by all servers.
	/// </summary>
	public abstract class ServerMain
	{
		/// <summary>
		/// Tries to find root folder and changes the working directory to it.
		/// Exits if not successful.
		/// </summary>
		public void NavigateToRoot()
		{
			// Go back max 2 folders, the bins should be in [server]/bin/(Debug|Release)
			for (int i = 0; i < 3; ++i)
			{
				if (Directory.Exists("system"))
					return;

				Directory.SetCurrentDirectory("..");
			}

			Log.Error("Unable to find root directory.");
			CliUtil.Exit(1);
		}

		/// <summary>
		/// Tries to call conf's load method, exits on error.
		/// </summary>
		public void LoadConf(BaseConf conf)
		{
			Log.Info("Reading configuration...");

			try
			{
				conf.Load();
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "Unable to read configuration. ({0})", ex.Message);
				CliUtil.Exit(1);
			}
		}

		/// <summary>
		/// Tries to initialize database with the information from conf, exits on error.
		/// </summary>
		public virtual void InitDatabase(SharedDb db, BaseConf conf)
		{
			Log.Info("Initializing database...");

			try
			{
				db.Init(conf.Database.Host, conf.Database.Port, conf.Database.User, conf.Database.Pass, conf.Database.Db);
			}
			catch (Exception ex)
			{
				Log.Error("Unable to open database connection. ({0})", ex.Message);
				CliUtil.Exit(1);
			}
		}

		/// <summary>
		/// Loads db, first from system, then from user.
		/// Logs problems as warnings.
		/// </summary>
		private void LoadDb(IDatabase db, string path, bool reload, bool log = true)
		{
			var systemPath = Path.Combine("system", path).Replace('\\', '/');
			var userPath = Path.Combine("user", path).Replace('\\', '/');

			var cachePath = Path.Combine("cache", path).Replace('\\', '/');
			cachePath = Path.ChangeExtension(cachePath, "mpk");
			var cacheDir = Path.GetDirectoryName(cachePath);
			if (!Directory.Exists(cacheDir))
				Directory.CreateDirectory(cacheDir);

			if (!File.Exists(systemPath))
				throw new FileNotFoundException("Data file '" + systemPath + "' couldn't be found.", systemPath);

			db.Load(new string[] { systemPath, userPath }, cachePath, reload);

			foreach (var ex in db.Warnings)
				Log.Warning("{0}", ex.ToString());

			if (log)
				Log.Info("  done loading {0} entries from {1}", db.Count, Path.GetFileName(path));
		}

		/// <summary>
		/// Loads system and user localization files.
		/// </summary>
		public void LoadLocalization(BaseConf conf)
		{
			var language = conf.Localization.Language;
			var path = Path.Combine("localization", language + ".po");

			Log.Info("Loading localization ({0})...", language);

			// Try user first
			try
			{
				Localization.Load(Path.Combine("user", path));
			}
			catch (FileNotFoundException)
			{
				// Try system second, if the file wasn't in user
				try
				{
					Localization.Load(Path.Combine("system", path));
				}
				catch (FileNotFoundException)
				{
					// Warn if language wasn't the default
					if (language != "en-US")
						Log.Warning("Localization file '{0}.po' not found.", language);
				}
			}
		}
	}
}