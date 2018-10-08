using Shared.Util;
using Shared.Util.Configuration;
using System.Collections.Generic;

namespace Login.Util
{
	public class LoginConf : BaseConf
	{
		/// <summary>
		/// login.conf
		/// </summary>
		public LoginConfFile Login { get; protected set; }

		public LoginConf()
		{
			this.Login = new LoginConfFile();
		}

		public override void Load()
		{
			this.LoadDefault();
			this.Login.Load();
		}
	}

	/// <summary>
	/// Represents login.conf
	/// </summary>
	public class LoginConfFile : ConfFile
	{
		public int Port { get; protected set; }

		public bool NewAccounts { get; protected set; }

		public int DeletionWait { get; protected set; }
		
		public void Load()
		{
			this.Require("system/conf/login.conf");

			this.Port = this.GetInt("port", 11000);
			this.NewAccounts = this.GetBool("new_accounts", true);

			this.DeletionWait = this.GetInt("deletion_wait", 107);
		}
	}
}