using System.Linq.Expressions;

namespace HelpersCore;

public static partial class Extensions
{
	extension<T>(IQueryable<T> source)
	{
		/// <summary>
		/// Sorts the elements of a sequence in ascending or descending order according to a key.
		/// </summary>
		/// <param name="descending"><c>True</c> for descending order, <c>false</c> for ascending order.</param>
		/// <param name="keySelector">A function to extract a key from an element.</param>
		public IOrderedQueryable<T> OrderByDirection<TKey>(bool descending, Expression<Func<T, TKey>> keySelector)
			=> descending
				? source.OrderByDescending(keySelector)
				: source.OrderBy(keySelector);
	}

	extension<T>(IOrderedQueryable<T> source)
	{
		/// <summary>
		/// Performs a subsequent ordering of the elements in a sequence in ascending or descending order according to a key.
		/// </summary>
		/// <param name="descending"><c>True</c> for descending order, <c>false</c> for ascending order.</param>
		/// <param name="keySelector">A function to extract a key from an element.</param>
		public IOrderedQueryable<T> ThenByDirection<TKey>(bool descending, Expression<Func<T, TKey>> keySelector)
			=> descending ? source.ThenByDescending(keySelector) : source.ThenBy(keySelector);
	}
}