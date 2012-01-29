using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WpSqlDumpParser.Entities;
using WpSqlDumpParser.Parsing;

namespace WpSqlDumpParser.EntityCollections
{
    public class PageLinks : Dump<PageLink>
    {
        private static PageLinks instance;

        public static PageLinks Instance
        {
            get
            {
                if (instance == null)
                    instance = new PageLinks();
                return instance;
            }
        }

        private PageLinks()
        { }

        public override string Name
        {
            get { return "pagelinks"; }
        }

        public override IEnumerable<PageLink> Get(Stream stream)
        {
            var parser = new Parser();
            var result = from values in parser.Parse(stream)
                         select new PageLink(
                             values["from"].ToInt32(),
                             values["namespace"].ToInt32(),
                             values["title"].ToString()) into pageLink
                         where pageLink.FromId != 0
                         select pageLink;

            if (Limiter != null)
                result = Limiter(result);
            return result;
        }

        public override IEnumerable<PageLink> Get(string wiki, DateTime date)
        {
            return Get(wiki, date, true);
        }

        public IEnumerable<PageLink> Get(string wiki, DateTime date, bool downloadPages)
        {
            if (downloadPages && Repository<Page>.Instance == null)
                Pages.Instance.CreateRepository(wiki, date);

            return base.Get(wiki, date);
        }
    }
}