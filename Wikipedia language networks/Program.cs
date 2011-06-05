using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WpSqlDumpParser.EntityCollections;
using WpSqlDumpParser.IO;
using WpSqlDumpParser;

namespace Wikipedia_language_networks
{
    static class Program
    {
        static void Main()
        {
            DownloadStream.Log = true;
            CachingStream.CachePath = @"C:\Wikipedia dumps\";

            var networks = Networks.Instance;

            var dumpsManager = new DumpsManager();

            foreach (var lang in dumpsManager.Wikipedias)
            {
                string dumpName = lang.Replace('-', '_') + "wiki";

                Console.Out.Log(string.Format("Processing {0}.", dumpName));

                DateTime date = dumpsManager.GetLastDumpDate(dumpName);

                var langLinks =
                    LangLinks.Instance.Get(dumpName, date).Where(ll => ll.From != null && ll.From.NamespaceId != 2); // no user pages
                networks.ProcessDump(lang, langLinks);
            }

            var largestNonEnwikiNetworks = (from root in networks.Roots
                                           where !root.Children.Any(p => p.Language == "en")
                                           orderby root.Children.Select(p => p.Language).Distinct().Count() descending
                                           select root.Children).Take(50);

            string fileName = "networks without enwiki.txt";
            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine("{| class=\"wikitable\"");
                writer.WriteLine("|-");
                writer.WriteLine("! {0} !! {1} !! {2}", "No.", "Articles", "Count");

                int i = 0;

                foreach (var network in largestNonEnwikiNetworks)
                {
                    var articleLinks = from page in network
                                       orderby page.Language, page.Title
                                       select string.Format("[[:{0}:{1}]]", page.Language, page.Title);
                    var articleLinksString = string.Join(", ", articleLinks);

                    writer.WriteLine("|-");
                    writer.WriteLine("| {0} || {1} || {2}", ++i, articleLinksString, network.Count);
                }

                writer.WriteLine("|}");
            }
        }
    }
}