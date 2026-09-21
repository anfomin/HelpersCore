using System.Diagnostics.CodeAnalysis;
using NpgsqlTypes;

namespace HelpersCore;

public static partial class NpgsqlExtensions
{
	extension(NpgsqlRange<DateOnly> range)
	{
		/// <summary>
		/// Converts <see cref="NpgsqlRange&lt;DateOnly&gt;"/> to <see cref="NpgsqlRange&lt;DateTime&gt;"/>
		/// with specified <paramref name="kind"/>.
		/// </summary>
		/// <param name="kind">Date time kind.</param>
		public NpgsqlRange<DateTime> ToDateTime(DateTimeKind kind = DateTimeKind.Utc)
			=> NpgsqlRange.Create(
				range.BeginOrNull?.ToDateTime(TimeOnly.MinValue, kind),
				range.EndOrNull?.ToDateTime(TimeOnly.MaxValue, kind)
			);

		/// <summary>
		/// Converts <see cref="NpgsqlRange&lt;DateOnly&gt;"/> to <see cref="NpgsqlRange&lt;DateTime&gt;"/>
		/// converting from the specified <paramref name="sourceTimeZone"/> to UTC.
		/// </summary>
		/// <param name="sourceTimeZone">Source timezone.</param>
		public NpgsqlRange<DateTime> ToUniversalTime(TimeZoneInfo sourceTimeZone)
			=> NpgsqlRange.Create(
				range.BeginOrNull?.ToUniversalTime(TimeOnly.MinValue, sourceTimeZone),
				range.EndOrNull?.ToUniversalTime(TimeOnly.MaxValue, sourceTimeZone)
			);

		/// <summary>
		/// Returns <c>{lower} - {upper}</c> representation of the <see cref="NpgsqlRange&lt;DateOnly&gt;"/>.
		/// If bound is infinite, <c>∞</c> is used.
		/// </summary>
		/// <param name="format">A standard or custom date format string.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		public string ToDashString([StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string? format = null, IFormatProvider? provider = null)
			=> DateOnly.ToRangeString(range.BeginOrNull, range.EndOrNull, format, provider);

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
		/// Returns always inclusive lower <see cref="DateOnly"/> bound or <see cref="DateOnly.MinValue"/>
		/// if lower bound is infinite or <c>default</c> if range is empty.
		/// </summary>
		public DateOnly Begin
			=> range.IsEmpty ? default
				: range.LowerBoundInfinite ? DateOnly.MinValue
				: range.LowerBoundIsInclusive ? range.LowerBound
				: range.LowerBound.AddDays(1);

		/// <summary>
		/// Returns always inclusive upper <see cref="DateOnly"/> bound or <see cref="DateOnly.MaxValue"/>
		/// if upper bound is infinite or <c>default</c> if range is empty.
		/// </summary>
		public DateOnly End
			=> range.IsEmpty ? default
				: range.UpperBoundInfinite ? DateOnly.MaxValue
				: range.UpperBoundIsInclusive ? range.UpperBound
				: range.UpperBound.AddDays(-1);

		/// <summary>
		/// Returns always inclusive lower <see cref="DateOnly"/> bound or <c>null</c> if range is empty or lower bound is infinite.
		/// </summary>
		public DateOnly? BeginOrNull
			=> range.IsEmpty || range.LowerBoundInfinite ? null
				: range.LowerBoundIsInclusive ? range.LowerBound
				: range.LowerBound.AddDays(1);

		/// <summary>
		/// Returns always inclusive upper <see cref="DateOnly"/> bound or <c>null</c> if range is empty or upper bound is infinite.
		/// </summary>
		public DateOnly? EndOrNull
			=> range.IsEmpty || range.UpperBoundInfinite ? null
				: range.UpperBoundIsInclusive ? range.UpperBound
				: range.UpperBound.AddDays(-1);

		/// <summary>
		/// Returns number of days in the range.
		/// </summary>
		/// <returns>Number of days or <c>zero</c> if range is empty or any bound is infinite.</returns>
		public int? GetDays()
			=> range is { BeginOrNull: { } begin, EndOrNull: { } end }
				? end.DayNumber - begin.DayNumber + 1
				: null;

		/// <summary>
		/// Enumerate days in the period range.
		/// </summary>
		/// <returns><see cref="IEnumerable{DateOnly}"/> for days in range or empty collection if range empty or any bound is infinite.</returns>
		public IEnumerable<DateOnly> EnumerateDays()
			=> range is { BeginOrNull: { } begin, EndOrNull: { } end }
				? DateOnly.EnumerateDays(begin, end)
				: [];

		/// <summary>
		/// Enumerate months in the period range.
		/// </summary>
		/// <param name="wholeMonths"><c>True</c> to return whole months. <c>False</c> to return month in range only.</param>
		public IEnumerable<NpgsqlRange<DateOnly>> EnumerateMonths(bool wholeMonths = false)
		{
			if (range is not { BeginOrNull: { } begin, EndOrNull: { } end })
				yield break;
			while (begin <= end)
			{
				var monthBegin = wholeMonths ? begin.MonthBegin : begin;
				var monthEnd = wholeMonths ? begin.MonthEnd : DateOnly.Min(begin.MonthEnd, end);
				yield return new(monthBegin, monthEnd);
				begin = monthEnd.AddDays(1);
			}
		}
	}
}