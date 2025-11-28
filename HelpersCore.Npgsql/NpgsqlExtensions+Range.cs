using NpgsqlTypes;

namespace HelpersCore;

public static partial class NpgsqlExtensions
{
	extension<T>(NpgsqlRange<T>)
	{
		public static NpgsqlRange<T> Infinite => new(
			default, lowerBoundIsInclusive: true, lowerBoundInfinite: true,
			default, upperBoundIsInclusive: true, upperBoundInfinite: true
		);
	}

	extension<T>(NpgsqlRange<T> range) where T : IComparable<T>
	{
		/// <summary>
		/// Determines whether the specified value is contained within the range on .NET client side.
		/// </summary>
		public bool ContainsClient(T value) =>
			!range.IsEmpty
			&& (range.LowerBoundInfinite || range.LowerBound!.CompareTo(value) is int lower && (lower < 0 || lower == 0 && range.LowerBoundIsInclusive))
			&& (range.UpperBoundInfinite || range.UpperBound!.CompareTo(value) is int upper && (upper > 0 || upper == 0 && range.UpperBoundIsInclusive));

		/// <summary>
		/// Returns the intersection of this range with another range on .NET client side.
		/// </summary>
		/// <param name="other">Range to intersect with.</param>
		/// <returns>Intersection of two ranges.</returns>
		public NpgsqlRange<T> IntersectClient(NpgsqlRange<T> other)
		{
			if (range.IsEmpty || other.IsEmpty)
				return NpgsqlRange<T>.Empty;
			int lower = range.LowerBound?.CompareTo(other.LowerBound) ?? 0;
			int upper = range.UpperBound?.CompareTo(other.UpperBound) ?? 0;
			return new(
				lower > 0 ? range.LowerBound : other.LowerBound,
				lower > 0 ? range.LowerBoundIsInclusive
				: lower < 0 ? other.LowerBoundIsInclusive
				: range.LowerBoundIsInclusive && other.LowerBoundIsInclusive,
				range.LowerBoundInfinite || other.LowerBoundInfinite,
				upper < 0 ? range.UpperBound : other.UpperBound,
				upper < 0 ? range.UpperBoundIsInclusive
				: upper > 0 ? other.UpperBoundIsInclusive
				: range.UpperBoundIsInclusive && other.UpperBoundIsInclusive,
				range.UpperBoundInfinite || other.UpperBoundInfinite
			);
		}
	}

	extension<T>(NpgsqlRange<T> range) where T : struct, IComparable<T>
	{
		/// <summary>
		/// Returns a new range with the specified lower bound.
		/// If lower bound is greater than the current upper bound, the upper bound is adjusted accordingly.
		/// </summary>
		/// <param name="lower">New lower bound. If <c>null</c> then infinite.</param>
		/// <param name="inclusive"><c>True</c> if the lower bound is part of the range (i.e. inclusive); otherwise, <c>false</c>.</param>
		public NpgsqlRange<T> WithLower(T? lower, bool inclusive = true)
		{
			if (range.IsEmpty)
				return NpgsqlRange.Create(lower, null, lowerInclusive: inclusive);

			var upper = range.UpperBound;
			bool upperInclusive = range.UpperBoundIsInclusive;
			if (lower is not null && !range.UpperBoundInfinite)
			{
				int compare = lower.Value.CompareTo(upper);
				if (compare > 0 || (compare == 0 && (!inclusive || !upperInclusive)))
				{
					upper = lower.Value;
					upperInclusive = true;
				}
			}
			return new(
				lower ?? default, lowerBoundIsInclusive: inclusive, lowerBoundInfinite: lower is null,
				upper, upperBoundIsInclusive: upperInclusive, upperBoundInfinite: range.UpperBoundInfinite
			);
		}

		/// <summary>
		/// Returns a new range with the specified upper bound.
		/// If upper bound is less than the current lower bound, the lower bound is adjusted accordingly.
		/// </summary>
		/// <param name="upper">New upper bound. If <c>null</c> then infinite.</param>
		/// <param name="inclusive"><c>True</c> if the upper bound is part of the range (i.e. inclusive); otherwise, <c>false</c>.</param>
		public NpgsqlRange<T> WithUpper(T? upper, bool inclusive = true)
		{
			if (range.IsEmpty)
				return NpgsqlRange.Create(null, upper, upperInclusive: inclusive);

			var lower = range.LowerBound;
			bool lowerInclusive = range.LowerBoundIsInclusive;
			if (upper is not null && !range.LowerBoundInfinite)
			{
				int compare = lower.CompareTo(upper.Value);
				if (compare > 0 || (compare == 0 && (!inclusive || !lowerInclusive)))
				{
					lower = upper.Value;
					lowerInclusive = true;
				}
			}
			return new(
				lower, lowerBoundIsInclusive: lowerInclusive, lowerBoundInfinite: range.LowerBoundInfinite,
				upper ?? default, upperBoundIsInclusive: inclusive, upperBoundInfinite: upper is null
			);
		}
	}
}