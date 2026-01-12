namespace HelpersCore;

/// <summary>
/// Compares two <see cref="ReadOnlyMemory{Char}"/> instances for equality.
/// </summary>
public class ReadOnlyMemoryCharComparer(StringComparison comparisonType) : IComparer<ReadOnlyMemory<char>>, IEqualityComparer<ReadOnlyMemory<char>>
{
	/// <summary>
	/// Compares two <see cref="ReadOnlyMemory{Char}"/> instances for equality using <see cref="StringComparison.Ordinal"/>.
	/// </summary>
	public static readonly ReadOnlyMemoryCharComparer OrdinalIgnoreCase = new(StringComparison.OrdinalIgnoreCase);

	public int Compare(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
		=> x.Span.CompareTo(y.Span, comparisonType);

	public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
		=> x.Span.Equals(y.Span, comparisonType);

	public int GetHashCode(ReadOnlyMemory<char> obj)
		=> string.GetHashCode(obj.Span, comparisonType);
}