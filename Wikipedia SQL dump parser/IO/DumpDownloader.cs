using System;
using System.IO;
using System.IO.Compression;

namespace WpSqlDumpParser.IO
{
	public class DumpDownloader
	{
		public static Stream DownloadDump(string wiki, string dump, DateTime date, string cachePath = null)
		{
			Stream stream = new CachingStream(wiki, dump, date);

			GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress);
			return gzipStream;
		}
	}
}