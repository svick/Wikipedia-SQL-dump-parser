namespace WpSqlDumpParser
{
	static class ProjectName
	{
		public static string FromDatabaseName(string databaseName)
		{
			if (databaseName == "enwiki")
				return "Wikipedia";
			return null;
		}
	}
}