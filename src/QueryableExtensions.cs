using System.Linq.Expressions;

namespace HelpersCore;

public static class QueryableExtensions
{
	/// <summary>
	/// Sorts the elements of a sequence in ascending or descending order according to a key.
	/// </summary>
	/// <param name="descending"><c>True</c> for descending order, <c>false</c> for ascending order.</param>
	/// <param name="keySelector">A function to extract a key from an element.</param>
	public static IOrderedQueryable<TSource> OrderByDirection<TSource, TKey>(this IQueryable<TSource> source, bool descending, Expression<Func<TSource, TKey>> keySelector)
		=> descending
		? source.OrderByDescending(keySelector)
		: source.OrderBy(keySelector);

	/// <summary>
	/// Performs a subsequent ordering of the elements in a sequence in ascending or descending order according to a key.
	/// </summary>
	/// <param name="descending"><c>True</c> for descending order, <c>false</c> for ascending order.</param>
	/// <param name="keySelector">A function to extract a key from an element.</param>
	public static IOrderedQueryable<TSource> ThenByDirection<TSource, TKey>(this IOrderedQueryable<TSource> source, bool descending, Expression<Func<TSource, TKey>> keySelector)
		=> descending ? source.ThenByDescending(keySelector) : source.ThenBy(keySelector);
}