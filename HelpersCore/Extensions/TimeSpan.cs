namespace HelpersCore;

public static partial class Extensions
{
	extension(TimeSpan time)
	{
		/// <summary>
		/// Returns elapsed time for logging in formats:
		/// <list type="bullet">
		/// <item>35ms</item>
		/// <item>29s 549ms</item>
		/// <item>2min 11s 307ms</item>
		/// </list>
		/// </summary>
		public string ToLog()
			=> time.TotalSeconds < 1 ? $"{time.Milliseconds}ms"
				: time.TotalMinutes < 1 ? $"{(int)time.TotalSeconds}s {time.Milliseconds}ms"
				: time.Seconds == 0 ? $"{(int)time.TotalMinutes}min {time.Milliseconds}ms"
				: $"{(int)time.TotalMinutes}min {time.Seconds}s {time.Milliseconds}ms";

		/// <summary>
		/// Returns time in format #HH:mm:ss.
		/// </summary>
		/// <param name="roundToSeconds">If <c>true</c> rounds milliseconds to the nearest second.</param>
		public string ToHoursString(bool roundToSeconds = false)
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
		public string ToShortString(bool roundToSeconds = false)
		{
			if (roundToSeconds && time.Milliseconds >= 500)
				time += TimeSpan.FromMilliseconds(1000 - time.Milliseconds);
			return time.TotalHours >= 1
				? $"{(int)time.TotalHours:#00}:{time:mm\\:ss}"
				: time.ToString(@"mm\:ss");
		}
	}
}