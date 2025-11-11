namespace HelpersCore;

/// <summary>
/// Provides helper methods for <see cref="TimeSpan"/>.
/// </summary>
public static class TimeSpanHelper
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
}