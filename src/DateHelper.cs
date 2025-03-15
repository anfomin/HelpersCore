using System.Text.RegularExpressions;

namespace HelpersCore;

public static partial class DateHelper
{
	/// <summary>
	/// Returns the current date in UTC.
	/// </summary>
	public static DateOnly GetTodayUtc()
		=> DateOnly.FromDateTime(DateTime.UtcNow);

	/// <summary>
	/// Returns the current date in specified timezone.
	/// </summary>
	public static DateOnly GetToday(this TimeZoneInfo timeZone)
		=> DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone));

	/// <summary>
	/// Returns minimum of two datetimes.
	/// </summary>
	public static DateTime Min(DateTime d1, DateTime d2)
		=> d1.Ticks < d2.Ticks ? d1 : d2;

	/// <summary>
	/// Returns minimum of two dates.
	/// </summary>
	public static DateOnly Min(DateOnly d1, DateOnly d2)
		=> d1.DayNumber < d2.DayNumber ? d1 : d2;

	/// <summary>
	/// Returns maximum of two datetimes.
	/// </summary>
	public static DateTime Max(DateTime d1, DateTime d2)
		=> d1.Ticks < d2.Ticks ? d2 : d1;

	/// <summary>
	/// Returns maximum of two dates.
	/// </summary>
	public static DateOnly Max(DateOnly d1, DateOnly d2)
		=> d1.DayNumber < d2.DayNumber ? d2 : d1;

	/// <summary>
	/// Returns datetime in range of minimum and maximum.
	/// </summary>
	/// <param name="min">Minumum datetime.</param>
	/// <param name="max">Maximum datetime.</param>
	public static DateTime Clamp(this DateTime dateTime, DateTime min, DateTime max)
		=> dateTime.Ticks < min.Ticks ? min
		: dateTime.Ticks > max.Ticks ? max
		: dateTime;

	/// <summary>
	/// Returns date in range of minimum and maximum.
	/// </summary>
	/// <param name="min">Minumum date.</param>
	/// <param name="max">Maximum date.</param>
	public static DateOnly Clamp(this DateOnly date, DateOnly min, DateOnly max)
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
		for (var date = begin; date <= end; date = date.AddDays(1))
			yield return date;
	}

	/// <summary>
	/// Returns date in 'yyyy-MM' format.
	/// </summary>
	public static string ToMonthString(this DateOnly date)
		=> date.ToString("yyyy-MM");

	/// <summary>
	/// Parses date in 'yyyy-MM' format.
	/// </summary>
	/// <param name="input">Source string to parse.</param>
	/// <param name="result">Parsed date if successful.</param>
	/// <returns><c>True</c> if parse successful.</returns>
	public static bool TryParseMonth(string? input, out DateOnly result)
	{
		if (!string.IsNullOrEmpty(input))
		{
			var (p1, p2) = input.SplitFirst('-');
			if (int.TryParse(p1, out int year)
				&& int.TryParse(p2, out int month)
				&& year is >= 1 and <= 9999
				&& month is >= 1 and <= 12)
			{
				result = new DateOnly(year, month, 1);
				return true;
			}
		}
		result = DateOnly.MinValue;
		return false;
	}

	/// <summary>
	/// Tries to parse time in 'mm:ss' format or fallbacks to <see cref="TimeSpan.TryParse"/> .
	/// </summary>
	/// <param name="input">Source string to parse.</param>
	/// <param name="result">Parsed timespan if successful.</param>
	/// <returns><c>True</c> if parse successful.</returns>
	public static bool TryParseShort(string? input, out TimeSpan result)
	{
		if (string.IsNullOrEmpty(input))
		{
			result = default;
			return false;
		}

		var match = TimeShortRegex.Match(input);
		if (!match.Success)
			return TimeSpan.TryParse(input, out result);

		int min = int.Parse(match.Groups["min"].Value);
		int sec = int.Parse(match.Groups["sec"].Value);
		if (min > 59 || sec > 59)
		{
			result = default;
			return false;
		}
		result = new(0, min, sec);
		return true;
	}

	/// <summary>
	/// Parses time in 'mm:ss' format or fallbacks to <see cref="TimeSpan.Parse"/> .
	/// </summary>
	/// <param name="input">Source string to parse.</param>
	/// <results>Parsed timespan.</results>
	public static TimeSpan ParseShort(string? input)
		=> TryParseShort(input, out var result) ? result
		: throw new FormatException("Input string was not in corrent format");

	[GeneratedRegex(@"^((?<min>\d{1,2}):)?(?<sec>\d{1,2})$")]
	private static partial Regex TimeShortRegex { get; }
}