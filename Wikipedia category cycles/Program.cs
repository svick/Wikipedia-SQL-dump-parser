using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using WpSqlDumpParser;
using WpSqlDumpParser.Entities;
using WpSqlDumpParser.EntityCollections;
using WpSqlDumpParser.IO;

namespace WpCategoryCycles
{
    static class Program
	{
		static Stack<Tuple<Category, Queue<Category>>> stack;

		static void Main()
		{
		    var settings = Settings.Default;

		    Console.Write("Cache path [{0}]: ", settings.CachePath);
			string cachePath = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(cachePath))
				cachePath = settings.CachePath;
			settings.CachePath = cachePath;
			CachingStream.CachePath = cachePath;

			Console.Write("Wiki [{0}]: ", settings.Wiki);
			string wiki = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(wiki))
				wiki = settings.Wiki;
			settings.Wiki = wiki;

		    var rootCategories = settings.RootCatgories;
            if (rootCategories == null || rootCategories.DocumentElement.Name != "wikis")
            {
                rootCategories = new XmlDocument();
                var root = rootCategories.CreateElement("wikis");
                rootCategories.AppendChild(root);
                settings.RootCatgories = rootCategories;
            }
		    var elements = rootCategories.DocumentElement.GetElementsByTagName(wiki);
		    XmlElement wikiElement;
		    string oldRootCategory;
		    if (elements.Count != 0)
		    {
		        wikiElement = (XmlElement)elements[0];
		        oldRootCategory = wikiElement.InnerText;
		    }
		    else
		    {
		        wikiElement = rootCategories.CreateElement(wiki);
		        rootCategories.DocumentElement.AppendChild(wikiElement);
		        oldRootCategory = settings.RootCategory;
		    }
		    Console.Write("Root category [{0}]: ", oldRootCategory);
			string rootCategory = Console.ReadLine();
		    if (string.IsNullOrWhiteSpace(rootCategory))
		        rootCategory = oldRootCategory;
		    wikiElement.InnerText = rootCategory;
			settings.RootCategory = rootCategory;

		    var defaultDate = DumpsManager.GetLastDumpDate(wiki).ToString("yyyMMdd");
			Console.Write("Date [{0}]: ", defaultDate);
			string dateString = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(dateString))
			    dateString = defaultDate;

			settings.Save();

			DownloadStream.Log = true;

			PlaintextFile = string.Format("{0}-{1}-cycles.txt", wiki, dateString);
			File.Delete(PlaintextFile);
            WikitextFile = string.Format("{0}-{1}-cycles.wiki", wiki, dateString);
            File.Delete(WikitextFile);

			DateTime date = DateTime.ParseExact(dateString, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

			Pages.Instance.Limiter = pages => pages.Where(p => p.Namespace == Namespaces.Category);

			var categories = new Dictionary<string, Category>();

			foreach (var categoryLink in CategoryLinks.Instance.Get(wiki, date))
			{
				Page fromPage = categoryLink.From;
				if (fromPage == null)
					continue;

				string fromTitle = fromPage.Title;
				if (!categories.ContainsKey(fromTitle))
					categories.Add(fromTitle, new Category(fromTitle));
				string toTitle = categoryLink.ToTitle;
				if (!categories.ContainsKey(toTitle))
					categories.Add(toTitle, new Category(toTitle));
				categories[toTitle].Children.Add(categories[fromTitle]);
			}

			stack = new Stack<Tuple<Category, Queue<Category>>>();
			
			stack.Push(new Tuple<Category,Queue<Category>>(categories[rootCategory], new Queue<Category>(categories[rootCategory].Children)));

			while (stack.Count > 0)
			{
				var currentCategory = stack.Peek().Item1;
				var queue = stack.Peek().Item2;
				if (!queue.Any())
				{
					currentCategory.Closed = true;
					stack.Pop();
				}
				else
				{
					var toAdd = queue.Dequeue();
					if (stack.Any(t => t.Item1 == toAdd))
					{
						ReportCycle(toAdd);
						currentCategory.Children.Remove(toAdd);
					}
					else
					{
						if (!toAdd.Closed)
							stack.Push(new Tuple<Category, Queue<Category>>(toAdd, new Queue<Category>(toAdd.Children)));
					}
				}
			}
		}

	    private static string PlaintextFile;
	    private static string WikitextFile;
	    private static int WikitextCyclesWritten;
	    private static readonly int WikitextCyclesMax = 100;

		static void ReportCycle(Category alreadyInStack)
		{
		    var cycleCats = new[] { alreadyInStack }.Concat(
		        stack.Select(t => t.Item1).TakeWhile(cat => cat != alreadyInStack).Concat(new[] { alreadyInStack })
		        ).Select(cat => cat.Title).Reverse().ToArray();
		    string plaintextCycle = string.Join(" -> ", cycleCats);
			Console.WriteLine(plaintextCycle);
			File.AppendAllText(PlaintextFile, plaintextCycle + Environment.NewLine);
            if (WikitextCyclesWritten < WikitextCyclesMax)
            {
                string wikitextCycle = "* " + string.Join(" → ", cycleCats.Select(c => string.Format("[[:Category:{0}|{0}]]", c.Replace('_', ' '))));
                File.AppendAllText(WikitextFile, wikitextCycle + Environment.NewLine);
                WikitextCyclesWritten++;
            }
		}
	}
}