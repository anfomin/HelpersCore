using System.Text;

namespace HelpersCore;

public static partial class Extensions
{
	extension(ReadOnlySpan<char> source)
	{
		/// <summary>
		/// Returns string prefix with specified maximum length.
		/// </summary>
		public ReadOnlySpan<char> LimitLength(int maxLength)
			=> source[..Math.Min(source.Length, maxLength)];
	}

	extension(string source)
	{
		/// <summary>
		/// Returns <c>null</c> for empty string. Otherwise, returns string itself.
		/// </summary>
		public string? NullIfEmpty()
			=> source == string.Empty ? null : source;

		/// <summary>
		/// Returns string prefix with specified maximum length.
		/// </summary>
		public string LimitLength(int maxLength)
			=> source[..Math.Min(source.Length, maxLength)];

		/// <summary>
		/// Splits string by character first index.
		/// </summary>
		public (string First, string? Second) SplitFirst(char value)
		{
			int i = source.IndexOf(value);
			return i == -1 ? (source, null) : (source[..i], source[(i + 1)..]);
		}

		/// <summary>
		/// Splits string by character last index.
		/// </summary>
		public (string First, string? Second) SplitLast(char value)
		{
			int i = source.LastIndexOf(value);
			return i == -1 ? (source, null) : (source[..i], source[(i + 1)..]);
		}

		/// <summary>
		/// Converts string first char to upper case.
		/// </summary>
		public string Capitalize()
			=> string.IsNullOrEmpty(source)
				? source
				: char.ToUpper(source[0]) + source[1..];

		/// <summary>
		/// Converts string first char to lower case.
		/// </summary>
		public string Uncapitalize()
			=> string.IsNullOrEmpty(source) || source.Length <= 3 && source.All(char.IsUpper)
				? source
				: char.ToLower(source[0]) + source[1..];

		/// <summary>
		/// Appends <paramref name="other"/> to <paramref name="source"/> if both are not null or empty.
		/// </summary>
		public string Append(string? other, string separator)
			=> string.IsNullOrEmpty(source) || string.IsNullOrEmpty(other)
				? source
				: source + separator + other;

		/// <summary>
		/// Prepends <paramref name="other"/> before <paramref name="source"/> if both are not null or empty.
		/// Otherwise, returns <paramref name="source"/> string.
		/// </summary>
		public string Prepend(string? other, string separator)
			=> string.IsNullOrEmpty(source) || string.IsNullOrEmpty(other)
				? source
				: other + separator + source;

		/// <summary>
		/// Removes prefix from string if one starts with it.
		/// </summary>
		/// <param name="prefix">Prefix to remove.</param>
		/// <param name="trimWhitespace">Remove whitespaces after prefix.</param>
		public string TrimStart(string prefix, bool trimWhitespace = false)
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
		public string TrimEnd(string suffix, bool trimWhitespace = false)
		{
			if (!source.EndsWith(suffix))
				return source;
			source = source[0..^suffix.Length];
			return trimWhitespace ? source.TrimEnd() : source;
		}

		/// <summary>
		/// Removes all whitespace characters from string.
		/// </summary>
		public string RemoveWhitespace()
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
}