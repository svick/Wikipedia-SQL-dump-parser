using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace WpSqlDumpParser
{
	class DumpDownloader
	{
		public static Stream DownloadDump(string wiki, string dump, DateTime date)
		{
			string url = string.Format("http://download.wikimedia.org/{0}/{2}/{0}-{2}-{1}.sql.gz", wiki, dump, date.ToString("yyyyMMdd"));
			Console.Error.WriteLine("{0:dd.MM.yyyy hh:mm:ss} Downloading {1}.", DateTime.Now, url);

			WebClient wc = new WebClient();
			wc.Headers[HttpRequestHeader.UserAgent] = "[[w:en:User:Svick]] SQL dump parser";
			Stream httpStream = wc.OpenRead(url);
			GZipStream gzipStream = new GZipStream(httpStream, CompressionMode.Decompress);
			return gzipStream;
		}
	}
}