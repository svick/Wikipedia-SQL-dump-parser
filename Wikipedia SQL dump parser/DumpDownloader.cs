using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using WpSqlDumpParser.Streams;

namespace WpSqlDumpParser
{
	public class DumpDownloader
	{
		public static Stream DownloadDump(string wiki, string dump, DateTime date)
		{
			string url = string.Format("http://download.wikimedia.org/{0}/{2}/{0}-{2}-{1}.sql.gz", wiki, dump, date.ToString("yyyyMMdd"));
			Console.Error.WriteLine("{0:dd.MM.yyyy hh:mm:ss} Downloading {1}.", DateTime.Now, url);

			BlockingCollection<byte[]> queue = new BlockingCollection<byte[]>(50);

			Task.Factory.StartNew(() => Fill(queue, url));

			Stream stream = new StreamFromBlockingCollection(queue);

			GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress);
			return gzipStream;
		}

		public static void Fill(BlockingCollection<byte[]> collection, string url)
		{
			try
			{
				BlockDownloader downloader = new BlockDownloader(url);
				downloader.UserAgent = "[[w:en:User:Svick]] SQL dump parser";
				BlockDownloader.Log = true;

				foreach (var chunk in downloader)
					collection.Add(chunk);
			}
			catch (Exception ex)
			{
				Console.Out.Log(ex.ToString());
			}
			finally
			{
				collection.CompleteAdding();
			}
		}
	}
}