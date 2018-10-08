using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Configuration.Files;

namespace Server.Configuration
{
	public class Config
	{
		/// <summary>
		/// log.conf
		/// </summary>
		public LogConfFile Log { get; protected set; }

		/// <summary>
		/// database.conf
		/// </summary>
		public DatabaseConfFile Database { get; private set; }

		/// <summary>
		/// server.conf
		/// </summary>
		public ServerConfFile Server { get; protected set; }


		public Config()
		{
			this.Log = new LogConfFile();
			this.Database = new DatabaseConfFile();
			this.Server = new ServerConfFile();
		}

		public void Load()
		{
			this.Log.Load();
			this.Database.Load();
			this.Server.Load();
		}
	}
}