using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpSqlDumpParser.EntityCollections
{
	public class Repository<T> where T : IObjectWithId
	{
		private static Repository<T> instance = null;

		public static Repository<T> Instance
		{
			get
			{
				return instance;
			}
		}

		public static Repository<T> Create(IEnumerable<T> items)
		{
			Repository<T> result = new Repository<T>();
			foreach (T item in items)
				result.items.Add(item.Id, item);
			instance = result;
			return result;
		}

		private Repository()
		{ }

		Dictionary<int, T> items = new Dictionary<int, T>();

		public T FindById(int id)
		{
			if (items.ContainsKey(id))
				return items[id];
			else
				return default(T);
		}
	}
}