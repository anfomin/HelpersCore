namespace HelpersCore;

/// <summary>
/// Extension methods for <see cref="DateTime"/> and <see cref="DateOnly"/>.
/// </summary>
public static class DateTimeExtensions
{
	/// <summary>
	/// Returns if date is today in UTC.
	/// </summary>
	public static bool IsTodayUtc(this DateOnly date)
		=> date == DateHelper.GetTodayUtc();

	/// <summary>
	/// Returns if date is today or in past in UTC.
	/// </summary>
	public static bool IsTodayOrPastUtc(this DateOnly date)
		=> date <= DateHelper.GetTodayUtc();

	/// <summary>
	/// Returns if date is in past in UTC.
	/// </summary>
	public static bool IsPastUtc(this DateOnly date)
		=> date < DateHelper.GetTodayUtc();

	/// <summary>
	/// Returns if date is today or in future in UTC.
	/// </summary>
	public static bool IsTodayOrFutureUtc(this DateOnly date)
		=> date >= DateHelper.GetTodayUtc();

	/// <summary>
	/// Returns if date is in future in UTC.
	/// </summary>
	public static bool IsFutureUtc(this DateOnly date)
		=> date > DateHelper.GetTodayUtc();

	/// <summary>
	/// Returns if datetime matches year and month of another datetime.
	/// </summary>
	public static bool IsMatchYearMonth(this DateTime dateTime, DateTime other)
		=> dateTime.Year == other.Year && dateTime.Month == other.Month;

	/// <summary>
	/// Returns if date matches year and month of another date.
	/// </summary>
	public static bool IsMatchYearMonth(this DateOnly date, DateOnly other)
		=> date.Year == other.Year && date.Month == other.Month;

	/// <summary>
	/// Returns number of days in month.
	/// </summary>
	public static int GetDaysInMonth(this DateTime dateTime)
		=> DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

	/// <summary>
	/// Returns number of days in month.
	/// </summary>
	public static int GetDaysInMonth(this DateOnly date)
		=> DateTime.DaysInMonth(date.Year, date.Month);

	/// <summary>
	/// Gets the first day of the month.
	/// </summary>
	public static DateOnly GetMonthBegin(this DateOnly date)
		=> new(date.Year, date.Month, 1);

	/// <summary>
	/// Gets the last day of the month.
	/// </summary>
	public static DateOnly GetMonthEnd(this DateOnly date)
		=> new(date.Year, date.Month, date.GetDaysInMonth());

	/// <summary>
	/// Gets the first day of the year.
	/// </summary>
	public static DateOnly GetYearBegin(this DateOnly date)
		=> new(date.Year, 1, 1);

	/// <summary>
	/// Gets the last day of the year.
	/// </summary>
	public static DateOnly GetYearEnd(this DateOnly date)
		=> new(date.Year, 12, 31);

	/// <summary>
	/// Gets next <paramref name="dayOfWeek"/> after current date.
	/// </summary>
	/// <param name="dayOfWeek">Day of week.</param>
	public static DateOnly GetNextDayOfWeek(this DateOnly date, DayOfWeek dayOfWeek)
		=> date.AddDays((dayOfWeek - date.DayOfWeek + 7) % 7);

	/// <summary>
	/// Returns date with specified day of month.
	/// If day is greater than days in month, returns last day of month.
	/// </summary>
	/// <param name="day">Day of month.</param>
	public static DateOnly SetDay(this DateOnly date, int day)
		=> new(date.Year, date.Month, Math.Min(day, date.GetDaysInMonth()));

	/// <summary>
	/// Truncates the <see cref="DateTime"/> to milliseconds.
	/// </summary>
	public static DateTime TruncateToMilliseconds(this DateTime dateTime)
		=> new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);

	/// <summary>
	/// Truncates the <see cref="DateTime"/> to seconds.
	/// </summary>
	public static DateTime TruncateToSeconds(this DateTime dateTime)
		=> new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);

	/// <summary>
	/// Converts <paramref name="dateTime"/> to <see cref="DateTimeOffset"/> with the specified <paramref name="offset"/>.
	/// <paramref name="dateTime"/> of kind <see cref="DateTimeKind.Unspecified"/> treats as UTC time.
	/// </summary>
	/// <param name="dateTime">Date time to convert.</param>
	/// <param name="offset">Destination offset.</param>
	public static DateTimeOffset ToOffset(this DateTime dateTime, TimeSpan offset)
	{
		dateTime = dateTime.Kind == DateTimeKind.Unspecified
			? dateTime.Add(offset)
			: DateTime.SpecifyKind(dateTime.ToUniversalTime().Add(offset), DateTimeKind.Unspecified);
		return new(dateTime, offset);
	}

	/// <summary>
	/// Converts the specified <see cref="DateTime"/> to local <paramref name="timeProvider"/> offset.
	/// </summary>
	public static DateTime ToLocal(this DateTime dateTime, TimeProvider timeProvider)
		=> dateTime.Kind switch
		{
			DateTimeKind.Unspecified => throw new InvalidOperationException("Unable to convert unspecified DateTime to local time"),
			DateTimeKind.Local => dateTime,
			_ => DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeProvider.LocalTimeZone), DateTimeKind.Local),
		};

	/// <summary>
	/// Converts the specified <see cref="DateTimeOffset"/> to local <paramref name="timeProvider"/> offset.
	/// </summary>
	public static DateTime ToLocal(this DateTimeOffset dateTimeOffset, TimeProvider timeProvider)
	{
		var local = TimeZoneInfo.ConvertTimeFromUtc(dateTimeOffset.UtcDateTime, timeProvider.LocalTimeZone);
		return DateTime.SpecifyKind(local, DateTimeKind.Local);
	}

	/// <summary>
	/// Returns elapsed time for logging in formats:
	/// <list type="bullet">
	/// <item>35ms</item>
	/// <item>29s 549ms</item>
	/// <item>2min 11s 307ms</item>
	/// </list>
	/// </summary>
	public static string ToLog(this TimeSpan time)
		=> time.TotalSeconds < 1 ? $"{time.Milliseconds}ms"
		: time.TotalMinutes < 1 ? $"{(int)time.TotalSeconds}s {time.Milliseconds}ms"
		: time.Seconds == 0 ? $"{(int)time.TotalMinutes}min {time.Milliseconds}ms"
		: $"{(int)time.TotalMinutes}min {time.Seconds}s {time.Milliseconds}ms";

	/// <summary>
	/// Returns time in format #HH:mm:ss.
	/// </summary>
	/// <param name="roundToSeconds">If <c>true</c> rounds milliseconds to the nearest second.</param>
	public static string ToHoursString(this TimeSpan time, bool roundToSeconds = false)
	{
		if (roundToSeconds && time.Milliseconds >= 500)
			time += TimeSpan.FromMilliseconds(1000 - time.Milliseconds);
		return $"{(int)time.TotalHours:#00}:{time:mm\\:ss}";
	}

	/// <summary>
	/// If time is less than 1 hour then returns string in mm:ss format.
	/// Otherwise, returns time in format #HH:mm:ss.
	/// </summary>
	/// <param name="roundToSeconds">If <c>true</c> rounds milliseconds to the nearest second.</param>
	public static string ToShortString(this TimeSpan time, bool roundToSeconds = false)
	{
		if (roundToSeconds && time.Milliseconds >= 500)
			time += TimeSpan.FromMilliseconds(1000 - time.Milliseconds);
		return time.TotalHours >= 1
			? $"{(int)time.TotalHours:#00}:{time:mm\\:ss}"
			: time.ToString(@"mm\:ss");
	}
}