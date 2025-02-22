namespace HelpersCore;

public static class TimeProviderExtensions
{
	/// <summary>
	/// Converts the specified <see cref="DateTime"/> to local <see cref="DateTime"/>.
	/// </summary>
	public static DateTime ToLocal(this TimeProvider timeProvider, DateTime dateTime)
		=> dateTime.Kind switch
		{
			DateTimeKind.Unspecified => throw new InvalidOperationException("Unable to convert unspecified DateTime to local time"),
			DateTimeKind.Local => dateTime,
			_ => DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeProvider.LocalTimeZone), DateTimeKind.Local),
		};

	/// <summary>
	/// Converts the specified <see cref="DateTimeOffset"/> to local <see cref="DateTime"/>.
	/// </summary>
	public static DateTime ToLocal(this TimeProvider timeProvider, DateTimeOffset dateTimeOffset)
	{
		var local = TimeZoneInfo.ConvertTimeFromUtc(dateTimeOffset.UtcDateTime, timeProvider.LocalTimeZone);
		return DateTime.SpecifyKind(local, DateTimeKind.Local);
	}
}