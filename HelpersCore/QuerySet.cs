using System.Globalization;

namespace HelpersCore;

/// <summary>
/// Represents a readonly ordered set of items that can be parsed from <c>_</c>-separated string and converted to a query string.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
public class QuerySet<T> : QuerySetBase<QuerySet<T>, T>, IQuerySetStatic<QuerySet<T>, T>
	where T : struct, ISpanParsable<T>
{
	/// <summary>
	/// Initializes new empty instance.
	/// </summary>
	public QuerySet() { }

	/// <summary>
	/// Initializes new instance with specified <paramref name="items"/>.
	/// </summary>
	public QuerySet(params IEnumerable<T> items)
		: base(items) { }

	static QuerySet<T> IQuerySetStatic<QuerySet<T>, T>.Create(params IEnumerable<T> items)
		=> new(items);

	static bool IQuerySetStatic<QuerySet<T>, T>.TryParseItem(ReadOnlySpan<char> s, out T result)
		=> T.TryParse(s, CultureInfo.InvariantCulture, out result);

	static T IQuerySetStatic<QuerySet<T>, T>.ParseItem(ReadOnlySpan<char> s)
		=> T.Parse(s, CultureInfo.InvariantCulture);
}