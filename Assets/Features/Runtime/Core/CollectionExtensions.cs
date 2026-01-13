using System.Linq;

#nullable enable
namespace Features
{
	public partial class Utils
	{
		public static int FastCount<T>(this IEnumerable<T> values)
			=> values is IReadOnlyCollection<T> collection ? collection.Count : values.Count();

		public static void EnsureCapacity<T>(this List<T> list, int capacity)
			=> list.Capacity = Mathf.Max(list.Capacity, capacity);

		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
		{
			foreach (var item in items)
				collection.Add(item);
		}

		public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
		{
			ThrowHelper.IfNull(action, nameof(action));
			foreach (var item in values) action(item);
		}

		public static void ForEach<T>(this IEnumerable<T> values, Action<T, int> action)
		{
			ThrowHelper.IfNull(action, nameof(action));
			int i = 0;
			foreach (var item in values)
			{
				action(item, i);
				i++;
			}
		}

		public static T Clone<T, TElement>(this T collection)
			where T : ICollection<TElement>, new()
		{
			var result = new T();
			foreach (var item in collection)
			{
				result.Add(item);
			}
			return result;
		}

		public static List<TElement> Clone<TElement>(this List<TElement> elements)
			=> Clone<List<TElement>, TElement>(elements);
	}
}
