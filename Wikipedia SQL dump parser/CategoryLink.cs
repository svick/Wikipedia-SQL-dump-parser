using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpSqlDumpParser
{
	public class CategoryLink
	{
		public int FromId { get; protected set; }
		public string ToTitle { get; protected set; }
		public string SortKey { get; protected set; }

		public Page From
		{
			get
			{
				var repo = Repository<Page>.Instance;
				if (repo == null)
					throw new InvalidOperationException();
				return repo.FindById(FromId);
			}
		}

		public CategoryLink(int fromId, string toTitle, string sortKey)
		{
			FromId = fromId;
			ToTitle = toTitle;
			SortKey = sortKey;
		}
	}
}