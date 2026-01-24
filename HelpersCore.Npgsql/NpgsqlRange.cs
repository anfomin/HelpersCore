using NpgsqlTypes;

namespace HelpersCore;

/// <summary>
/// Provides helper methods to create <see cref="NpgsqlRange{T}"/>.
/// </summary>
public static class NpgsqlRange
{
	/// <summary>
	/// Creates a new <see cref="NpgsqlRange{T}"/>.
	/// </summary>
	/// <param name="lower">Range lower bound. If <c>null</c> then infinite.</param>
	/// <param name="upper">Range upper bound. If <c>null</c> then infinite.</param>
	/// <param name="lowerInclusive"><c>True</c> if the lower bound is part of the range (i.e. inclusive); otherwise, <c>false</c>.</param>
	/// <param name="upperInclusive"><c>True</c> if the upper bound is part of the range (i.e. inclusive); otherwise, <c>false</c>.</param>
	public static NpgsqlRange<T> Create<T>(T? lower, T? upper, bool lowerInclusive = true, bool upperInclusive = true)
		where T : struct
		=> new(
			lower ?? default, lowerBoundIsInclusive: lowerInclusive, lowerBoundInfinite: lower is null,
			upper ?? default, upperBoundIsInclusive: upperInclusive, upperBoundInfinite: upper is null
		);

	/// <summary>
	/// Creates a new <see cref="NpgsqlRange{T}"/> with infinite upper bound.
	/// </summary>
	/// <param name="lower">Range lower bound.</param>
	/// <param name="lowerInclusive"><c>True</c> if the lower bound is part of the range (i.e. inclusive); otherwise, <c>false</c>.</param>
	public static NpgsqlRange<T> From<T>(T lower, bool lowerInclusive = true)
		where T : struct
		=> new(
			lower, lowerBoundIsInclusive: lowerInclusive, lowerBoundInfinite: false,
			default, upperBoundIsInclusive: true, upperBoundInfinite: true
		);

	/// <summary>
	/// Creates a new <see cref="NpgsqlRange{T}"/> with infinite lower bound.
	/// </summary>
	/// <param name="upper">Range upper bound.</param>
	/// <param name="upperInclusive"><c>True</c> if the upper bound is part of the range (i.e. inclusive); otherwise, <c>false</c>.</param>
	public static NpgsqlRange<T> To<T>(T upper, bool upperInclusive = true)
		where T : struct
		=> new(
			default, lowerBoundIsInclusive: true, lowerBoundInfinite: true,
			upper, upperBoundIsInclusive: upperInclusive, upperBoundInfinite: false
		);
}