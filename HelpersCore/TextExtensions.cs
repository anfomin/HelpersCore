namespace HelpersCore;

/// <summary>
/// Extension methods for text processing.
/// </summary>
public static class TextExtensions
{
	/// <summary>
	/// Provides unit declination for specified <paramref name="value"/>.
	/// </summary>
	/// <param name="singular">Declination for singular value.</param>
	/// <param name="plural">Declination for plural value.</param>
	public static string GetUnit(this int value, string singular, string plural)
		=> Math.Abs(value) == 1 ? singular : plural;

	/// <summary>
	/// Provides unit declination for specified <paramref name="value"/>.
	/// </summary>
	/// <param name="one">Declination for value 1.</param>
	/// <param name="two">Declination for value 2.</param>
	/// <param name="five">Declination for value 5. If <c>null</c> then uses <paramref name="two"/>.</param>
	public static string GetUnit(this int value, string one, string two, string? five)
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
	public static string WithUnit(this int value, string singular, string plural)
		=> value + "\u00a0" + GetUnit(value, singular, plural);

	/// <summary>
	/// Returns <paramref name="value"/> with unit declination for specified <paramref name="value"/>.
	/// </summary>
	/// <param name="one">Declination for value 1.</param>
	/// <param name="two">Declination for value 2.</param>
	/// <param name="five">Declination for value 5. If <c>null</c> then uses <paramref name="two"/>.</param>
	public static string WithUnit(this int value, string one, string two, string? five)
		=> value + "\u00a0" + GetUnit(value, one, two, five);

	/// <summary>
	/// Returns <paramref name="value"/> with unit declination for specified <paramref name="value"/>.
	/// </summary>
	/// <param name="format">Format string for value.</param>
	/// <param name="one">Declination for value 1.</param>
	/// <param name="two">Declination for value 2.</param>
	/// <param name="five">Declination for value 5. If <c>null</c> then uses <paramref name="two"/>.</param>
	public static string WithUnit(this int value, string? format, string one, string two, string? five)
		=> value.ToString(format) + "\u00a0" + GetUnit(value, one, two, five);

	/// <summary>
	/// Returns string like-pattern "%source%".
	/// </summary>
	public static string ToLikePattern(this string source)
		=> $"%{source}%";

	/// <summary>
	/// Splits string by spaces into like-patterns "%word%".
	/// </summary>
	public static string[] SplitToLikePatterns(this string source) => source
		.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		.Select(ToLikePattern)
		.ToArray();

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
}