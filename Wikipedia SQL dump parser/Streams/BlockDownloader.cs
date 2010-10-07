using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace WpSqlDumpParser.Streams
{
	class BlockDownloader : IEnumerable<byte[]>
	{
		public string Uri { get; protected set; }
		public string UserAgent { get; set; }
		public int BlockSize { get; set; }

		public static bool Log { get; set; }
		public static bool Verbose { get; set; }

		public BlockDownloader(string uri)
		{
			Uri = uri;
			BlockSize = 1 * 1024 * 1024; //1 MB
		}

		public IEnumerator<byte[]> GetEnumerator()
		{
			int position = 0;
			bool finished = false;

			int i = 0;
			while (!finished)
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Uri);
				request.AddRange(position);
				request.Timeout = 60000;

				WebResponse response;

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

				Stream responseStream = response.GetResponseStream();

				while (!finished)
				{
					byte[] buffer = new byte[BlockSize];

					int read;
					try
					{
						read = responseStream.Read(buffer, 0, buffer.Length);
					}
					catch (TimeoutException)
					{
						if (Log)
							Console.Error.Log("Timeout encoutered");
						break;
					}

					if (read == 0)
					{
						finished = true;
						break;
					}

					position += read;

					if (read < buffer.Length)
					{
						byte[] oldBuffer = buffer;
						buffer = new byte[read];
						Array.Copy(oldBuffer, buffer, read);
					}

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

					yield return buffer;
				}
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}