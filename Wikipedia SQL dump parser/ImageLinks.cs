using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WpSqlDumpParser
{
	public class ImageLinks : Dump<ImageLink>
	{
		static ImageLinks instance;
		public static ImageLinks Instance
		{
			get
			{
				if (instance == null)
					instance = new ImageLinks();
				return instance;
			}
		}

		private ImageLinks()
		{ }

		public override string Name
		{
			get { return "imagelinks"; }
		}

		public override IEnumerable<ImageLink> Get(Stream stream)
		{
			Parser parser = new Parser();
			var result = from values in parser.Parse(stream)
									 select new ImageLink(
										 values["from"].ToInt32(),
										 values["to"].ToString()) into imageLink
									 where imageLink.FromId != 0
									 select imageLink;

			if (Limiter != null)
				result = Limiter(result);
			return result;
		}

		public override IEnumerable<ImageLink> Get(string wiki, DateTime date)
		{
			return Get(wiki, date, true);
		}

		public IEnumerable<ImageLink> Get(string wiki, DateTime date, bool downloadPages)
		{
			if (downloadPages && Repository<Page>.Instance == null)
				Pages.Instance.CreateRepository(wiki, date);

			return base.Get(wiki, date);
		}
	}
}