using System;
using System.Collections.Generic;
using System.Linq;

namespace WpSqlDumpParser
{
	public static class OrderedSetOperations
	{
		public static IEnumerable<T> Merge<T>(IEnumerable<IEnumerable<T>> lists, IComparer<T> comparer = null) where T : IComparable<T>
		{
			if (comparer == null)
				comparer = Comparer<T>.Default;

			List<IEnumerator<T>> enumerators = new List<IEnumerator<T>>(lists.Select(list => list.GetEnumerator()));
			foreach (IEnumerator<T> enumerator in enumerators.ToArray())
				if (!enumerator.MoveNext())
					enumerators.Remove(enumerator);
			while (enumerators.Count > 0)
			{
				T current = enumerators.Select(enumerator => enumerator.Current).Min(comparer);
				yield return current;
				foreach (IEnumerator<T> enumerator in enumerators.Where(enumerator => comparer.Compare(enumerator.Current, current) == 0).ToArray())
					if (!enumerator.MoveNext())
						enumerators.Remove(enumerator);
			}
		}

		public static T Min<T>(this IEnumerable<T> source, IComparer<T> comparer)
		{
			bool any = false;
			T min = default(T);

			foreach (T item in source)
			{
				if (!any)
				{
					min = item;
					any = true;
				}
				else
				{
					if (comparer.Compare(item, min) < 0)
						min = item;
				}
			}

			if (!any)
				throw new InvalidOperationException();

			return min;
		}
	}
}
