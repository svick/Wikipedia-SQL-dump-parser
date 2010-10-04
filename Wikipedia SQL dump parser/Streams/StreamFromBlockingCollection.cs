using System;
using System.Collections.Concurrent;
using System.IO;

namespace WpSqlDumpParser.Streams
{
	public class StreamFromBlockingCollection : Stream
	{
		BlockingCollection<byte[]> collection;
		byte[] current = null;
		int position = 0;
		bool end = false;

		public StreamFromBlockingCollection(BlockingCollection<byte[]> collection)
		{
			this.collection = collection;
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
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (end)
				return 0;
			if (current == null)
			{
				try
				{
					current = collection.Take();
				}
				catch (InvalidOperationException)
				{
					end = true;
					return 0;
				}

				position = 0;
			}

			int toRead = Math.Min(count, current.Length - position);

			Array.Copy(current, position, buffer, offset, toRead);

			position += toRead;

			if (position == current.Length)
				current = null;

			return toRead;
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
	}
}