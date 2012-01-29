using System;
using WpSqlDumpParser.EntityCollections;

namespace WpSqlDumpParser.Entities
{
    public class PageLink
    {
        public int FromId { get; protected set; }
        public int ToNamespaceId { get; protected set; }
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

        public Namespace ToNamespace
        {
            get
            {
                var repo = Repository<Namespace>.Instance;
                if (repo == null)
                    throw new InvalidOperationException();
                return repo.FindById(ToNamespaceId);
            }
        }

        public PageLink(int fromId, int toNamespaceId, string toTitle)
        {
            FromId = fromId;
            ToNamespaceId = toNamespaceId;
            ToTitle = toTitle;
        }
    }
}