using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpSqlDumpParser
{
	static class ProjectName
	{
		public static string FromDatabaseName(string databaseName)
		{
			if (databaseName.EndsWith("wiki"))
				return "Wikipedia";
			return null;
		}
	}
}