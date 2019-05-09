using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WpSqlDumpParser.Entities;
using WpSqlDumpParser.Parsing;

namespace WpSqlDumpParser.EntityCollections
{
    public class PageProps : Dump<PageProp>
    {
        private static PageProps instance;

        public static PageProps Instance
        {
            get
            {
                if (instance == null)
                    instance = new PageProps();
                return instance;
            }
        }

        private PageProps()
        { }

        public override string Name
        {
            get { return "page_props"; }
        }

        public override IEnumerable<PageProp> Get(Stream stream)
        {
            var parser = new Parser();
            var result = from values in parser.Parse(stream)
                select new PageProp(
                    values["page"].ToInt32(),
                    values["propname"].ToString(),
                    values["value"].ToString(),
                    values["sortkey"].ToNullableFloat());

            if (Limiter != null)
                result = Limiter(result);
            return result;
        }

        public override IEnumerable<PageProp> Get(string wiki, DateTime date)
        {
            return Get(wiki, date, true);
        }

        public IEnumerable<PageProp> Get(string wiki, DateTime date, bool downloadPages)
        {
            if (downloadPages && Repository<Page>.Instance == null)
                Pages.Instance.CreateRepository(wiki, date);

            return base.Get(wiki, date);
        }
    }
}