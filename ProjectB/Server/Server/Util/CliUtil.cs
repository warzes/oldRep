using System;
using System.Linq;

namespace Server.Util
{
	public class CliUtil
	{
		private const string TitlePrefix = "Server : ";

		private static readonly string[] Logo = new string[]
		{
			@"S E R V E R",
		};

		private static void WriteLineCentered(string line, int referenceLength = -1)
		{
			if (referenceLength < 0)
				referenceLength = line.Length;

			Console.WriteLine(line.PadLeft(line.Length + Console.WindowWidth / 2 - referenceLength / 2));
		}

		private static void WriteLinesCentered(string[] lines)
		{
			var longestLine = lines.Max(a => a.Length);
			foreach (var line in lines)
				WriteLineCentered(line, longestLine);
		}

		public static void WriteSeperator()
		{
			Console.WriteLine("".PadLeft(Console.WindowWidth, '_'));
		}
		
		public static void WriteHeader(string consoleTitle, ConsoleColor color)
		{
			Console.Title = TitlePrefix + consoleTitle;

			Console.ForegroundColor = color;
			WriteLinesCentered(Logo);

			Console.WriteLine();
			
			Console.ResetColor();
			WriteSeperator();
		}

		public static void LoadingTitle()
		{
			if (!Console.Title.StartsWith("* "))
				Console.Title = "* " + Console.Title;
		}

		public static void RunningTitle()
		{
			Console.Title = Console.Title.TrimStart('*', ' ');
		}
		public static void Exit(int exitCode, bool wait = true)
		{
			if (wait && UserInteractive)
			{
				Log.Info("Press Enter to exit.");
				Console.ReadLine();
			}
			Log.Info("Exiting...");
			Environment.Exit(exitCode);
		}

		public static bool UserInteractive
		{
			get
			{
#if __MonoCS__
				return (Console.In is System.IO.StreamReader);
#else
				return Environment.UserInteractive;
#endif
			}
		}
	}
}