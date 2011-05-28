using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WpSqlDumpParser.Entities;
using WpSqlDumpParser.EntityCollections;
using WpSqlDumpParser.IO;

namespace WpCategoryCycles
{
	class Program
	{
		static Stack<Tuple<Category, Queue<Category>>> stack;

		static void Main(string[] args)
		{
			Console.Write("Cache path [{0}]: ", Settings.Default.CachePath);
			string cachePath = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(cachePath))
				cachePath = Settings.Default.CachePath;
			Settings.Default.CachePath = cachePath;
			CachingStream.CachePath = cachePath;
			Console.Write("Wiki [{0}]: ", Settings.Default.Wiki);
			string wiki = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(wiki))
				wiki = Settings.Default.Wiki;
			Settings.Default.Wiki = wiki;
			Console.Write("Root category [{0}]: ", Settings.Default.RootCategory);
			string rootCategory = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(rootCategory))
				rootCategory = Settings.Default.RootCategory;
			Settings.Default.RootCategory = rootCategory;
			Console.Write("Date [{0}]: ", Settings.Default.Date);
			string dateString = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(dateString))
				dateString = Settings.Default.Date;
			if (string.IsNullOrWhiteSpace(dateString))
				dateString = DateTime.Today.ToString("yyyyMMdd");
			Settings.Default.Date = dateString;
			Settings.Default.Save();

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
                string wikitextCycle = "* " + string.Join(" → ", cycleCats.Select(c => string.Format("[[:Category:{0}|{0}]]", c)));
                File.AppendAllText(WikitextFile, wikitextCycle + Environment.NewLine);
                WikitextCyclesWritten++;
            }
		}
	}
}