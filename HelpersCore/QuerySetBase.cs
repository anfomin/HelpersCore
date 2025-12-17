using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace HelpersCore;

/// <summary>
/// Abstract readonly ordered set of items that can be parsed from <c>_</c>-separated string and converted to a query string.
/// </summary>
/// <typeparam name="TSelf">Query set final type.</typeparam>
/// <typeparam name="TItem">Item type.</typeparam>
public abstract class QuerySetBase<TSelf, TItem> : IReadOnlySet<TItem>, ISpanParsable<TSelf>, IEquatable<TSelf>
	where TSelf : class, IReadOnlySet<TItem>, ISpanParsable<TSelf>, IEquatable<TSelf>, IQuerySetStatic<TSelf, TItem>, new()
	where TItem : notnull
{
	const char Separator = '_';
	readonly ImmutableSortedSet<TItem> _items;

	public int Count => _items.Count;

	/// <summary>
	/// Gets if the set is empty.
	/// </summary>
	public bool IsEmpty => _items.IsEmpty;

	/// <summary>
	/// Initializes new empty instance.
	/// </summary>
	protected QuerySetBase()
		=> _items = ImmutableSortedSet<TItem>.Empty;

	/// <summary>
	/// Initializes new instance with specified <paramref name="items"/>.
	/// </summary>
	protected QuerySetBase(params IEnumerable<TItem> items)
		=> _items = items.ToImmutableSortedSet();

	public IEnumerator<TItem> GetEnumerator()
		=> _items.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
		=> _items.GetEnumerator();

	public bool Contains(TItem item)
		=> _items.Contains(item);

	public bool IsProperSubsetOf(IEnumerable<TItem> other)
		=> _items.IsProperSubsetOf(other);

	public bool IsProperSupersetOf(IEnumerable<TItem> other)
		=> _items.IsProperSupersetOf(other);

	public bool IsSubsetOf(IEnumerable<TItem> other)
		=> _items.IsSubsetOf(other);

	public bool IsSupersetOf(IEnumerable<TItem> other)
		=> _items.IsSupersetOf(other);

	public bool Overlaps(IEnumerable<TItem> other)
		=> _items.Overlaps(other);

	public bool SetEquals(IEnumerable<TItem> other)
		=> _items.SetEquals(other);

	public override int GetHashCode()
		=> _items.GetHashCode();

	public override bool Equals(object? obj)
		=> obj is TSelf other && Equals(other);

	public bool Equals(TSelf? other)
		=> other is not null && SetEquals(other);

	/// <summary>
	/// Returns a <c>_</c>-separated string representation of the set or <c>null</c> if the set is empty.
	/// </summary>
	public override string? ToString()
		=> _items.Count == 0 ? null : string.Join(Separator, _items.Select(TSelf.ConvertItemToString));

	/// <summary>
	/// Returns a <c>_</c>-separated string of specified <paramref name="items"/> or <c>null</c> if no items are provided.
	/// </summary>
	public static string? GetString(params IEnumerable<TItem>? items)
		=> items == null || !items.Any() ? null : string.Join(Separator, items.Distinct().Order().Select(TSelf.ConvertItemToString));

	/// <summary>
	/// Tries to parse <c>_</c>-separated query string value into <typeparamref name="TItem"/> items set.
	/// </summary>
	/// <param name="s">Source string to parse.</param>
	/// <param name="result">Parsed set if successful.</param>
	/// <returns><c>True</c> if parse successful.</returns>
	public static bool TryParse(ReadOnlySpan<char> s, [NotNullWhen(true)] out TSelf? result)
	{
		if (s.IsEmpty || s.IsWhiteSpace())
		{
			result = new();
			return true;
		}

		bool has = false;
		List<TItem> items = [];
		foreach (var range in s.Split(Separator))
		{
			var part = s[range];
			if (part.IsEmpty || part.IsWhiteSpace())
				continue;

			has = true;
			if (TSelf.TryParseItem(part, out var item))
				items.Add(item);
		}

		if (has && items.Count == 0)
		{
			result = null;
			return false;
		}
		result = TSelf.Create(items);
		return true;
	}

	/// <summary>
	/// Tries to parse <c>_</c>-separated query string value into <typeparamref name="TItem"/> items set.
	/// </summary>
	/// <param name="s">Source string to parse.</param>
	/// <param name="result">Parsed set if successful.</param>
	/// <returns><c>True</c> if parse successful.</returns>
	public static bool TryParse([NotNullWhen(true)] string? s, [NotNullWhen(true)] out TSelf? result)
		=> TryParse(s.AsSpan(), out result);

	static bool ISpanParsable<TSelf>.TryParse(ReadOnlySpan<char> s, IFormatProvider? formatProvider, [NotNullWhen(true)] out TSelf? result)
		=> TryParse(s, out result);

	static bool IParsable<TSelf>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [NotNullWhen(true)] out TSelf? result)
		=> TryParse(s.AsSpan(), out result);

	/// <summary>
	/// Parses <c>_</c>-separated query string value into <typeparamref name="TItem"/> items set.
	/// </summary>
	/// <param name="s">Source string to parse.</param>
	/// <returns>Parsed set.</returns>
	public static TSelf Parse(ReadOnlySpan<char> s)
	{
		if (s.IsEmpty || s.IsWhiteSpace())
			return new();

		List<TItem> items = [];
		foreach (var range in s.Split(Separator))
		{
			var part = s[range];
			if (part.IsEmpty || part.IsWhiteSpace())
				continue;
			items.Add(TSelf.ParseItem(part));
		}
		return TSelf.Create(items);
	}

	/// <summary>
	/// Parses <c>_</c>-separated query string value into <typeparamref name="TItem"/> items set.
	/// </summary>
	/// <param name="s">Source string to parse.</param>
	/// <returns>Parsed set.</returns>
	public static TSelf Parse(string s)
		=> Parse(s.AsSpan());

	static TSelf ISpanParsable<TSelf>.Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
		=> Parse(s);

	static TSelf IParsable<TSelf>.Parse(string s, IFormatProvider? provider)
		=> Parse(s.AsSpan());
}

/// <summary>
/// Static interface for <see cref="QuerySetBase{TSelf, TItem}"/> to provide item parsing and creation methods.
/// </summary>
/// <typeparam name="TSelf">Set type.</typeparam>
/// <typeparam name="TItem">Item type.</typeparam>
public interface IQuerySetStatic<out TSelf, TItem>
	where TSelf : IQuerySetStatic<TSelf, TItem>
	where TItem : notnull
{
	/// <summary>
	/// Creates new instance of <typeparamref name="TSelf"/> with specified <paramref name="items"/>.
	/// </summary>
	static abstract TSelf Create(params IEnumerable<TItem> items);

	/// <summary>
	/// Tries to parse single <typeparamref name="TItem"/> from string.
	/// </summary>
	/// <param name="s">Source string to parse.</param>
	/// <param name="result">Parsed item if successful.</param>
	/// <returns><c>True</c> if parse successful.</returns>
	static abstract bool TryParseItem(ReadOnlySpan<char> s, out TItem result);

	/// <summary>
	/// Parses single <typeparamref name="TItem"/> from string.
	/// </summary>
	/// <param name="s">Source string to parse.</param>
	/// <returns>Parsed item.</returns>
	static abstract TItem ParseItem(ReadOnlySpan<char> s);

	/// <summary>
	/// Converts specified <paramref name="item"/> to string representation.
	/// </summary>
	static virtual string ConvertItemToString(TItem item)
		=> item.ToString()!;
}