﻿using System;
using System.IO;
using System.Reflection;
using Shared.Util;
using CSScriptLibrary;
using System.Text.RegularExpressions;
using System.Text;

namespace Shared.Scripting.Compilers
{
	/// <summary>
	/// C# compiler, utilizing CSScript
	/// </summary>
	public class CSharpCompiler : Compiler
	{
		public override Assembly Compile(string path, string outPath, bool cache)
		{
			Assembly asm = null;
			try
			{
				// Get from cache?
				if (this.ExistsAndUpToDate(path, outPath) && cache)
					return Assembly.LoadFrom(outPath);

				// Precompile script to a temp file
				var precompiled = this.PreCompile(File.ReadAllText(path));
				var tmp = Path.GetTempFileName();
				File.WriteAllText(tmp, precompiled);

#if DEBUG
				var debug = true;
#else
				var debug = false;
#endif

				// Compile
				// Mono needs the settings to not treat harmless warnings as errors (like a missing await in an async Task) and to not spam us with warnings.
				asm = CSScript.LoadWithConfig(tmp, null, debug, CSScript.GlobalSettings, "/warnaserror- /warn:0");

				this.SaveAssembly(asm, outPath);
			}
			catch (csscript.CompilerException ex)
			{
				var errors = ex.Data["Errors"] as System.CodeDom.Compiler.CompilerErrorCollection;
				var newExs = new CompilerErrorsException();

				foreach (System.CodeDom.Compiler.CompilerError err in errors)
				{
					var newEx = new CompilerError(path, err.Line, err.Column, err.ErrorText, err.IsWarning);
					newExs.Errors.Add(newEx);
				}

				throw newExs;
			}
			catch (UnauthorizedAccessException)
			{
				// Thrown if file can't be copied. Happens if script was initially loaded from cache.
				// TODO: Also thrown if CS-Script can't create the file, ie under Linux, if /tmp/CSSCRIPT isn't writeable.
				//   Handle that somehow?
			}
			catch (Exception ex)
			{
				Log.Exception(ex);
			}

			return asm;
		}
	}
}