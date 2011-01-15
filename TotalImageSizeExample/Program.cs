using System;
using System.Linq;
using WpSqlDumpParser.EntityCollections;
using WpSqlDumpParser.IO;

namespace ChineseCategoryLinks
{
	class Program
	{
		static void Main()
		{
			CachingStream.CachePath = @"F:\Wikipedia dumps";

			var categoryLinks = CategoryLinks.Instance.Get("zhwiki", new DateTime(2010, 11, 09));

			foreach (var cl in categoryLinks)
			{
				Console.Write("{0}\t{1}", cl.From.FullName, cl.ToTitle);
				Console.ReadLine();
			}
		}
	}
}