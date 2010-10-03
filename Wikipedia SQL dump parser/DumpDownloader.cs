using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace WpSqlDumpParser
{
	class DumpDownloader
	{
		static readonly TimeSpan initialWaitTime = TimeSpan.FromSeconds(1);

		public static Stream DownloadDump(string wiki, string dump, DateTime date)
		{
			string url = string.Format("http://download.wikimedia.org/{0}/{2}/{0}-{2}-{1}.sql.gz", wiki, dump, date.ToString("yyyyMMdd"));
			Console.Error.WriteLine("{0:dd.MM.yyyy hh:mm:ss} Downloading {1}.", DateTime.Now, url);

			WebClient wc = new WebClient();
			wc.Headers[HttpRequestHeader.UserAgent] = "[[w:en:User:Svick]] SQL dump parser";
			Stream httpStream = null;
			int i = 0;
			while (httpStream == null)
				try
				{
					httpStream = wc.OpenRead(url);
				}
				catch (WebException ex)
				{
					TimeSpan waitTime = TimeSpan.FromSeconds(initialWaitTime.TotalSeconds * Math.Pow(2, i));
					Console.Error.WriteLine(ex.Message);
					Console.Error.WriteLine("Waiting {0} s.", (int)waitTime.TotalSeconds);
					System.Threading.Thread.Sleep(waitTime);
					i++;
				}
			GZipStream gzipStream = new GZipStream(httpStream, CompressionMode.Decompress);
			return gzipStream;
		}
	}
}