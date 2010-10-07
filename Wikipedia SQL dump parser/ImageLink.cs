using System;

namespace WpSqlDumpParser
{
	public class ImageLink
	{
		public int FromId { get; protected set; }
		public string ToTitle { get; protected set; }

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

		public ImageLink(int fromId, string toTitle)
		{
			FromId = fromId;
			ToTitle = toTitle;
		}
	}
}