using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WpSqlDumpParser;

namespace WpCategoryCycles
{
	class Program
	{
		static Stack<Tuple<Category, Queue<Category>>> stack;

		static void Main(string[] args)
		{
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
			Console.WriteLine("Date [{0}]: ", Settings.Default.Date);
			string dateString = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(dateString))
				dateString = Settings.Default.Date;
			if (string.IsNullOrWhiteSpace(dateString))
				dateString = DateTime.Today.ToString("yyyyMMdd");
			Settings.Default.Date = dateString;
			Settings.Default.Save();

			DateTime date = DateTime.ParseExact(dateString, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

			Dictionary<string, Category> categories = new Dictionary<string, Category>();

			foreach (var categoryLink in CategoryLinks.Instance.Get(wiki, date))
			{
				if (categoryLink.From.Namespace == Namespaces.Category)
				{
					string fromTitle = categoryLink.From.Title;
					if (!categories.ContainsKey(fromTitle))
						categories.Add(fromTitle, new Category(fromTitle));
					string toTitle = categoryLink.ToTitle;
					if (!categories.ContainsKey(toTitle))
						categories.Add(toTitle, new Category(toTitle));
					categories[fromTitle].Children.Add(categories[toTitle]);
				}
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

		static readonly string file = "cycles.txt";

		static void ReportCycle(Category alreadyInStack)
		{
			string cycle = String.Join(
				" -> ",
				new[] { alreadyInStack }.Concat(
					stack.Select(t => t.Item1).TakeWhile(cat => cat != alreadyInStack).Concat(new[] { alreadyInStack })
				).Select(cat => cat.Title));
			Console.WriteLine(cycle);
			File.AppendAllText(file, cycle + Environment.NewLine);
		}
	}
}