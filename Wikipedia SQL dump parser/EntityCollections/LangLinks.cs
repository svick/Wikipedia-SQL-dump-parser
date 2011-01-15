using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WpSqlDumpParser.Entities;
using WpSqlDumpParser.Parsing;

namespace WpSqlDumpParser.EntityCollections
{
	class LangLinks : Dump<LangLink>
	{
		static LangLinks instance;
		public static LangLinks Instance
		{
			get
			{
				if (instance == null)
					instance = new LangLinks();
				return instance;
			}
		}

		private LangLinks()
		{ }

		public override string Name
		{
			get { return "langlinks"; }
		}

		public override IEnumerable<LangLink> Get(Stream stream)
		{
			Parser parser = new Parser();
			var result = from values in parser.Parse(stream)
									 select new LangLink(
										 values["from"].ToInt32(),
										 values["lang"].ToString(),
										 values["title"].ToString()) into categoryLink
									 where categoryLink.FromId != 0
									 select categoryLink;

			if (Limiter != null)
				result = Limiter(result);
			return result;
		}

		public override IEnumerable<LangLink> Get(string wiki, DateTime date)
		{
			return Get(wiki, date, true);
		}

		public IEnumerable<LangLink> Get(string wiki, DateTime date, bool downloadPages)
		{
			if (downloadPages && Repository<Page>.Instance == null)
				Pages.Instance.CreateRepository(wiki, date);

			return base.Get(wiki, date);
		}
	}
}