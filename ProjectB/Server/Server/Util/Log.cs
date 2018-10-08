﻿using System;
using System.Globalization;
using System.IO;

namespace Server.Util
{
	[Flags]
	public enum LogLevel : int
	{
		Info = 0x0001,
		Warning = 0x0002,
		Error = 0x0004,
		Debug = 0x0008,
		Status = 0x0010,
		Exception = 0x0020,
		Unimplemented = 0x0040,
		None = 0x7FFF,
	}

	public static class Log
	{
		private static string _logFile;
		public static LogLevel Hide { get; set; }
		public static string Archive { get; set; }

		public static string LogFile
		{
			get { return _logFile; }
			set
			{
				if (value != null)
				{
					var pathToFile = Path.GetDirectoryName(value);

					if (!Directory.Exists(pathToFile))
						Directory.CreateDirectory(pathToFile);

					if (File.Exists(value))
					{
						if (Archive != null)
						{
							if (!Directory.Exists(Archive))
								Directory.CreateDirectory(Archive);

							var time = File.GetLastWriteTime(value);
							var archive = Path.Combine(Archive, time.ToString("yyyy-MM-dd_HH-mm"));
							var archiveFilePath = Path.Combine(archive, Path.GetFileName(value));

							if (!Directory.Exists(archive))
								Directory.CreateDirectory(archive);

							if (File.Exists(archiveFilePath))
								File.Delete(archiveFilePath);

							File.Move(value, archiveFilePath);
						}

						File.Delete(value);
					}
				}

				_logFile = value;
			}
		}

		public static void Info(string format, params object[] args)
		{
			WriteLine(LogLevel.Info, format, args);
		}

		public static void Warning(string format, params object[] args)
		{
			WriteLine(LogLevel.Warning, format, args);
		}

		public static void Error(string format, params object[] args)
		{
			WriteLine(LogLevel.Error, format, args);
		}

		public static void Debug(string format, params object[] args)
		{
			WriteLine(LogLevel.Debug, format, args);
		}

		public static void Debug(object obj)
		{
			WriteLine(LogLevel.Debug, obj.ToString());
		}

		public static void Status(string format, params object[] args)
		{
			WriteLine(LogLevel.Status, format, args);
		}

		public static void Exception(Exception ex, string description = null, params object[] args)
		{
			if (description != null)
			{
				if (Hide.HasFlag(LogLevel.Exception))
					description += " See log file for more details.";

				WriteLine(LogLevel.Error, description, args);
			}

			WriteLine(LogLevel.Exception, ex.ToString());
		}

		public static void Unimplemented(string format, params object[] args)
		{
			WriteLine(LogLevel.Unimplemented, format, args);
		}

		public static void Progress(int current, int max)
		{
			var donePerc = (100f / max * current);
			var done = (int)Math.Min(20, Math.Ceiling(20f / max * current));

			Write(LogLevel.Info, false, "[" + ("".PadRight(done, '#') + "".PadLeft(20 - done, '.')) + "] {0,5:0.0}%\r", donePerc);
		}

		public static void WriteLine(LogLevel level, string format, params object[] args)
		{
			Write(level, format + Environment.NewLine, args);
		}

		public static void WriteLine()
		{
			WriteLine(LogLevel.None, "");
		}

		public static void Write(LogLevel level, string format, params object[] args)
		{
			Write(level, true, format, args);
		}

		private static void Write(LogLevel level, bool toFile, string format, params object[] args)
		{
			lock (Console.Out)
			{
				if (!Hide.HasFlag(level))
				{
					switch (level)
					{
						case LogLevel.Info: Console.ForegroundColor = ConsoleColor.White; break;
						case LogLevel.Warning: Console.ForegroundColor = ConsoleColor.Yellow; break;
						case LogLevel.Error: Console.ForegroundColor = ConsoleColor.Red; break;
						case LogLevel.Debug: Console.ForegroundColor = ConsoleColor.Cyan; break;
						case LogLevel.Status: Console.ForegroundColor = ConsoleColor.Green; break;
						case LogLevel.Exception: Console.ForegroundColor = ConsoleColor.DarkRed; break;
						case LogLevel.Unimplemented: Console.ForegroundColor = ConsoleColor.DarkGray; break;
					}

					if (level != LogLevel.None)
						Console.Write("[{0}]", level);

					Console.ForegroundColor = ConsoleColor.Gray;

					if (level != LogLevel.None)
						Console.Write(" - ");

					Console.Write(format, args);
				}

				if (_logFile != null && toFile)
				{
					using (var file = new StreamWriter(_logFile, true))
					{
						file.Write("{0:yyyy-MM-dd HH:mm} ", DateTime.Now);
						if (level != LogLevel.None)
							file.Write("[{0}] - ", level);
						file.Write(format, args);
						file.Flush();
					}
				}
			}
		}
	}
}