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
                                            let languageCount = root.Children.Select(p => p.Language).Distinct().Count()
                                            orderby languageCount descending
                                            select new { root.Children, LanguageCount = languageCount }).ToArray();

            var sections = new[]
                           {
                               new
                               {
                                   name = "Articles",
                                   networks = largestNonEnwikiNetworks
                                   .Where(n => n.Children.Any(p => !p.Title.Contains(':')))
                                   .Take(100)
                               },
                               new
                               {
                                   name = "Other",
                                   networks = largestNonEnwikiNetworks
                                   .Where(n => n.Children.All(p => p.Title.Contains(':')))
                                   .Take(100)
                               }
                           };

            string fileName = "networks without enwiki.txt";
            using (var writer = new StreamWriter(fileName))
            {
                foreach (var section in sections)
                {
                    writer.WriteLine("=={0}==", section.name);

                    writer.WriteLine("{| class=\"wikitable\"");
                    writer.WriteLine("|-");
                    writer.WriteLine("! {0} !! {1} !! {2}", "No.", "Pages", "Count");

                    int i = 0;

                    foreach (var network in section.networks)
                    {
                        var articleLinks = from page in network.Children
                                           orderby page.Language, page.Title
                                           select string.Format("[[:{0}:{1}]]", page.Language, page.Title);
                        var articleLinksString = string.Join(", ", articleLinks);

                        writer.WriteLine("|-");
                        writer.WriteLine("| {0} || {1} || {2}", ++i, articleLinksString, network.LanguageCount);
                    }

                    writer.WriteLine("|}");
                }
            }
        }
    }
}