using System;
using WpSqlDumpParser.EntityCollections;

namespace WpSqlDumpParser.Entities
{
    public class PageProp
    {
        public int PageId { get; protected set; }
        public string Name { get; protected set; }
        public string Value { get; protected set; }
        public float? SortKey { get; protected set; }

        public Page Page
        {
            get
            {
                var repo = Repository<Page>.Instance;
                if (repo == null)
                    throw new InvalidOperationException();
                return repo.FindById(PageId);
            }
        }

        public PageProp(int pageId, string name, string value, float? sortKey)
        {
            PageId = pageId;
            Name = name;
            Value = value;
            SortKey = sortKey;
        }
    }
}