using System.Diagnostics.CodeAnalysis;

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

	readonly StringComparison _comparisonType = comparisonType;

	public int Compare(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
		=> x.Span.CompareTo(y.Span, _comparisonType);

	public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
		=> x.Span.Equals(y.Span, _comparisonType);

	public int GetHashCode([DisallowNull] ReadOnlyMemory<char> obj)
		=> string.GetHashCode(obj.Span, _comparisonType);
}