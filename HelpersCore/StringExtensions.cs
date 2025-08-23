using System.Text;

namespace HelpersCore;

/// <summary>
/// Extension methods for <see cref="string"/>.
/// </summary>
public static class StringExtensions
{
	/// <summary>
	/// Returns <c>null</c> for empty string. Otherwise, returns string itself.
	/// </summary>
	public static string? NullIfEmpty(this string source)
		=> source == string.Empty ? null : source;

	/// <summary>
	/// Returns string prefix with specified maximum length.
	/// </summary>
	public static string LimitLength(this string source, int maxLength)
		=> source[..Math.Min(source.Length, maxLength)];

	/// <summary>
	/// Returns string prefix with specified maximum length.
	/// </summary>
	public static ReadOnlySpan<char> LimitLength(this ReadOnlySpan<char> source, int maxLength)
		=> source[..Math.Min(source.Length, maxLength)];

	/// <summary>
	/// Splits string by character first index.
	/// </summary>
	public static (string First, string? Second) SplitFirst(this string source, char value)
	{
		int i = source.IndexOf(value);
		return i == -1 ? (source, null) : (source[..i], source[(i+1)..]);
	}

	/// <summary>
	/// Splits string by character last index.
	/// </summary>
	public static (string First, string? Second) SplitLast(this string source, char value)
	{
		int i = source.LastIndexOf(value);
		return i == -1 ? (source, null) : (source[..i], source[(i+1)..]);
	}

	/// <summary>
	/// Converts string first char to upper case.
	/// </summary>
	public static string Capitalize(this string source)
		=> string.IsNullOrEmpty(source)
		? source
		: char.ToUpper(source[0]) + source[1..];

	/// <summary>
	/// Converts string first char to lower case.
	/// </summary>
	public static string Uncapitalize(this string source)
		=> string.IsNullOrEmpty(source) || source.Length <= 3 && source.All(char.IsUpper)
		? source
		: char.ToLower(source[0]) + source[1..];

	/// <summary>
	/// Appends <paramref name="other"/> to <paramref name="source"/> if both are not null or empty.
	/// </summary>
	public static string Append(this string source, string? other, string separator)
		=> string.IsNullOrEmpty(source) || string.IsNullOrEmpty(other)
		? source
		: source + separator + other;

	/// <summary>
	/// Prepends <paramref name="other"/> before <paramref name="source"/> if both are not null or empty.
	/// Otherwise, returns <paramref name="source"/> string.
	/// </summary>
	public static string Prepend(this string source, string? other, string separator)
		=> string.IsNullOrEmpty(source) || string.IsNullOrEmpty(other)
		? source
		: other + separator + source;

	/// <summary>
	/// Removes prefix from string if one starts with it.
	/// </summary>
	/// <param name="prefix">Prefix to remove.</param>
	/// <param name="trimWhitespace">Remove whitespaces after prefix.</param>
	public static string TrimStart(this string source, string prefix, bool trimWhitespace = false)
	{
		if (!source.StartsWith(prefix))
			return source;
		source = source[prefix.Length..];
		return trimWhitespace ? source.TrimStart() : source;
	}

	/// <summary>
	/// Removes suffix from string if one end with it.
	/// </summary>
	/// <param name="suffix">Suffix to remove.</param>
	/// <param name="trimWhitespace">Remove whitespaces before suffix.</param>
	public static string TrimEnd(this string source, string suffix, bool trimWhitespace = false)
	{
		if (!source.EndsWith(suffix))
			return source;
		source = source[0..^suffix.Length];
		return trimWhitespace ? source.TrimEnd() : source;
	}

	/// <summary>
	/// Removes all whitespace characters from string.
	/// </summary>
	public static string RemoveWhitespace(this string source)
	{
		StringBuilder sb = new(source.Length);
		foreach (char c in source)
		{
			if (!char.IsWhiteSpace(c))
				sb.Append(c);
		}
		return sb.ToString();
	}
}