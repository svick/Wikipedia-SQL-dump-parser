using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace Wikipedia_language_networks
{
    public class DumpsManager
    {
        private static readonly string SitematrixUrl = "http://en.wikipedia.org/w/api.php?action=sitematrix&format=xml";
        private static readonly string DumpsUrl = "http://dumps.wikimedia.org/";
        private static readonly string UserAgent = "[[User:Svick]] Wikipedia language networks";

        private static WebClient WC
        {
            get
            {
                var wc = new WebClient();
                wc.Headers[HttpRequestHeader.UserAgent] = UserAgent;
                return wc;
            }
        }

        readonly Lazy<IEnumerable<string>> m_wikipedias = new Lazy<IEnumerable<string>>(GetWikipedias);

        public IEnumerable<string> Wikipedias { get { return m_wikipedias.Value; } }

        protected static IEnumerable<string> GetWikipedias()
        {
            XDocument doc;
            using (var sitemaxtrixStream = WC.OpenRead(SitematrixUrl))
            {
                doc = XDocument.Load(sitemaxtrixStream);
            }

            return from lang in doc.Element("api").Element("sitematrix").Elements("language")
                   from site in lang.Element("site").Elements("site")
                   where site.Attribute("closed") == null
                         && site.Attribute("code").Value == "wiki"
                   select lang.Attribute("code").Value;
        }

        public DateTime GetLastDumpDate(string wiki)
        {
            string directoryListing = null;
            while (directoryListing == null)
            {
                try
                {
                    directoryListing = WC.DownloadString(DumpsUrl + wiki + '/');
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            directoryListing = directoryListing.Replace("&nbsp;", " ");

            XNamespace ns = "http://www.w3.org/1999/xhtml";

            XDocument doc = XDocument.Parse(directoryListing);

            return (from elem in doc.Descendants(ns + "a")
                    select DateTimeExtensions.ParseDate(elem.Value)
                    into date
                    where date != null
                    select date.Value).Max();
        }
    }
}