using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shared.Scripting.Compilers
{
	public class CompilerErrorsException : Exception
	{
		public List<CompilerError> Errors { get; protected set; }

		public CompilerErrorsException()
		{
			this.Errors = new List<CompilerError>();
		}
	}

	public class CompilerError : Exception
	{
		public string File { get; protected set; }
		public int Line { get; protected set; }
		public int Column { get; protected set; }
		public bool IsWarning { get; protected set; }

		public CompilerError(string file, int line, int column, string message, bool isWarning)
			: base(message)
		{
			this.File = file;
			this.Line = line;
			this.Column = column;
			this.IsWarning = isWarning;
		}
	}
}