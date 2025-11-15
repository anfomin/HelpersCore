namespace HelpersCore;

public static partial class Extensions
{
	extension(DateTime dateTime)
	{
		/// <summary>
		/// Returns if datetime matches year and month of another datetime.
		/// </summary>
		public bool IsMatchYearMonth(DateTime other)
			=> dateTime.Year == other.Year && dateTime.Month == other.Month;

		/// <summary>
		/// Returns number of days in month.
		/// </summary>
		public int GetDaysInMonth()
			=> DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

		/// <summary>
		/// Truncates the <see cref="DateTime"/> to hours.
		/// </summary>
		public DateTime TruncateToHours()
			=> new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, dateTime.Kind);

		/// <summary>
		/// Truncates the <see cref="DateTime"/> to minutes.
		/// </summary>
		public DateTime TruncateToMinutes()
			=> new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Kind);

		/// <summary>
		/// Truncates the <see cref="DateTime"/> to seconds.
		/// </summary>
		public DateTime TruncateToSeconds()
			=> new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);

		/// <summary>
		/// Truncates the <see cref="DateTime"/> to milliseconds.
		/// </summary>
		public DateTime TruncateToMilliseconds()
			=> new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);

		/// <summary>
		/// Converts <see cref="DateTime"/> to <see cref="DateTimeOffset"/> with the specified <paramref name="offset"/>.
		/// <see cref="DateTime"/> of kind <see cref="DateTimeKind.Unspecified"/> treated as UTC time.
		/// </summary>
		/// <param name="offset">Destination offset.</param>
		public DateTimeOffset ToOffset(TimeSpan offset)
		{
			dateTime = dateTime.Kind switch
			{
				DateTimeKind.Unspecified => dateTime.Add(offset),
				_ => DateTime.SpecifyKind(dateTime.ToUniversalTime().Add(offset), DateTimeKind.Unspecified)
			};
			return new(dateTime, offset);
		}

		/// <summary>
		/// Converts <see cref="DateTime"/> to UTC.
		/// <see cref="DateTime"/> of kind <see cref="DateTimeKind.Unspecified"/> treated as <paramref name="sourceTimeZone"/>.
		/// </summary>
		/// <param name="sourceTimeZone">Source timezone.</param>
		public DateTime ToUniversalTime(TimeZoneInfo sourceTimeZone)
			=> dateTime.Kind switch
			{
				DateTimeKind.Utc => dateTime,
				DateTimeKind.Local => dateTime.ToUniversalTime(),
				_ => TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone),
			};

		/// <summary>
		/// Converts <see cref="DateTime"/> to the specified <paramref name="timeZone"/>.
		/// <see cref="DateTime"/> of kind <see cref="DateTimeKind.Unspecified"/> treated as UTC time.
		/// </summary>
		/// <param name="timeZone">Destination timezone.</param>
		public DateTime ToTimeZone(TimeZoneInfo timeZone)
			=> TimeZoneInfo.ConvertTimeFromUtc(
				dateTime.Kind == DateTimeKind.Local ? dateTime.ToUniversalTime() : dateTime,
				timeZone
			);

		/// <summary>
		/// Converts <see cref="DateTime"/> to the timezone specified by <see cref="TimeProvider"/>.
		/// <see cref="DateTime"/> of kind <see cref="DateTimeKind.Unspecified"/> treated as UTC time.
		/// </summary>
		/// <param name="timeProvider">Time provider that specifies timezone to convert to.</param>
		public DateTime ToTimeZone(TimeProvider timeProvider)
			=> dateTime.ToTimeZone(timeProvider.LocalTimeZone);

		/// <summary>
		/// Converts <see cref="DateTime"/> to the timezone specified by <see cref="ITimeProvider"/>.
		/// <see cref="DateTime"/> of kind <see cref="DateTimeKind.Unspecified"/> treated as UTC time.
		/// </summary>
		/// <param name="timeProvider">Time provider that specifies timezone to convert to.</param>
		public DateTime ToTimeZone(ITimeProvider timeProvider)
			=> dateTime.ToTimeZone(timeProvider.LocalTimeZone);
	}

	extension(DateTimeOffset dateTimeOffset)
	{
		/// <summary>
		/// Converts <see cref="DateTimeOffset"/> to the specified <paramref name="timeZone"/>.
		/// </summary>
		/// <param name="timeZone">Destination timezone.</param>
		public DateTimeOffset ToTimeZone(TimeZoneInfo timeZone)
			=> TimeZoneInfo.ConvertTime(dateTimeOffset, timeZone);

		/// <summary>
		/// Converts <see cref="DateTimeOffset"/> to the timezone specified by <see cref="TimeProvider"/>.
		/// </summary>
		/// <param name="timeProvider">Time provider that specifies timezone to convert to.</param>
		public DateTimeOffset ToTimeZone(TimeProvider timeProvider)
			=> TimeZoneInfo.ConvertTime(dateTimeOffset, timeProvider.LocalTimeZone);

		/// <summary>
		/// Converts <see cref="DateTimeOffset"/> to the timezone specified by <see cref="ITimeProvider"/>.
		/// </summary>
		/// <param name="timeProvider">Time provider that specifies timezone to convert to.</param>
		public DateTimeOffset ToTimeZone(ITimeProvider timeProvider)
			=> TimeZoneInfo.ConvertTime(dateTimeOffset, timeProvider.LocalTimeZone);
	}
}