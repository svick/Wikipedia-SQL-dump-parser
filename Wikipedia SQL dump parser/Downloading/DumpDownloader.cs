using System;
using System.IO;
using System.IO.Compression;

namespace WpSqlDumpParser.Downloading
{
	public class DumpDownloader
	{
		public static Stream DownloadDump(string wiki, string dump, DateTime date)
		{
			string url = string.Format("http://download.wikimedia.org/{0}/{2}/{0}-{2}-{1}.sql.gz", wiki, dump, date.ToString("yyyyMMdd"));
			Console.Error.WriteLine("{0:dd.MM.yyyy hh:mm:ss} Downloading {1}.", DateTime.Now, url);

			Stream stream = new DownloadStream(url);

			GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress);
			return gzipStream;
		}
	}
}