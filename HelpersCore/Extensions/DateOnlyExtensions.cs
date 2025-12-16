using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="DateOnly"/>.
/// </summary>
public static class DateOnlyExtensions
{
	extension(DateOnly)
	{
		/// <summary>
		/// Returns the current date in UTC.
		/// </summary>
		public static DateOnly UtcToday
			=> DateOnly.FromDateTime(DateTime.UtcNow);

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
		/// Parses <see cref="DateOnly"/> in <c>yyyy-MM</c> format.
		/// </summary>
		/// <param name="s">Source string to parse.</param>
		/// <returns>Parsed date.</returns>
		public static DateOnly ParseYearMonth(ReadOnlySpan<char> s)
		{
			int index = s.IndexOf('-');
			if (index == -1)
				throw new FormatException("YearMonth string must contain '-' separator.");

			int year = int.Parse(s[..index]);
			int month = int.Parse(s[(index + 1)..]);
			if (year is < 1 or > 9999)
				throw new FormatException("Year must be in range [1,9999].");
			if (month is < 1 or > 12)
				throw new FormatException("Month must be in range [1,12].");
			return new DateOnly(year, month, 1);
		}

		/// <summary>
		/// Parses <see cref="DateOnly"/> in <c>yyyy-MM</c> format.
		/// </summary>
		/// <param name="s">Source string to parse.</param>
		/// <returns>Parsed date.</returns>
		public static DateOnly ParseYearMonth(string? s)
			=> ParseYearMonth(s.AsSpan());

		/// <summary>
		/// Tries to parse <see cref="DateOnly"/> in <c>yyyy-MM</c> format.
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
		/// Tries to parse <see cref="DateOnly"/> in <c>yyyy-MM</c> format.
		/// </summary>
		/// <param name="s">Source string to parse.</param>
		/// <param name="result">Parsed date if successful.</param>
		/// <returns><c>True</c> if parse successful.</returns>
		public static bool TryParseYearMonth([NotNullWhen(true)] string? s, out DateOnly result)
			=> TryParseYearMonth(s.AsSpan(), out result);
	}

	extension(DateOnly date)
	{
		/// <summary>
		/// Returns number of days in month.
		/// </summary>
		public int DaysInMonth
			=> DateTime.DaysInMonth(date.Year, date.Month);

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
		/// Returns if date matches year and month of another date.
		/// </summary>
		public bool IsMatchYearMonth(DateOnly other)
			=> date.Year == other.Year && date.Month == other.Month;

		/// <summary>
		/// Gets the first day of the month.
		/// </summary>
		public DateOnly GetMonthBegin()
			=> new(date.Year, date.Month, 1);

		/// <summary>
		/// Gets the last day of the month.
		/// </summary>
		public DateOnly GetMonthEnd()
			=> new(date.Year, date.Month, date.DaysInMonth);

		/// <summary>
		/// Gets the first day of the year.
		/// </summary>
		public DateOnly GetYearBegin()
			=> new(date.Year, 1, 1);

		/// <summary>
		/// Gets the last day of the year.
		/// </summary>
		public DateOnly GetYearEnd()
			=> new(date.Year, 12, 31);

		/// <summary>
		/// Gets next <paramref name="dayOfWeek"/> after current date.
		/// </summary>
		/// <param name="dayOfWeek">Day of week.</param>
		public DateOnly GetNextDayOfWeek(DayOfWeek dayOfWeek)
			=> date.AddDays((dayOfWeek - date.DayOfWeek + 7) % 7);

		/// <summary>
		/// Returns date with specified day of month.
		/// If day is greater than days in month, returns last day of month.
		/// </summary>
		/// <param name="day">Day of month.</param>
		public DateOnly SetDay(int day)
			=> new(date.Year, date.Month, Math.Min(day, date.DaysInMonth));

		/// <summary>
		/// Converts <see cref="DateOnly"/> and <see cref="TimeOnly"/> from the specified <paramref name="sourceTimeZone"/> to UTC.
		/// </summary>
		/// <param name="time">Time to convert.</param>
		/// <param name="sourceTimeZone">Source timezone.</param>
		public DateTime ToUniversalTime(TimeOnly time, TimeZoneInfo sourceTimeZone)
			=> date.ToDateTime(time, DateTimeKind.Unspecified).ToUniversalTime(sourceTimeZone);

		/// <summary>
		/// Returns date in 'yyyy-MM' format.
		/// </summary>
		public string ToYearMonthString()
			=> date.ToString("yyyy-MM");
	}

	extension(TimeZoneInfo timeZone)
	{
		/// <summary>
		/// Returns the current date in specified timezone.
		/// </summary>
		public DateOnly GetToday()
			=> DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone));
	}

	extension(TimeProvider timeProvider)
	{
		/// <summary>
		/// Returns the current date in specified by <see cref="TimeProvider"/> timezone.
		/// </summary>
		public DateOnly GetToday()
			=> GetToday(timeProvider.LocalTimeZone);
	}

	extension(ITimeProvider timeProvider)
	{
		/// <summary>
		/// Returns the current date in specified by <see cref="ITimeProvider"/> timezone.
		/// </summary>
		public DateOnly GetToday()
			=> GetToday(timeProvider.LocalTimeZone);
	}
}