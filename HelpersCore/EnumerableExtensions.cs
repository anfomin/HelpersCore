using System.Collections;
using System.Numerics;

namespace HelpersCore;

public static class EnumerableExtensions
{
	/// <summary>
	/// Wraps this object instance into an IEnumerable&lt;T&gt;
	/// consisting of a single non-null item or empty.
	/// </summary>
	/// <typeparam name="T">Type of the object.</typeparam>
	/// <param name="item">The instance that will be wrapped.</param>
	/// <returns>An IEnumerable&lt;T&gt; consisting of a single item or empty.</returns>
	public static IEnumerable<T> YieldIfNotNull<T>(this T? item)
		where T : struct
	{
		if (item.HasValue)
			yield return item.Value;
	}

	/// <summary>
	/// Indicates whether the specified collection is null or has no elements.
	/// </summary>
	public static bool IsNullOrEmpty<T>(this ICollection<T>? source)
		=> source == null || source.Count == 0;

	/// <summary>
	/// Returns first item or null if no items matching <paramref name="predicate"/>.
	/// </summary>
	public static T? FirstOrNull<T>(this IEnumerable<T> source, Func<T, bool>? predicate = null)
		where T : struct
	{
		foreach (var item in source)
		{
			if (predicate == null || predicate(item))
				return item;
		}
		return null;
	}

	/// <summary>
	/// Returns last item or null if no items matching <paramref name="predicate"/>.
	/// </summary>
	public static T? LastOrNull<T>(this IEnumerable<T> source, Func<T, bool>? predicate = null)
		where T : struct
		=> source.Reverse().FirstOrNull(predicate);

	/// <summary>
	/// Returns item if list contains just one item. Otherwise, null.
	/// </summary>
	public static T? OneOrNull<T>(this IEnumerable<T> source)
		where T : class
	{
		bool first = true;
		T? result = null;
		foreach (var item in source)
		{
			if (!first)
				return null;
			result = item;
			first = false;
		}
		return result;
	}

	/// <summary>
	/// Selects non-null elements.
	/// </summary>
	public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source)
		where T : class
		=> (IEnumerable<T>)source.Where(item => item != null);

	/// <summary>
	/// Selects non-null elements.
	/// </summary>
	public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> source)
		where T : struct
		=> source.Where(item => item.HasValue).Select(item => item!.Value);

	/// <summary>
	/// Selects non-null elements.
	/// </summary>
	public static IAsyncEnumerable<T> NotNull<T>(this IAsyncEnumerable<T?> source)
		where T : class
		=> (IAsyncEnumerable<T>)source.Where(item => item != null);

	/// <summary>
	/// Selects non-null elements.
	/// </summary>
	public static IAsyncEnumerable<T> NotNull<T>(this IAsyncEnumerable<T?> source)
		where T : struct
		=> source.Where(item => item.HasValue).Select(item => item!.Value);

	/// <summary>
	/// Determines whether a sequence does not contain elements.
	/// </summary>
	public static bool None<TSource>(this IEnumerable<TSource> source)
		=> !source.Any();

	/// <summary>
	/// Determines whether a sequence does not contain elements that satisfies <paramref name="predicate"/>.
	/// </summary>
	public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
		=> !source.Any(predicate);

	/// <summary>
	/// Returns paired elements from sequence.
	/// If sequence contains less than 2 elements then returns empty sequence.
	/// </summary>
	public static IEnumerable<(T, T)> Pairwise<T>(this IEnumerable<T> source)
	{
		var previous = default(T);
		using var it = source.GetEnumerator();
		if (it.MoveNext())
			previous = it.Current;
		while (it.MoveNext())
			yield return (previous!, previous = it.Current);
	}

	/// <summary>
	/// Determines first item index of the specified type.
	/// </summary>
	/// <returns>Index of the first item of the specified type or -1 if no such element.</returns>
	public static int IndexOf<T>(this IEnumerable source)
	{
		int i = 0;
		foreach (var item in source)
		{
			if (item is T)
				return i;
			i++;
		}
		return -1;
	}

	/// <summary>
	/// Determines item index.
	/// </summary>
	/// <param name="item">Item to search.</param>
	/// <returns>Index of the item or -1 if no such item.</returns>
	public static int IndexOf(this IEnumerable items, object item)
	{
		int i = 0;
		foreach (var itm in items)
		{
			if (itm == item)
				return i;
			i++;
		}
		return -1;
	}

	/// <summary>
	/// Returns hash of all collection items.
	/// </summary>
	public static int GetItemsHash<T>(this IReadOnlyCollection<T> source)
		where T : notnull
	{
		int code = source.Count;
		foreach (var item in source)
			code = unchecked(code * 314159 + item.GetHashCode());
		return code;
	}

	/// <summary>
	/// Removes items that matches <paramref name="predicate"/>.
	/// </summary>
	/// <param name="predicate">Predicate to match items.</param>
	/// <returns>Removed items.</returns>
	public static List<T> RemoveIf<T>(this IList<T> list, Func<T, bool> predicate)
	{
		List<T> res = [];
		for (int i = 0; i < list.Count; i++)
		{
			var item = list[i];
			if (predicate(item))
			{
				list.RemoveAt(i);
				res.Add(item);
				i--;
			}
		}
		return res;
	}

	/// <summary>
	/// Determines whether all elements of a sequence are equal.
	/// If source is empty then returns <c>false</c>.
	/// </summary>
	/// <param name="selector">Method used to extract comparable value.</param>
	/// <param name="value">Same value if method return <c>true</c>.</param>
	public static bool AllSame<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, out TResult value)
	{
		bool has = false;
		TResult first = default!;
		foreach (var (index, item) in source.Index())
		{
			has = true;
			var result = selector(item);
			if (index == 0)
				first = result;
			else if (result is null ? first is not null : !result.Equals(first))
			{
				value = default!;
				return false;
			}
		}
		if (!has)
		{
			value = default!;
			return false;
		}
		value = first;
		return true;
	}

	/// <summary>
	/// Determines whether all elements of a sequence are equal.
	/// If source is empty then returns <c>false</c>.
	/// </summary>
	/// <param name="selector">Method used to extract comparable value.</param>
	public static bool AllSame<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		=> AllSame(source, selector, out var _);

	/// <summary>
	/// Determines whether all elements of a sequence are equal.
	/// If source is empty then returns <c>false</c>.
	/// </summary>
	/// <param name="value">Same value if method return <c>true</c>.</param>
	public static bool AllSame<TSource>(this IEnumerable<TSource> source, out TSource value)
		=> AllSame(source, r => r, out value);

	/// <summary>
	/// Determines whether all elements of a sequence are equal.
	/// If source is empty then returns <c>false</c>.
	/// </summary>
	public static bool AllSame<TSource>(this IEnumerable<TSource> source)
		=> AllSame(source, r => r, out var _);

	/// <summary>
	/// Computes a sum of the nullable values.
	/// If all values are <c>null</c> then returns <c>null</c>.
	/// </summary>
	public static T? SumOrNull<T>(this IEnumerable<T?> source)
		where T : struct, IAdditionOperators<T, T, T>
	{
		bool allNull = true;
		T acc = default;
		foreach (var value in source)
		{
			if (value is { } value2)
			{
				allNull = false;
				acc += value2;
			}
		}
		return allNull ? null : acc;
	}

	/// <summary>
	/// Computes a sum of the sequence of nullable values that are obtained
	/// by invoking a transform function on each element of the input sequence.
	/// If all values are <c>null</c> then returns <c>null</c>.
	/// </summary>
	/// <param name="source">Source element.</param>
	/// <param name="selector">Transform function to get summable value.</param>
	public static TValue? SumOrNull<T, TValue>(this IEnumerable<T> source, Func<T, TValue?> selector)
		where TValue : struct, IAdditionOperators<TValue, TValue, TValue>
	{
		bool allNull = true;
		TValue acc = default;
		foreach (var item in source)
		{
			var value = selector(item);
			if (value is { } value2)
			{
				allNull = false;
				acc += value2;
			}
		}
		return allNull ? null : acc;
	}

	/// <summary>
	/// Splits items into two list depending on <paramref name="predicate"/> result.
	/// </summary>
	public static (List<T> Falsy, List<T> Truthy) Partition<T>(this IEnumerable<T> source, Func<T, bool> predicate)
	{
		var falsy = new List<T>();
		var truthy = new List<T>();
		foreach (var item in source)
			(predicate(item) ? truthy : falsy).Add(item);
		return (falsy, truthy);
	}

	/// <summary>
	/// Joins non-null items with specified separator.
	/// </summary>
	public static string Join<T>(this IEnumerable<T> items, char separator)
		=> string.Join(separator, items
			.Select(item => item?.ToString())
			.Where(item => !string.IsNullOrEmpty(item))
		);

	/// <summary>
	/// Joins non-null items with specified separator.
	/// </summary>
	public static string Join<T>(this IEnumerable<T> items, string separator)
		=> string.Join(separator, items
			.Select(item => item?.ToString())
			.Where(item => !string.IsNullOrEmpty(item))
		);

	/// <summary>
	/// Adds or removes item from the set depending on include flag.
	/// </summary>
	/// <param name="include"><c>True</c> to add item to the set, <c>false</c> to remove item from the set.</param>
	public static bool Toggle<T>(this ISet<T> set, T item, bool include)
		=> include ? set.Add(item) : set.Remove(item);

	/// <summary>
	/// Provides <see cref="IGrouping{TKey,TElement}"/> deconstruction.
	/// </summary>
	public static void Deconstruct<TKey, TElement>(this IGrouping<TKey, TElement> pair, out TKey key, out IEnumerable<TElement> items)
	{
		key = pair.Key;
		items = pair;
	}
}