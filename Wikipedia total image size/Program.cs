using System;

namespace WpTotalImageSize
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
			Console.Write("Date [{0}]: ", Settings.Default.Date);
			string dateString = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(dateString))
				dateString = Settings.Default.Date;
			if (string.IsNullOrWhiteSpace(dateString))
				dateString = DateTime.Today.ToString("yyyyMMdd");
			Settings.Default.Date = dateString;
			Console.Write("Commons date [{0}]: ", Settings.Default.CommonsDate);
			string commonsDateString = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(commonsDateString))
				commonsDateString = Settings.Default.CommonsDate;
			if (string.IsNullOrWhiteSpace(commonsDateString))
				commonsDateString = DateTime.Today.ToString("yyyyMMdd");
			Settings.Default.CommonsDate = commonsDateString;
			Settings.Default.Save();

			DateTime date = DateTime.ParseExact(dateString, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
			DateTime commonsDate = DateTime.ParseExact(commonsDateString, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);

		}
	}
}