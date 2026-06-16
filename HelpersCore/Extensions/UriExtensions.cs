namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="Uri"/>.
/// </summary>
public static class UriExtensions
{
	extension(Uri uri)
	{
		/// <summary>
		/// Query string value for <c>null</c>.
		/// </summary>
		public static string QueryNullValue => "$null";

		/// <summary>
		/// Query string array values separator character.
		/// </summary>
		public static char QueryArraySeparator => '_';

		/// <summary>
		/// Returns if URI path ends with <paramref name="suffix"/>.
		/// </summary>
		public bool IsPathEndsWith(string suffix)
			=> uri.AbsolutePath.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
	}
}