using System;
using System.IO;
using System.Net;

namespace WpSqlDumpParser.IO
{
	public class DownloadStream : Stream
	{
		public string Uri { get; protected set; }
		public string UserAgent { get; set; }

		public static bool Log { get; set; }
		public static bool Verbose { get; set; }

		int i = 0;
		int position;
		WebResponse response = null;
		Stream responseStream = null;

		public DownloadStream(string uri, int position = 0)
		{
			Uri = uri;
			this.position = position;
			Console.Error.Log(string.Format("Downloading {1}.", uri));
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
		{ }

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
			while (true)
			{
				if (responseStream == null)
				{
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Uri);
					request.AddRange(position);
					request.Timeout = 60000;

					try
					{
						response = request.GetResponse();
					}
					catch (WebException ex)
					{
						if (Log)
							Console.Error.Log(ex.Message);
						continue;
					}

					responseStream = response.GetResponseStream();
				}

				try
				{
					int read = responseStream.Read(buffer, offset, count);
					position += read;

					if (Verbose)
						Console.Error.Log(
							string.Format(
								"Just downloaded {0:f2} kB, total {1:f2} / {2:f2} MB.",
								(float)read / 1024,
								(float)position / 1024 / 1024,
								(float)response.ContentLength / 1024 / 1024
							));
					else if (Log && ++i % 100 == 0)
					{
						Console.Error.Log(
							string.Format(
								"{0:f2} / {1:f2} MB",
								(float)position / 1024 / 1024,
								(float)response.ContentLength / 1024 / 1024
								));
					}

					return read;
				}
				catch (TimeoutException)
				{
					if (Log)
						Console.Error.Log("Timeout encoutered");
					responseStream.Dispose();
					responseStream = null;
				}
			}
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
			if (responseStream != null)
				responseStream.Dispose();
		}
	}
}