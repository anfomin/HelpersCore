namespace HelpersCore;

/// <summary>
/// Represents a readonly ordered set of <see cref="Enum"/> that can be parsed from <c>_</c>-separated string and converted to a query string.
/// </summary>
/// <typeparam name="T">Enum type.</typeparam>
public class QueryEnums<T> : QuerySetBase<QueryEnums<T>, T>, IQuerySetStatic<QueryEnums<T>, T>
	where T : struct, Enum
{
	/// <summary>
	/// Initializes new empty instance.
	/// </summary>
	public QueryEnums() { }

	/// <summary>
	/// Initializes new instance with specified <paramref name="items"/>.
	/// </summary>
	public QueryEnums(params IEnumerable<T> items)
		: base(items) { }

	static string IQuerySetStatic<QueryEnums<T>, T>.ConvertItemToString(T item)
		=> Convert.ToInt32(item).ToString();

	static QueryEnums<T> IQuerySetStatic<QueryEnums<T>, T>.Create(params IEnumerable<T> items)
		=> new(items);

	static bool IQuerySetStatic<QueryEnums<T>, T>.TryParseItem(ReadOnlySpan<char> s, out T result)
		=> Enum.TryParse(s, true, out result);

	static T IQuerySetStatic<QueryEnums<T>, T>.ParseItem(ReadOnlySpan<char> s)
		=> Enum.Parse<T>(s, true);
}