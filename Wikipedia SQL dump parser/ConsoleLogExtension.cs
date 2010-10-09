using System;
using System.IO;

namespace WpSqlDumpParser
{
	static class ConsoleLogExtension
	{
		public static void Log(this TextWriter console, string message)
		{
			console.WriteLine("{0:dd.MM.yyyy HH:mm:ss} {1}", DateTime.Now, message);
		}
	}
}