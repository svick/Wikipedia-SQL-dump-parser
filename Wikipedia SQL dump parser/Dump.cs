using System;
using System.Collections.Generic;
using System.IO;

namespace WpSqlDumpParser
{
	public abstract class Dump<T>
	{
		public abstract string Name { get; }
		public abstract IEnumerable<T> Get(Stream stream);

		public IEnumerable<T> Get(string wiki, DateTime date)
		{
			return Get(DumpDownloader.DownloadDump(wiki, Name, date));
		}
	}

	public abstract class DumpWithId<T> : Dump<T> where T : IObjectWithId
	{
		public Repository<T> CreateRepository(Stream stream)
		{
			return Repository<T>.Create(Get(stream));
		}

		public Repository<T> CreateRepository(string wiki, DateTime date)
		{
			return Repository<T>.Create(Get(wiki, date));
		}
	}
}