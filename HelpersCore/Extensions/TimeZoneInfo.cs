namespace HelpersCore;

public static partial class Extensions
{
	extension(TimeZoneInfo timeZone)
	{
		/// <summary>
		/// Returns the current date in specified timezone.
		/// </summary>
		public DateOnly GetToday()
			=> DateOnly.GetToday(timeZone);
	}

	extension(TimeProvider timeProvider)
	{
		/// <summary>
		/// Returns the current date in specified by <see cref="TimeProvider"/> timezone.
		/// </summary>
		public DateOnly GetToday()
			=> DateOnly.GetToday(timeProvider);
	}

	extension(ITimeProvider timeProvider)
	{
		/// <summary>
		/// Returns the current date in specified by <see cref="ITimeProvider"/> timezone.
		/// </summary>
		public DateOnly GetToday()
			=> DateOnly.GetToday(timeProvider);
	}
}