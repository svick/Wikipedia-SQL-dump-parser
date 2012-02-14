using System;
using WpSqlDumpParser.EntityCollections;

namespace WpSqlDumpParser.Entities
{
    public enum CategoryLinkType
    {
        Page,
        Subcat,
        File
    }

	public class CategoryLink
	{
		public int FromId { get; protected set; }
		public string ToTitle { get; protected set; }
		public string SortKey { get; protected set; }
        public CategoryLinkType Type { get; protected set; }

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

		public CategoryLink(int fromId, string toTitle, string sortKey, CategoryLinkType type)
		{
			FromId = fromId;
			ToTitle = toTitle;
			SortKey = sortKey;
		    Type = type;
		}
	}
}