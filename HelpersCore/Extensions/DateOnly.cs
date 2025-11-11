namespace HelpersCore;

public static partial class Extensions
{
	extension(DateOnly date)
	{
		/// <summary>
		/// Returns if date is today in UTC.
		/// </summary>
		public bool IsTodayUtc()
			=> date == DateHelper.GetTodayUtc();

		/// <summary>
		/// Returns if date is today or in past in UTC.
		/// </summary>
		public bool IsTodayOrPastUtc()
			=> date <= DateHelper.GetTodayUtc();

		/// <summary>
		/// Returns if date is in past in UTC.
		/// </summary>
		public bool IsPastUtc()
			=> date < DateHelper.GetTodayUtc();

		/// <summary>
		/// Returns if date is today or in future in UTC.
		/// </summary>
		public bool IsTodayOrFutureUtc()
			=> date >= DateHelper.GetTodayUtc();

		/// <summary>
		/// Returns if date is in future in UTC.
		/// </summary>
		public bool IsFutureUtc()
			=> date > DateHelper.GetTodayUtc();

		/// <summary>
		/// Returns if date matches year and month of another date.
		/// </summary>
		public bool IsMatchYearMonth(DateOnly other)
			=> date.Year == other.Year && date.Month == other.Month;

		/// <summary>
		/// Returns number of days in month.
		/// </summary>
		public int GetDaysInMonth()
			=> DateTime.DaysInMonth(date.Year, date.Month);

		/// <summary>
		/// Gets the first day of the month.
		/// </summary>
		public DateOnly GetMonthBegin()
			=> new(date.Year, date.Month, 1);

		/// <summary>
		/// Gets the last day of the month.
		/// </summary>
		public DateOnly GetMonthEnd()
			=> new(date.Year, date.Month, date.GetDaysInMonth());

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
			=> new(date.Year, date.Month, Math.Min(day, date.GetDaysInMonth()));

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
}