using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpSqlDumpParser
{
	class CategoryLinks : Dump<CategoryLink>
	{
		CategoryLinks instance;
		public CategoryLinks Instance
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
			return from values in parser.Parse(stream, 4)
						 select new CategoryLink(
							 values[0].ToInt32(),
							 values[1].ToString(),
							 values[2].ToString()) into categoryLink
						 where categoryLink.FromId != 0
						 select categoryLink;
		}

		public override IEnumerable<CategoryLink> Get(string wiki, DateTime date)
		{
			return Get(wiki, date, true);
		}

		public IEnumerable<CategoryLink> Get(string wiki, DateTime date, bool downloadPages)
		{
			if (Repository<Page>.Instance == null)
				Pages.Instance.CreateRepository(wiki, date);

			return base.Get(wiki, date);
		}
	}
}