namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="TimeSpan"/>.
/// </summary>
public static class TimeSpanExtensions
{
	extension(TimeSpan)
	{
		/// <summary>
		/// Tries to parse time in 'mm:ss' format or fallbacks to <see cref="TimeSpan.TryParse(ReadOnlySpan{char}, out TimeSpan)"/> .
		/// </summary>
		/// <param name="s">Source string to parse.</param>
		/// <param name="result">Parsed timespan if successful.</param>
		/// <returns><c>True</c> if parse successful.</returns>
		public static bool TryParseShort(ReadOnlySpan<char> s, out TimeSpan result)
		{
			int index = s.IndexOf(':');
			if (index != -1
				&& int.TryParse(s[..index], out int min)
				&& int.TryParse(s[(index + 1)..], out int sec)
				&& min is >= 0 and < 60
				&& sec is >= 0 and < 60)
			{
				result = new(0, min, sec);
				return true;
			}
			result = default;
			return false;
		}

		/// <summary>
		/// Tries to parse time in 'mm:ss' format or fallbacks to <see cref="TimeSpan.TryParse(string?, out TimeSpan)"/> .
		/// </summary>
		/// <param name="s">Source string to parse.</param>
		/// <param name="result">Parsed timespan if successful.</param>
		/// <returns><c>True</c> if parse successful.</returns>
		public static bool TryParseShort(string? s, out TimeSpan result)
		{
			if (!string.IsNullOrEmpty(s))
				return TryParseShort(s.AsSpan(), out result);
			result = default;
			return false;
		}

		/// <summary>
		/// Parses time in 'mm:ss' format or fallbacks to <see cref="TimeSpan.Parse(string)"/> .
		/// </summary>
		/// <param name="s">Source string to parse.</param>
		/// <results>Parsed timespan.</results>
		public static TimeSpan ParseShort(ReadOnlySpan<char> s)
			=> TryParseShort(s, out var result)
				? result
				: throw new FormatException("Input string was not in correct format");

		/// <summary>
		/// Parses time in 'mm:ss' format or fallbacks to <see cref="TimeSpan.Parse(string)"/> .
		/// </summary>
		/// <param name="s">Source string to parse.</param>
		/// <results>Parsed timespan.</results>
		public static TimeSpan ParseShort(string s)
			=> TryParseShort(s, out var result)
				? result
				: throw new FormatException("Input string was not in correct format");
	}

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