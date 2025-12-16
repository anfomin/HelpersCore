using System.Runtime.CompilerServices;

namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="DateTime"/>.
/// </summary>
public static class DateTimeExtensions
{
	extension(DateTime)
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
	}

	extension(DateTime dateTime)
	{
		/// <summary>
		/// Returns number of days in month for current <see cref="DateTime"/>.
		/// </summary>
		public int DaysInMonthCurrent
			=> DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

		/// <summary>
		/// Returns datetime in range of minimum and maximum.
		/// </summary>
		/// <param name="min">Minimum datetime.</param>
		/// <param name="max">Maximum datetime.</param>
		public DateTime Clamp(DateTime min, DateTime max)
			=> dateTime.Ticks < min.Ticks ? min
				: dateTime.Ticks > max.Ticks ? max
				: dateTime;

		/// <summary>
		/// Returns if datetime matches year and month of another datetime.
		/// </summary>
		public bool IsMatchYearMonth(DateTime other)
			=> dateTime.Year == other.Year && dateTime.Month == other.Month;

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