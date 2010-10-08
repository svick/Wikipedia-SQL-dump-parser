using System;
using System.Collections.Generic;
using System.IO;
using WpSqlDumpParser.IO;

namespace WpSqlDumpParser.EntityCollections
{
	public abstract class Dump<T>
	{
		public abstract string Name { get; }
		public abstract IEnumerable<T> Get(Stream stream);

		public virtual IEnumerable<T> Get(string wiki, DateTime date)
		{
			return Get(DumpDownloader.DownloadDump(wiki, Name, date));
		}

		public Func<IEnumerable<T>, IEnumerable<T>> Limiter { get; set; }
	}

	public abstract class DumpWithId<T> : Dump<T> where T : IObjectWithId
	{
		public virtual Repository<T> CreateRepository(Stream stream)
		{
			return Repository<T>.Create(Get(stream));
		}

		public virtual Repository<T> CreateRepository(string wiki, DateTime date)
		{
			return Repository<T>.Create(Get(wiki, date));
		}
	}
}