using System.Text.RegularExpressions;

namespace HelpersCore;

/// <summary>
/// Provides extensions for text representation.
/// </summary>
public static partial class TextExtensions
{
	extension(int value)
	{
		/// <summary>
		/// Provides unit declination for specified <paramref name="value"/>.
		/// </summary>
		/// <param name="singular">Declination for singular value.</param>
		/// <param name="plural">Declination for plural value.</param>
		public string GetUnit(string singular, string plural)
			=> Math.Abs(value) == 1 ? singular : plural;

		/// <summary>
		/// Provides unit declination for specified <paramref name="value"/>.
		/// </summary>
		/// <param name="one">Declination for value 1.</param>
		/// <param name="two">Declination for value 2.</param>
		/// <param name="five">Declination for value 5. If <c>null</c> then uses <paramref name="two"/>.</param>
		public string GetUnit(string one, string two, string? five)
		{
			value = Math.Abs(value);
			int n = (value % 100 > 20) ? value % 10 : value % 20;
			return n switch
			{
				1 => one,
				2 or 3 or 4 => two,
				_ => five ?? two
			};
		}

		/// <summary>
		/// Returns <paramref name="value"/> with unit declination for specified <paramref name="value"/>.
		/// </summary>
		/// <param name="singular">Declination for singular value.</param>
		/// <param name="plural">Declination for plural value.</param>
		public string WithUnit(string singular, string plural)
			=> value + "\u00a0" + GetUnit(value, singular, plural);

		/// <summary>
		/// Returns <paramref name="value"/> with unit declination for specified <paramref name="value"/>.
		/// </summary>
		/// <param name="one">Declination for value 1.</param>
		/// <param name="two">Declination for value 2.</param>
		/// <param name="five">Declination for value 5. If <c>null</c> then uses <paramref name="two"/>.</param>
		public string WithUnit(string one, string two, string? five)
			=> value + "\u00a0" + GetUnit(value, one, two, five);

		/// <summary>
		/// Returns <paramref name="value"/> with unit declination for specified <paramref name="value"/>.
		/// </summary>
		/// <param name="format">Format string for value.</param>
		/// <param name="one">Declination for value 1.</param>
		/// <param name="two">Declination for value 2.</param>
		/// <param name="five">Declination for value 5. If <c>null</c> then uses <paramref name="two"/>.</param>
		public string WithUnit(string? format, string one, string two, string? five)
			=> value.ToString(format) + "\u00a0" + GetUnit(value, one, two, five);
	}

	extension(string source)
	{
		/// <summary>
		/// Returns string like-pattern <c>%source%</c>.
		/// </summary>
		public string ToLikePattern()
			=> $"%{source}%";

		/// <summary>
		/// Splits string by spaces into like-patterns <c>%word%</c>.
		/// </summary>
		public string[] SplitToLikePatterns() => source
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Select(ToLikePattern)
			.ToArray();

		/// <summary>
		/// If <see cref="String"/> length is more then specified <paramref name="maxLength"/>
		/// then it is split by last matching whitespace and <c>…</c> added.
		/// </summary>
		public string LimitEllipsis(int maxLength)
		{
			if (source.Length <= maxLength)
				return source;

			int i = maxLength;
			var match = NonWhiteSpace.Match(source.Trim());
			while (match.Success && match.Index >= maxLength)
			{
				i = match.Index;
				match = match.NextMatch();
			}
			return $"{source[..i]}…";
		}
	}

	/// <summary>
	/// Returns a string representation of bits per second.
	/// </summary>
	/// <param name="bits">Bits value.</param>
	/// <example>5 b/s, 25 kb/s, 312 Mb/s, 494 Gb/s</example>
	public static string ToBitsPerSecondString(this long bits)
	{
		string unit = "b";
		double v = bits;
		if (v > 1024)
		{
			v /= 1024;
			unit = "kb";
		}
		if (v > 1024)
		{
			v /= 1024;
			unit = "Mb";
		}
		if (v > 1024)
		{
			v /= 1024;
			unit = "Gb";
		}
		return $"{v:0.##}\u00a0{unit}/s";
	}

	[GeneratedRegex(@"\W+")]
	private static partial Regex NonWhiteSpace { get; }
}