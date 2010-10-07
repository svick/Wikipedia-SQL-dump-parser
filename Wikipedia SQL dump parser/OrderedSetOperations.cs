using System;
using System.Collections.Generic;
using System.Linq;

namespace WpSqlDumpParser
{
	public static class OrderedSetOperations
	{
		public static IEnumerable<T> Merge<T>(IEnumerable<IEnumerable<T>> lists) where T : IComparable<T>
		{
			List<IEnumerator<T>> enumerators = new List<IEnumerator<T>>(lists.Select(list => list.GetEnumerator()));
			foreach (IEnumerator<T> enumerator in enumerators.ToArray())
				if (!enumerator.MoveNext())
					enumerators.Remove(enumerator);
			while (enumerators.Count > 0)
			{
				T current = enumerators.Select(enumerator => enumerator.Current).Min();
				yield return current;
				foreach (IEnumerator<T> enumerator in enumerators.Where(enumerator => enumerator.Current.CompareTo(current) == 0).ToArray())
					if (!enumerator.MoveNext())
						enumerators.Remove(enumerator);
			}
		}
	}
}
