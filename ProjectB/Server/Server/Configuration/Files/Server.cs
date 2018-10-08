namespace Server.Configuration.Files
{
	public class ServerConfFile : ConfFile
	{
		public int Port { get; protected set; }
		public bool NewAccounts { get; protected set; }

		public void Load()
		{
			this.Require("system/conf/server.conf");

			this.Port = this.GetInt("port", 11000);
			this.NewAccounts = this.GetBool("new_accounts", true);
		}
	}
}
