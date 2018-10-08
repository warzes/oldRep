using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Server.Network;
using Server.Database;
using Server.Configuration;
using Server.Network.Handlers;
using Server.Util;
using Server.Util.Commands;
using Server.World;

namespace Server
{
	public partial class ServerApp
	{
		public static readonly ServerApp Instance = new ServerApp();

		public Network.Server Server { get; set; }
		public List<Client> Clients { get; private set; }

		public ServerDb Database { get; private set; }

		public Config Conf { get; private set; }

		public WorldManager World { get; private set; }


		private ServerApp()
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			this.Server = new Network.Server();
			this.Server.Handlers = new ServerHandlers();
			this.Server.Handlers.AutoLoad();
			this.Server.ClientDisconnected += this.OnClientDisconnected;

			this.Clients = new List<Client>();
		}

		public void Run()
		{
			CliUtil.WriteHeader("Login Server", ConsoleColor.Magenta);
			CliUtil.LoadingTitle();

			this.NavigateToRoot();

			this.LoadConf(this.Conf = new Config());
			this.InitDatabase(this.Database = new ServerDb(), this.Conf);

			this.InitializeWorld();

			this.Server.Start(this.Conf.Server.Port);

			CliUtil.RunningTitle();

			// Commands
			var commands = new ConsoleCommands();
			commands.Wait();
		}

		private void InitializeWorld()
		{
			Log.Info("Initializing world...");

			this.World = new WorldManager();
			this.World.Initialize();
		}

		private void OnClientDisconnected(Client client)
		{
			var update = false;

			lock (this.Clients)
			{
				if (this.Clients.Contains(client))
				{
					this.Clients.Remove(client);
					update = true;
				}
			}

			if (update)
			{
				//var channel = (client.Account != null ? this.ServerList.GetChannel(client.Account.Name) : null);
				//if (channel == null)
				//{
				//	Log.Warning("Unregistered channel disconnected.");
				//	return;
				//}
				//Log.Status("Channel '{0}' disconnected, switched to Maintenance.", client.Account.Name);
				//channel.State = ChannelState.Maintenance;

				//Send.ChannelStatus(this.ServerList.List);
				//Send.Internal_ChannelStatus(this.ServerList.List);
			}
		}

		private void CheckDatabaseUpdates()
		{
			Log.Info("Checking for updates...");

			var files = Directory.GetFiles("sql");
			foreach (var filePath in files.Where(file => Path.GetExtension(file).ToLower() == ".sql"))
				this.RunUpdate(Path.GetFileName(filePath));
		}

		private void RunUpdate(string updateFile)
		{
			if (this.Database.CheckUpdate(updateFile))
				return;

			Log.Info("Update '{0}' found, executing...", updateFile);

			this.Database.RunUpdate(updateFile);
		}

	}
}