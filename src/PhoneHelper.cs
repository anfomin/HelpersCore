using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace HelpersCore;

/// <summary>
/// Provides helper methods for phone numbers.
/// </summary>
public static partial class PhoneHelper
{
	/// <summary>
	/// Removes the phone number formatting.
	/// </summary>
	public static string Clean(string number)
	{
		if (string.IsNullOrEmpty(number))
			return number;

		char[] res = new char[number.Length];
		int chars = 0;
		foreach (char c in number.Where(IsPhoneChar))
		{
			res[chars] = c;
			chars++;
		}
		return new string(res, 0, chars);
	}

	/// <summary>
	/// Tries to match phone number anywhere in string.
	/// </summary>
	public static bool TryGetPhone(string s, [MaybeNullWhen(false)] out string phone)
	{
		if (PhoneRegex.Match(s) is { Success: true } match)
		{
			phone = match.Value;
			return true;
		}
		phone = null;
		return false;
	}

	/// <summary>
	/// Returns if string phone number in some format.
	/// </summary>
	public static bool IsPhoneLike(string s)
		=> PhoneRegex.Match(s) is { Success: true, Index: 0 } match
		&& match.Length == s.Length;

	/// <summary>
	/// Returns if char has meaning in phone number.
	/// </summary>
	public static bool IsPhoneChar(char c)
		=> char.IsDigit(c) || c == '+' || c == '*' || c == '#' || c == ',' || c == ';';

	[GeneratedRegex(@"\+?(\d+\s*( |-|\(|\))\s*)*\d+((,|;)[\d,;#]+)?")]
	private static partial Regex PhoneRegex { get; }
}