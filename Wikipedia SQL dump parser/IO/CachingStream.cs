using System;
using System.IO;
using System.Linq;

namespace WpSqlDumpParser.IO
{
	public class CachingStream : Stream
	{
	    private static readonly string FinishedFilesName = "finished.txt";

		public static string CachePath { get; set; }

		/// <summary>
		/// 0: not yet started
		/// 1: reading from file
		/// 2: reading from file finished, downloading not yet started
		/// 3: downloading and writing to file
		/// 4: finished
		/// </summary>
		int phase = 0;

		long position = 0;

		FileStream fileReader = null;
		DownloadStream downloadStream = null;
		FileStream fileWriter = null;

        private readonly string fileName;
        private readonly string filePath;
	    private readonly string finishedFilesPath;
	    private readonly string url;

	    public CachingStream(string wiki, string dump, DateTime date)
		{
			fileName = string.Format("{0}-{2}-{1}.sql.gz", wiki, dump, date.ToString("yyyyMMdd"));
            if (CachePath != null)
            {
                filePath = Path.Combine(CachePath, fileName);
                finishedFilesPath = Path.Combine(CachePath, FinishedFilesName);
            }
		    url = string.Format("http://dumps.wikimedia.org/{0}/{1}/{2}", wiki, date.ToString("yyyyMMdd"), fileName);
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void Flush()
		{	}

		public override long Length
		{
			get { throw new NotSupportedException(); }
		}

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int result = 0;
			while (result == 0 && phase < 4)
				switch (phase)
				{
				case 0:
					if (initFileReading())
						phase = 1;
					else
						phase = 2;
					break;
				case 1:
					result = readFile(buffer, offset, count);
					if (result == 0)
					{
						fileReader.Close();
						phase = FinishedDownloading() ? 4 : 2;
					}
					break;
				case 2:
					initDownloading();
					phase = 3;
					break;
				case 3:
					result = download(buffer, offset, count);
					if (result == 0)
					{
						downloadStream.Close();
						fileWriter.Close();
                        SetFinishedDownloading();
						phase = 4;
					}
					break;
				}
			position += result;
			return result;
		}

	    private bool FinishedDownloading()
	    {
            if (!File.Exists(finishedFilesPath))
                return false;

	        return File.ReadLines(finishedFilesPath).Contains(fileName);
	    }

        private void SetFinishedDownloading()
        {
            if (!FinishedDownloading())
                File.AppendAllLines(finishedFilesPath, new[] { fileName });
        }

	    bool initFileReading()
		{
			try
			{
				if (filePath != null)
				{
					fileReader = File.OpenRead(filePath);
					return true;
				}
			}
			catch
			{ }
			return false;
		}

		int readFile(byte[] buffer, int offset, int count)
		{
			return fileReader.Read(buffer, offset, count);
		}

		void initDownloading()
		{
			downloadStream = new DownloadStream(url, position);
			if (filePath != null)
			{
				fileWriter = File.Open(filePath, FileMode.Append);
				fileWriter.Seek(position, SeekOrigin.Begin);
			}
		}

		int download(byte[] buffer, int offset, int count)
		{
			int result = downloadStream.Read(buffer, offset, count);
			if (fileWriter != null)
				fileWriter.Write(buffer, offset, result);
			return result;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (fileReader != null)
				fileReader.Dispose();
			if (downloadStream != null)
				downloadStream.Dispose();
			if (fileWriter != null)
				fileWriter.Dispose();
		}
	}
}