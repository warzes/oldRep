using System;
using Server.Util;

namespace Server
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				ServerApp.Instance.Run();
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "An exception occured while starting the server.");
				CliUtil.Exit(1);
			}
		}
	}
}