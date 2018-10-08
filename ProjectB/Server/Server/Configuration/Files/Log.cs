﻿using Server.Util;

namespace Server.Configuration.Files
{
	/// <summary>
	/// Represents log.conf
	/// </summary>
	public class LogConfFile : ConfFile
	{
		public bool Archive { get; protected set; }
		public LogLevel Hide { get; protected set; }

		public void Load()
		{
			this.Require("system/conf/log.conf");

			this.Archive = this.GetBool("archive", true);
			this.Hide = (LogLevel)this.GetInt("cmd_hide", (int)(LogLevel.Debug));

			if (this.Archive)
				Log.Archive = "log/archive/";
			Log.LogFile = string.Format("log/{0}.txt", System.AppDomain.CurrentDomain.FriendlyName.Replace(".exe", "").Replace(".vshost", ""));
			Log.Hide |= this.Hide;
		}
	}
}
