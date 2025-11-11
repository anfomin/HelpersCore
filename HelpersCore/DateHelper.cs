using System.Runtime.CompilerServices;

namespace HelpersCore;

/// <summary>
/// Provides helper methods for <see cref="DateTime"/> and <see cref="DateOnly"/>.
/// </summary>
public static class DateHelper
{
	extension(DateOnly date)
	{
		/// <summary>
		/// Returns the current date in UTC.
		/// </summary>
		public static DateOnly GetTodayUtc()
			=> DateOnly.FromDateTime(DateTime.UtcNow);

		/// <summary>
		/// Returns the current date in specified timezone.
		/// </summary>
		public static DateOnly GetToday(TimeZoneInfo timeZone)
			=> DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone));

		/// <summary>
		/// Returns the current date in specified time provider timezone.
		/// </summary>
		public static DateOnly GetToday(TimeProvider timeProvider)
			=> GetToday(timeProvider.LocalTimeZone);

		/// <summary>
		/// Gets last day of the month.
		/// </summary>
		/// <param name="year">Date year.</param>
		/// <param name="month">Date month.</param>
		public static DateOnly GetMonthEnd(int year, int month)
			=> new(year, month, DateTime.DaysInMonth(year, month));

		/// <summary>
		/// Returns minimum of two dates.
		/// </summary>
		public static DateOnly Min(DateOnly d1, DateOnly d2)
			=> d1.DayNumber < d2.DayNumber ? d1 : d2;

		/// <summary>
		/// Returns minimum of dates.
		/// </summary>
		/// <param name="dates">Dates to get minimum of. Must contain at least one date.</param>
		[OverloadResolutionPriority(-1)]
		public static DateOnly Min(params IReadOnlyCollection<DateOnly> dates)
			=> dates.Aggregate(Min);

		/// <summary>
		/// Returns maximum of two dates.
		/// </summary>
		public static DateOnly Max(DateOnly d1, DateOnly d2)
			=> d1.DayNumber < d2.DayNumber ? d2 : d1;

		/// <summary>
		/// Returns maximum of dates.
		/// </summary>
		/// <param name="dates">Dates to get maximum of. Must contain at least one date.</param>
		[OverloadResolutionPriority(-1)]
		public static DateOnly Max(params IReadOnlyCollection<DateOnly> dates)
			=> dates.Aggregate(Max);

		/// <summary>
		/// Returns date in range of minimum and maximum.
		/// </summary>
		/// <param name="min">Minimum date.</param>
		/// <param name="max">Maximum date.</param>
		public DateOnly Clamp(DateOnly min, DateOnly max)
			=> date.DayNumber < min.DayNumber ? min
				: date.DayNumber > max.DayNumber ? max
				: date;

		/// <summary>
		/// Enumerates days between two dates.
		/// </summary>
		/// <param name="begin">Starting date.</param>
		/// <param name="end">Inclusive end date.</param>
		public static IEnumerable<DateOnly> EnumerateDays(DateOnly begin, DateOnly end)
		{
			for (var d = begin; d <= end; d = d.AddDays(1))
				yield return d;
		}

		/// <summary>
		/// Enumerates months between two dates.
		/// Yields first day of each month.
		/// </summary>
		/// <param name="begin">Starting date.</param>
		/// <param name="end">Inclusive end date.</param>
		public static IEnumerable<DateOnly> EnumerateMonths(DateOnly begin, DateOnly end)
		{
			for (var d = begin.GetMonthBegin(); d <= end; d = d.AddMonths(1))
				yield return d;
		}

		/// <summary>
		/// Parses date in 'yyyy-MM' format.
		/// </summary>
		/// <param name="s">Source string to parse.</param>
		/// <param name="result">Parsed date if successful.</param>
		/// <returns><c>True</c> if parse successful.</returns>
		public static bool TryParseYearMonth(ReadOnlySpan<char> s, out DateOnly result)
		{
			int index = s.IndexOf('-');
			if (index != -1
				&& int.TryParse(s[..index], out int year)
				&& int.TryParse(s[(index + 1)..], out int month)
				&& year is >= 1 and <= 9999
				&& month is >= 1 and <= 12)
			{
				result = new(year, month, 1);
				return true;
			}
			result = default;
			return false;
		}

		/// <summary>
		/// Parses date in 'yyyy-MM' format.
		/// </summary>
		/// <param name="s">Source string to parse.</param>
		/// <param name="result">Parsed date if successful.</param>
		/// <returns><c>True</c> if parse successful.</returns>
		public static bool TryParseYearMonth(string? s, out DateOnly result)
		{
			if (!string.IsNullOrEmpty(s))
				return TryParseYearMonth(s.AsSpan(), out result);
			result = default;
			return false;
		}
	}

	extension(DateTime dateTime)
	{
		/// <summary>
		/// Returns minimum of two datetimes.
		/// </summary>
		public static DateTime Min(DateTime d1, DateTime d2)
			=> d1.Ticks < d2.Ticks ? d1 : d2;

		/// <summary>
		/// Returns minimum of datetimes.
		/// </summary>
		/// <param name="dateTimes">Datetimes to get minimum of. Must contain at least one datetime.</param>
		[OverloadResolutionPriority(-1)]
		public static DateTime Min(params IReadOnlyCollection<DateTime> dateTimes)
			=> dateTimes.Aggregate(Min);

		/// <summary>
		/// Returns maximum of two datetimes.
		/// </summary>
		public static DateTime Max(DateTime d1, DateTime d2)
			=> d1.Ticks < d2.Ticks ? d2 : d1;

		/// <summary>
		/// Returns maximum of datetimes.
		/// </summary>
		/// <param name="dateTimes">Datetimes to get maximum of. Must contain at least one datetime.</param>
		[OverloadResolutionPriority(-1)]
		public static DateTime Max(params IReadOnlyCollection<DateTime> dateTimes)
			=> dateTimes.Aggregate(Max);

		/// <summary>
		/// Returns datetime in range of minimum and maximum.
		/// </summary>
		/// <param name="min">Minimum datetime.</param>
		/// <param name="max">Maximum datetime.</param>
		public DateTime Clamp(DateTime min, DateTime max)
			=> dateTime.Ticks < min.Ticks ? min
				: dateTime.Ticks > max.Ticks ? max
				: dateTime;
	}
}