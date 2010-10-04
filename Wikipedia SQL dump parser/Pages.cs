using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpSqlDumpParser
{
	public class Pages : DumpWithId<Page>
	{
		static Pages instance;
		public static Pages Instance
		{
			get
			{
				if (instance == null)
					instance = new Pages();
				return instance;
			}
		}

		private Pages()
		{ }

		public override string Name
		{
			get { return "page"; } 
		}

		public override IEnumerable<Page> Get(Stream stream)
		{
			if (Repository<Namespace>.Instance == null)
				Namespaces.CreateRepository();

			Parser parser = new Parser();

			var result = from values in parser.Parse(stream)
									 select new Page(
										 values["id"].ToInt32(),
										 values["namespace"].ToInt32(),
										 values["title"].ToString(),
										 values["is_redirect"].ToBoolean());

			if (Limiter != null)
				result = Limiter(result);
			return result;
		}

		public override Repository<Page> CreateRepository(string wiki, DateTime date)
		{
			if (Repository<Namespace>.Instance == null)
				Namespaces.CreateRepository(ProjectName.FromDatabaseName(wiki));

			return base.CreateRepository(wiki, date);
		}
	}
}