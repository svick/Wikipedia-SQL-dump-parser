using System.Collections.Generic;
using System.Linq;
using WpSqlDumpParser.Entities;

namespace Wikipedia_language_networks
{
    public class Networks
    {
        public static readonly Networks Instance = new Networks();

        private readonly Dictionary<string, Dictionary<string, Page>> m_pages =
            new Dictionary<string, Dictionary<string, Page>>();

        private readonly HashSet<Page> m_roots = new HashSet<Page>();

        public IEnumerable<Page> Roots { get { return m_roots; } }

        public IEnumerable<Page> Pages
        {
            get
            {
                return from langPages in m_pages.Values
                       from page in langPages.Values
                       select page;
            }
        }

        public Page CreateOrGet(string language, string title)
        {
            title = title.Replace('_', ' ');

            if (!m_pages.ContainsKey(language))
                m_pages[language] = new Dictionary<string, Page>();
            var languagePages = m_pages[language];
            if (!languagePages.ContainsKey(title))
            {
                var page = new Page(language, title);
                languagePages[title] = page;
                m_roots.Add(page);
            }
            return languagePages[title];
        }

        public Page Get(string language, string title)
        {
            title = title.Replace('_', ' ');

            if (!m_pages.ContainsKey(language))
                return null;

            var languagePages = m_pages[language];
            if (!languagePages.ContainsKey(title))
                return null;

            return languagePages[title];
        }

        protected Networks()
        { }

        public void Connect(Page page1, Page page2)
        {
            var root1 = page1.FindRoot();
            var root2 = page2.FindRoot();

            if (root1 == root2)
                return;

            if (root1.Children.Count < root2.Children.Count)
                SetRoot(root1, root2);
            else
                SetRoot(root2, root1);
        }

        void SetRoot(Page oldRoot, Page newRoot)
        {
            oldRoot.Parent = newRoot;
            newRoot.Children.AddRange(oldRoot.Children);
            newRoot.Children.Add(oldRoot);
            oldRoot.Children = null;
            m_roots.Remove(oldRoot);
        }

        public void ProcessDump(string language, IEnumerable<LangLink> langLinks)
        {
            foreach (var langLink in langLinks)
            {
                if (langLink.From == null)
                    continue;
                var from = CreateOrGet(language, langLink.From.Title);
                var to = Get(langLink.Lang, langLink.Title);
                if (to != null)
                    Connect(from, to);
            }
        }
    }
}