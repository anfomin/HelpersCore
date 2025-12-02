using System.Diagnostics.CodeAnalysis;

namespace HelpersCore;

/// <summary>
/// Represents a sort key and direction in format <c>key-direction</c>.
/// </summary>
/// <param name="Key">Sort key.</param>
/// <param name="Descending">Sort direction.</param>
public readonly record struct DataSort(string Key, bool Descending = false) : IParsable<DataSort>
{
	const char Delimiter = '-';

	public override string ToString()
		=> Descending ? $"{Key}{Delimiter}desc" : Key;

	/// <summary>
	/// Deconstructs the <see cref="DataSort"/> into its components.
	/// </summary>
	/// <param name="key">Sort key.</param>
	/// <param name="descending">Sort direction.</param>
	public void Deconstruct(out string key, out bool descending)
		=> (key, descending) = (Key, Descending);

	/// <summary>
	/// Parses a <see cref="DataSort"/> from a span like <c>key</c> or <c>key-direction</c>.
	/// </summary>
	/// <param name="s">The string to parse.</param>
	public static DataSort Parse(ReadOnlySpan<char> s)
	{
		int index = s.LastIndexOf(Delimiter);
		return index == -1
			? new(s.ToString())
			: new(s[..index].ToString(), s[(index + 1)..].Equals("desc", StringComparison.OrdinalIgnoreCase));
	}

	/// <summary>
	/// Parses a <see cref="DataSort"/> from a string like <c>key</c> or <c>key-direction</c>.
	/// </summary>
	public static DataSort Parse(string s)
		=> Parse(s.AsSpan());

	static DataSort IParsable<DataSort>.Parse(string s, IFormatProvider? provider)
		=> Parse(s.AsSpan());

	/// <summary>
	/// Tries to parse a <see cref="DataSort"/> from a span like <c>key</c> or <c>key-direction</c>.
	/// </summary>
	/// <param name="s">The span to parse.</param>
	/// <param name="result">When this method returns, contains the result of successfully parsing <paramref name="s" /> or an undefined value on failure.</param>
	/// <returns><see langword="true" /> if <paramref name="s" /> was successfully parsed; otherwise, <see langword="false" />.</returns>
	public static bool TryParse(ReadOnlySpan<char> s, out DataSort result)
	{
		if (s.IsEmpty)
		{
			result = default;
			return false;
		}
		result = Parse(s);
		return true;
	}

	/// <summary>
	/// Tries to parse a <see cref="DataSort"/> from a string like <c>key</c> or <c>key-direction</c>.
	/// </summary>
	/// <param name="s">The string to parse.</param>
	/// <param name="result">When this method returns, contains the result of successfully parsing <paramref name="s" /> or an undefined value on failure.</param>
	/// <returns><see langword="true" /> if <paramref name="s" /> was successfully parsed; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? s, out DataSort result)
	{
		if (string.IsNullOrEmpty(s))
		{
			result = default;
			return true;
		}
		result = Parse(s.AsSpan());
		return true;
	}

	static bool IParsable<DataSort>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out DataSort result)
		=> TryParse(s, out result);
}