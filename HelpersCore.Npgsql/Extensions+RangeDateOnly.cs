using System.Diagnostics.CodeAnalysis;
using NpgsqlTypes;

namespace HelpersCore;

public static partial class Extensions
{
	extension(NpgsqlRange<DateOnly> range)
	{
		/// <summary>
		/// Converts <see cref="NpgsqlRange&lt;DateOnly&gt;"/> to <see cref="NpgsqlRange&lt;DateTime&gt;"/>
		/// with specified <paramref name="kind"/>.
		/// </summary>
		/// <param name="kind">Date time kind.</param>
		public NpgsqlRange<DateTime> ToDateTime(DateTimeKind kind = DateTimeKind.Utc)
			=> Create(
				range.LowerBoundOrNull?.ToDateTime(TimeOnly.MinValue, kind),
				range.UpperBoundOrNull?.ToDateTime(TimeOnly.MaxValue, kind)
			);

		/// <summary>
		/// Converts <see cref="NpgsqlRange&lt;DateOnly&gt;"/> to <see cref="NpgsqlRange&lt;DateTime&gt;"/>
		/// converting from the specified <paramref name="sourceTimeZone"/> to UTC.
		/// </summary>
		/// <param name="sourceTimeZone">Source timezone.</param>
		public NpgsqlRange<DateTime> ToUniversalTime(TimeZoneInfo sourceTimeZone)
			=> Create(
				range.LowerBoundOrNull?.ToUniversalTime(TimeOnly.MinValue, sourceTimeZone),
				range.UpperBoundOrNull?.ToUniversalTime(TimeOnly.MaxValue, sourceTimeZone)
			);

		/// <summary>
		/// Returns <c>{lower} - {upper}</c> representation of the <see cref="NpgsqlRange&lt;DateOnly&gt;"/>.
		/// If bound is infinite, <c>∞</c> is used.
		/// </summary>
		/// <param name="format">A standard or custom date format string.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		public string ToDashString([StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string? format = null, IFormatProvider? provider = null)
		{
			string begin = range.LowerBoundOrNull?.ToString(format, provider) ?? "∞";
			string end = range.UpperBoundOrNull?.ToString(format, provider) ?? "∞";
			return $"{begin} – {end}";
		}

		/// <summary>
		/// Returns <c>true</c> if both range bounds are finite.
		/// </summary>
		public bool IsFinite
			=> range is { LowerBoundInfinite: false, UpperBoundInfinite: false };

		/// <summary>
		/// Returns <c>true</c> if both range bounds are infinite.
		/// </summary>
		public bool IsInfiniteBoth
			=> range is { LowerBoundInfinite: true, UpperBoundInfinite: true };

		/// <summary>
		/// Returns <c>true</c> if any of range bounds is infinite.
		/// </summary>
		public bool IsInfiniteAny
			=> range.LowerBoundInfinite || range.UpperBoundInfinite;

		/// <summary>
		/// Returns lower <see cref="DateOnly"/> bound or <c>null</c> if lower bound is infinite.
		/// </summary>
		public DateOnly? LowerBoundOrNull
			=> range.LowerBoundInfinite ? null
				: range.LowerBoundIsInclusive ? range.LowerBound
				: range.LowerBound.AddDays(1);

		/// <summary>
		/// Returns upper <see cref="DateOnly"/> bound or <c>null</c> if upper bound is infinite.
		/// </summary>
		public DateOnly? UpperBoundOrNull
			=> range.UpperBoundInfinite ? null
				: range.UpperBoundIsInclusive ? range.UpperBound
				: range.UpperBound.AddDays(-1);

		/// <summary>
		/// Returns range begin and end as a tuple.
		/// </summary>
		/// <exception cref="InvalidOperationException">If any bound is infinite.</exception>
		public (DateOnly Begin, DateOnly End) GetBeginEnd()
		{
			if (range.LowerBoundInfinite || range.UpperBoundInfinite)
				throw new InvalidOperationException("Infinite range");
			return (
				range.LowerBoundIsInclusive ? range.LowerBound : range.LowerBound.AddDays(1),
				range.UpperBoundIsInclusive ? range.UpperBound : range.UpperBound.AddDays(-1)
			);
		}

		/// <summary>
		/// Returns number of days in the range.
		/// </summary>
		/// <exception cref="InvalidOperationException">If any bound is infinite.</exception>
		public int GetDays()
		{
			var (begin, end) = range.GetBeginEnd();
			return (int)(end.AddDays(1).ToDateTime(TimeOnly.MinValue) - begin.ToDateTime(TimeOnly.MinValue)).TotalDays;
		}

		/// <summary>
		/// Enumerate days in the period range.
		/// </summary>
		/// <exception cref="InvalidOperationException">If any bound is infinite.</exception>
		public IEnumerable<DateOnly> EnumerateDays()
		{
			var (begin, end) = range.GetBeginEnd();
			return DateOnly.EnumerateDays(begin, end);
		}

		/// <summary>
		/// Enumerate months in the period range.
		/// </summary>
		/// <param name="wholeMonths"><c>True</c> to return whole months. <c>False</c> to return month in range only.</param>
		public IEnumerable<NpgsqlRange<DateOnly>> EnumerateMonths(bool wholeMonths = false)
		{
			var (begin, end) = range.GetBeginEnd();
			while (begin <= end)
			{
				var monthBegin = wholeMonths ? begin.GetMonthBegin() : begin;
				var monthEnd = wholeMonths ? begin.GetMonthEnd() : DateOnly.Min(begin.GetMonthEnd(), end);
				yield return new(monthBegin, monthEnd);
				begin = monthEnd.AddDays(1);
			}
		}
	}

	/// <summary>
	/// Returns <c>{lower} - {upper}</c> representation of the date range.
	/// </summary>
	/// <param name="format">A standard or custom date format string.</param>
	/// <param name="provider">An object that supplies culture-specific formatting information.</param>
	public static string ToDashString(this (DateOnly Begin, DateOnly End) range,
		[StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string? format = null,
		IFormatProvider? provider = null)
		=> $"{range.Begin.ToString(format, provider)} – {range.End.ToString(format, provider)}";

	/// <summary>
	/// Returns <c>{lower} - {upper}</c> representation of the date range.
	/// If upper bound is <c>null</c>, <c>∞</c> is used.
	/// </summary>
	/// <param name="format">A standard or custom date format string.</param>
	/// <param name="provider">An object that supplies culture-specific formatting information.</param>
	public static string ToDashString(this (DateOnly Begin, DateOnly? End) range,
		[StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string? format = null,
		IFormatProvider? provider = null)
		=> $"{range.Begin.ToString(format, provider)} – {range.End?.ToString(format, provider) ?? "∞"}";
}