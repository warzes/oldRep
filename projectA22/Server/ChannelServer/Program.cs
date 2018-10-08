using System;
using Shared.Util;

namespace Channel
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				ChannelServer.Instance.Run();
			}
			catch (Exception ex)
			{
				Log.Exception(ex, "An exception occured while starting the server.");
				CliUtil.Exit(1);
			}
		}
	}
}
