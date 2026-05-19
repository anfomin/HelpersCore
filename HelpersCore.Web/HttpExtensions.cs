using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Net.Http.Headers;

namespace HelpersCore;

/// <summary>
/// Provides extension methods for <see cref="HttpContext"/>, <see cref="HttpResponse"/>.
/// </summary>
public static class HttpExtensions
{
	extension(HttpContext context)
	{
		/// <summary>
		/// Returns absolute URL for specified relative URL.
		/// </summary>
		public string GetAbsoluteUrl(string relativeUrl)
			=> $"{context.Request.Scheme}://{context.Request.Host}{relativeUrl}";
	}

	extension(HttpResponse response)
	{
		/// <summary>
		/// Sets response headers to disable caching anywhere.
		/// </summary>
		public void SetNoCache()
			=> response.GetTypedHeaders().SetNoCache();

		/// <summary>
		/// Sets response headers for private caching with specified max age.
		/// </summary>
		/// <param name="maxAge">Specifies the maximum amount of time the response is considered fresh</param>
		public void SetPrivateCache(TimeSpan maxAge)
			=> response.GetTypedHeaders().SetPrivateCache(maxAge);
	}

	extension(ResponseHeaders headers)
	{
		/// <summary>
		/// Sets response headers to disable caching anywhere.
		/// </summary>
		public void SetNoCache()
		{
			headers.CacheControl = new CacheControlHeaderValue
			{
				NoCache = true,
				NoStore = true,
				MustRevalidate = true
			};
			headers.Set(HeaderNames.Expires, 0);
		}

		/// <summary>
		/// Sets response headers for private caching with specified max age.
		/// </summary>
		/// <param name="maxAge">Specifies the maximum amount of time the response is considered fresh</param>
		public void SetPrivateCache(TimeSpan maxAge)
		{
			headers.CacheControl = new CacheControlHeaderValue
			{
				Private = true,
				MaxAge = maxAge
			};
		}
	}
}