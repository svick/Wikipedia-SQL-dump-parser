using System;
using System.Linq;
using WpSqlDumpParser;
using WpSqlDumpParser.EntityCollections;
using WpSqlDumpParser.IO;

namespace ListArticleToArticleLinks
{
    static class Program
    {
        static void Main()
        {
            // path, where the dumps will be downloaded
            CachingStream.CachePath = @"C:\Wikipedia dumps";

            // we won't need other pages, so there's no need to load them into memory
            Pages.Instance.Limiter = pages => pages.Where(p => p.Namespace == Namespaces.Article);

            var pageLinks = PageLinks.Instance.Get("enwiki", DumpsManager.GetLastDumpDate("enwiki"));

            var articleToArticleLinks =
                pageLinks.Where(
                    pl => pl.From != null // because of page limiter above, this will give only links from articles
                          && pl.ToNamespace == Namespaces.Article); // only links to articles

            foreach (var link in articleToArticleLinks)
                Console.WriteLine("{0}->{1}", link.From.Title, link.ToTitle);
        }
    }
}