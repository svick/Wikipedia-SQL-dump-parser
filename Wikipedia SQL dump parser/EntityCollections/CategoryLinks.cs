using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WpSqlDumpParser.Entities;
using WpSqlDumpParser.Parsing;

namespace WpSqlDumpParser.EntityCollections
{
	public class CategoryLinks : Dump<CategoryLink>
	{
		static CategoryLinks instance;
		public static CategoryLinks Instance
		{
			get
			{
				if (instance == null)
					instance = new CategoryLinks();
				return instance;
			}
		}

		private CategoryLinks()
		{ }

		public override string Name
		{
			get { return "categorylinks"; }
		}

		public override IEnumerable<CategoryLink> Get(Stream stream)
		{
			Parser parser = new Parser();
			var result = from values in parser.Parse(stream)
									 select new CategoryLink(
										 values["from"].ToInt32(),
										 values["to"].ToString(),
										 values["sortkey"].ToString()) into categoryLink
									 where categoryLink.FromId != 0
									 select categoryLink;

			if (Limiter != null)
				result = Limiter(result);
			return result;
		}

		public override IEnumerable<CategoryLink> Get(string wiki, DateTime date)
		{
			return Get(wiki, date, true);
		}

		public IEnumerable<CategoryLink> Get(string wiki, DateTime date, bool downloadPages)
		{
			if (downloadPages && Repository<Page>.Instance == null)
				Pages.Instance.CreateRepository(wiki, date);

			return base.Get(wiki, date);
		}
	}
}