using Login.Database;
using Login.Network;
using Login.Network.Handlers;
using Login.Util;
using Core.Const;
using Core.Network;
using Shared;
using Shared.Network;
using Shared.Util;
using Swebs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Login
{
	public class LoginServer : ServerMain
	{
		public static readonly LoginServer Instance = new LoginServer();
		
		public DefaultServer<LoginClient> Server { get; set; }
		public ServerInfoManager ServerList { get; private set; }
		public LoginDb Database { get; private set; }
		public LoginConf Conf { get; private set; }
		public List<LoginClient> ChannelClients { get; private set; }

		private LoginServer()
		{			
			this.Server = new DefaultServer<LoginClient>();
			this.Server.Handlers = new LoginServerHandlers();
			this.Server.Handlers.AutoLoad();
			this.Server.ClientDisconnected += this.OnClientDisconnected;

			this.ServerList = new ServerInfoManager();

			this.ChannelClients = new List<LoginClient>();
		}

		public void Run()
		{
			CliUtil.WriteHeader("Login Server", ConsoleColor.Magenta);
			CliUtil.LoadingTitle();

			this.NavigateToRoot();

			// Load
			this.LoadConf(this.Conf = new LoginConf());
			this.InitDatabase(this.Database = new LoginDb(), this.Conf);
			this.CheckDatabaseUpdates();
			this.LoadLocalization(this.Conf);

			// Start
			this.Server.Start(this.Conf.Login.Port);

			CliUtil.RunningTitle();

			// Commands
			var commands = new LoginConsoleCommands();
			commands.Wait();
		}

		private void OnClientDisconnected(LoginClient client)
		{
			var update = false;

			lock (this.ChannelClients)
			{
				if (this.ChannelClients.Contains(client))
				{
					this.ChannelClients.Remove(client);
					update = true;
				}
			}

			if (update)
			{
				var channel = (client.Account != null ? this.ServerList.GetChannel(client.Account.Name) : null);
				if (channel == null)
				{
					Log.Warning("Unregistered channel disconnected.");
					return;
				}
				Log.Status("Channel '{0}' disconnected, switched to Maintenance.", client.Account.Name);
				channel.State = ChannelState.Maintenance;

				Send.ChannelStatus(this.ServerList.List);
				Send.Internal_ChannelStatus(this.ServerList.List);
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
			if (LoginServer.Instance.Database.CheckUpdate(updateFile))
				return;

			Log.Info("Update '{0}' found, executing...", updateFile);

			LoginServer.Instance.Database.RunUpdate(updateFile);
		}
		
		public void BroadcastChannels(Packet packet)
		{
			lock (this.Server.Clients)
			{
				foreach (var client in this.Server.Clients.Where(a => a.State == ClientState.LoggedIn && this.ChannelClients.Contains(a)))
				{
					client.Send(packet);
				}
			}
		}

		public void BroadcastPlayers(Packet packet)
		{
			lock (this.Server.Clients)
			{
				foreach (var client in this.Server.Clients.Where(a => a.State == ClientState.LoggedIn && !this.ChannelClients.Contains(a)))
				{
					client.Send(packet);
				}
			}
		}
		
		public void RequestDisconnect(string accountName)
		{
			var client = this.Server.Clients.FirstOrDefault(a => a.State != ClientState.Dead && a.Account != null && a.Account.Name == accountName);
			if (client != null)
				client.Kill();

			Send.Internal_RequestDisconnect(accountName);
			Thread.Sleep(2000);
			this.Database.SetAccountLoggedIn(accountName, false);
		}
	}
}