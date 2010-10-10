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
				if (keySelector == null)
					return Comparer<T>.Default.Compare(x, y);
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
			return OrderUnique(collectionFactory, new KeyComparer<T, K>(keySelector));
		}

		public static IEnumerable<T> OrderUnique<T>(Func<IEnumerable<T>> collectionFactory, IComparer<T> comparer)
		{
			SortedSet<T> items = new SortedSet<T>(comparer);

			bool firstPass = true;
			T min = default(T);

			bool finished = false;

			while (!finished)
			{
				items.Clear();

				var collection = collectionFactory();
				if (!firstPass)
					collection = collection.Where(x => comparer.Compare(x, min) > 0);
				var enumerator = collection.GetEnumerator();

				while (items.Count < PassSize)
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
					if (comparer.Compare(current, items.Max) < 0)
						if (items.Add(current))
							items.Remove(items.Max);
				}

				foreach (T item in items)
					yield return item;

				min = items.Max;
				firstPass = false;
			}
		}
	}
}