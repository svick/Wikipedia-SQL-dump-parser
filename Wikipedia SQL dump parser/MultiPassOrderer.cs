using System;
using System.Collections.Generic;
using System.Linq;

namespace WpSqlDumpParser
{
	public static class MultiPassOrderer
	{
		class KeyComparer<T, K> : IComparer<T> where K : IComparable<K>
		{
			readonly Func<T, K> keySelector;

			public KeyComparer(Func<T, K> keySelector)
			{
				this.keySelector = keySelector;
			}

			public int Compare(T x, T y)
			{
				return keySelector(x).CompareTo(keySelector(y));
			}
		}

		public static int PassSize { get; set; }

		static MultiPassOrderer()
		{
			PassSize = 1000000; // 1,000,000
		}

		public static IEnumerable<T> OrderUnique<T, K>(Func<IEnumerable<T>> collectionFactory, Func<T, K> keySelector) where K : IComparable<K>
		{
			SortedSet<T> items = new SortedSet<T>(new KeyComparer<T, K>(keySelector));

			bool firstPass = true;
			K min = default(K);

			bool finished = false;

			while (!finished)
			{
				items.Clear();

				var collection = collectionFactory();
				if (!firstPass)
					collection = collection.Where(x => keySelector(x).CompareTo(min) > 0);
				var enumerator = collection.GetEnumerator();

				for (int i = 0; i < PassSize; i++)
				{
					if (!enumerator.MoveNext())
					{
						finished = true;
						break;
					}
					items.Add(enumerator.Current);
				}

				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					if (keySelector(current).CompareTo(keySelector(items.Max)) < 0)
						if (items.Add(current))
							items.Remove(items.Max);
				}

				foreach (T item in items)
					yield return item;

				min = keySelector(items.Max);
			}
		}
	}
}