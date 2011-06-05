using System.Collections.Generic;

namespace Wikipedia_language_networks
{
    public class Page
    {
        public string Language { get; protected set; }
        public string Title { get; protected set; }
        public Page Parent { get; set; }
        public List<Page> Children { get; set; }

        public Page(string language, string title)
        {
            Language = language;
            Title = title;
            Children = new List<Page>();
        }

        public Page FindRoot()
        {
            var path = new List<Page>();

            var predecessor = this;
            while (predecessor.Parent != null)
            {
                path.Add(predecessor);
                predecessor = predecessor.Parent;
            }

            foreach (var page in path)
                page.Parent = predecessor;

            return predecessor;
        }

        public override string ToString()
        {
            return Language + ':' + Title;
        }
    }
}