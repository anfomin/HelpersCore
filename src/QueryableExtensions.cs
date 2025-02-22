using System.Linq.Expressions;

namespace HelpersCore;

public static class QueryableExtensions
{
	/// <summary>
	/// Applies ascending or descending sorting.
	/// </summary>
	/// <param name="descending">True for descending, false for ascending.</param>
	public static IOrderedQueryable<TSource> OrderByDirection<TSource, TKey>(this IQueryable<TSource> source, bool descending, Expression<Func<TSource, TKey>> keySelector)
		=> descending
		? source.OrderByDescending(keySelector)
		: source.OrderBy(keySelector);
}