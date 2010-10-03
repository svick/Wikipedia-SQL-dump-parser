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
			Parser parser = new Parser();
			return from values in parser.Parse(stream, 12)
						 select new Page(
							 values[0].ToInt32(),
							 values[1].ToInt32(),
							 values[2].ToString(),
							 values[5].ToBoolean());
		}
	}
}