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
		/// </summary>
		/// <param name="baseUri">Base URL, may be absolute or relative.</param>
		/// <param name="relativeUri">Relative URI.</param>
		/// <returns>Combined URI.</returns>
		public static string Combine(string baseUri, string relativeUri)
		{
			if (relativeUri == string.Empty)
				return baseUri;
			Uri @base = new(baseUri.EndsWith('/') ? baseUri : $"{baseUri}/", UriKind.RelativeOrAbsolute);
			string result = new Uri(@base, relativeUri).ToString();
			return relativeUri.EndsWith('/') ? result : result.TrimEnd('/');
		}
	}
}