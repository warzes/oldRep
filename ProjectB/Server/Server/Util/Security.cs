﻿using Server.Network;
using System;
using System.Diagnostics;
using System.Linq;

namespace Server.Util
{
	public sealed class SecurityViolationEventArgs : EventArgs
	{
		public Client Client { get; private set; }
		public string Report { get; private set; }
		public string StackReport { get; private set; }
		public IncidentSeverityLevel Level { get; private set; }

		public SecurityViolationEventArgs(Client offender, IncidentSeverityLevel level, string report, string stacktrace)
		{
			this.Client = offender;
			this.Level = level;
			this.Report = report;
			this.StackReport = stacktrace;
		}
	}

	/// <summary>
	/// The base for the exceptions to throw when someone does something bad
	/// </summary>
	public abstract class SecurityViolationException : Exception
	{
		private const int _stackDepth = 5;

		private readonly string _message;

		/// <summary>
		/// Details on what happened
		/// </summary>
		public override string Message { get { return _message; } }

		/// <summary>
		/// A short stack trace to identify where the incident occured.
		/// </summary>
		public string StackReport { get; private set; }

		/// <summary>
		/// How bad it was
		/// </summary>
		public IncidentSeverityLevel Level { get; private set; }

		protected SecurityViolationException(IncidentSeverityLevel lvl, string report)
		{
			this.Level = lvl;

			var stacktrace = new StackTrace(2); // Skip 2 frames for this and calling ctor

			this.StackReport = string.Join(" --> ",
				stacktrace.GetFrames()
				.Take(_stackDepth)
				.Reverse()
				.Select(n => n.GetMethod().DeclaringType.Name + "." + n.GetMethod().Name));

			_message = string.Format("{0}: {1}", stacktrace.GetFrame(0).GetMethod().Name, report);
		}
	}

	/// <summary>
	/// Used to indicate something suspicious happened, but it
	/// *could* be nothing. This setting should be rarely used...
	/// </summary>
	public sealed class MildViolation : SecurityViolationException
	{
		public MildViolation(string report, params object[] args)
			: base(IncidentSeverityLevel.Mild, string.Format(report, args))
		{
		}
	}

	/// <summary>
	/// Something that is a strong indicator for a hack, but not certain.
	/// </summary>
	public sealed class ModerateViolation : SecurityViolationException
	{
		public ModerateViolation(string report, params object[] args)
			: base(IncidentSeverityLevel.Moderate, string.Format(report, args))
		{
		}
	}

	/// <summary>
	/// Something happened that could really only be caused by a hack tool
	/// </summary>
	public sealed class SevereViolation : SecurityViolationException
	{
		public SevereViolation(string report, params object[] args)
			: base(IncidentSeverityLevel.Severe, string.Format(report, args))
		{
		}
	}

	public enum IncidentSeverityLevel
	{
		Mild = 1,
		Moderate,
		Severe
	}
}