using System.Collections.Generic;
using System.IO;

namespace WpSqlDumpParser
{
	static class Pages
	{
		public IEnumerable<Page> Get(Stream stream)
		{
			Parser parser = new Parser();
			foreach (var values in parser.Parse(stream, 12))
				yield return new Page(
					values[0].ToInt32(),
					values[1].ToInt32(),
					values[2].ToString(),
					values[5].ToBoolean());
		}

		public Repository<Page> CreateRepository(Stream stream)
		{
			return Repository<Page>.Create(Get(stream));
		}
	}
}