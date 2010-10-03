using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpSqlDumpParser;

namespace WpCategoryCycles
{
	class Program
	{
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
		}
	}
}