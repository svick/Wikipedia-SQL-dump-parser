using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpSqlDumpParser
{
	static class Namespaces
	{
		public static void CreateRepository(string project = "Project")
		{
			Namespace[] namespaces = new[]
			{
				new Namespace(0, ""),
				new Namespace(1, "Talk"),
				new Namespace(2, "User"),
				new Namespace(3, "User talk"),
				new Namespace(4, project),
				new Namespace(5, project + " talk"),
				new Namespace(6, "File"),
				new Namespace(7, "File talk"),
				new Namespace(8, "MediaWiki"),
				new Namespace(9, "MediaWiki talk"),
				new Namespace(10, "Template"),
 				new Namespace(11, "Template talk"),
				new Namespace(12, "Help"),
				new Namespace(13, "Help talk"),
				new Namespace(14, "Category"),
				new Namespace(15, "Category talk"),
				new Namespace(100, "Portal"),
				new Namespace(101, "Portal talk"),
				new Namespace(108, "Book"),
				new Namespace(109, "Book talk")
			};
			Repository<Namespace>.Create(namespaces);
		}
	}
}