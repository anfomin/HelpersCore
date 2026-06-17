using Microsoft.AspNetCore.Http.Extensions;

namespace HelpersCore;

/// <summary>
/// Provides helper methods for URI.
/// </summary>
public static class UriHelperExtensions
{
	extension(UriHelper)
	{
		/// <summary>
		/// Combines <paramref name="baseUri"/> and <paramref name="relativeUri"/> into one URI string.
		/// If <paramref name="baseUri"/> doesn't end with '/', it will be considered as directory and '/' will be added before combining.
		/// </summary>
		/// <param name="baseUri">Base URI, may be absolute or relative.</param>
		/// <param name="relativeUri">Relative URI.</param>
		/// <returns>Combined URI.</returns>
		public static string Combine(string baseUri, string relativeUri)
		{
			if (relativeUri == string.Empty)
				return baseUri;
			return new Uri(baseUri, UriKind.RelativeOrAbsolute)
				.Combine(relativeUri, considerDirectory: true)
				.ToString();
		}
	}
}