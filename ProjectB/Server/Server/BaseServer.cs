using System;
using System.IO;
using System.Collections.Generic;
using Server.Network;
using Server.Database;
using Server.Configuration;
using Server.Network.Handlers;
using Server.Util;

namespace Server
{
	public partial class ServerApp
	{
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

		public void LoadConf(Config conf)
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

		public void InitDatabase(ServerDb db, Config conf)
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

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				Log.Error("Server has encountered an unexpected and unrecoverable error. We're going to try to save as much as we can.");
			}
			catch { }
			try
			{
				this.Server.Stop();
			}
			catch { }
			try
			{
				// save the world
			}
			catch { }
			try
			{
				Log.Exception((Exception)e.ExceptionObject);
				Log.Status("Closing server.");
			}
			catch { }

			CliUtil.Exit(1, false);
		}
	}
}