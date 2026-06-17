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

		/// <summary>
		/// Combines current URI with <paramref name="relativeUri"/>.
		/// </summary>
		/// <param name="relativeUri">Relative URI to combine with.</param>
		/// <param name="considerDirectory">
		/// If <see langword="true"/> and current URI doesn't end with '/',
		/// it will be considered as directory and '/' will be added before combining.
		/// </param>
		/// <returns>Combined URI.</returns>
		public Uri Combine(string? relativeUri, bool considerDirectory = false)
		{
			if (string.IsNullOrEmpty(relativeUri))
				return uri;
			Uri relative = new(relativeUri, UriKind.Relative);
			return uri.Combine(relative, considerDirectory);
		}

		/// <summary>
		/// Combines current URI with <paramref name="relativeUri"/>.
		/// </summary>
		/// <param name="relativeUri">Relative URI to combine with.</param>
		/// <param name="considerDirectory">
		/// If <see langword="true"/> and current URI doesn't end with '/',
		/// it will be considered as directory and '/' will be added before combining.
		/// </param>
		/// <returns>Combined URI.</returns>
		public Uri Combine(Uri relativeUri, bool considerDirectory = false)
		{
			Uri? fakeBase = null;
			if (!uri.IsAbsoluteUri)
			{
				fakeBase = new("http://localhost/");
				uri = new(fakeBase, uri);
			}

			if (considerDirectory && !uri.AbsolutePath.EndsWith('/'))
			{
				var builder = new UriBuilder(uri);
				builder.Path += '/';
				uri = builder.Uri;
			}

			Uri combined = new(uri, relativeUri);
			if (combined.AbsolutePath.EndsWith('/') && !Uri.GetWithoutQueryAndHash(relativeUri.OriginalString).EndsWith('/'))
			{
				var builder = new UriBuilder(combined);
				builder.Path = builder.Path[..^1];
				combined = builder.Uri;
			}
			if (fakeBase != null)
				combined = fakeBase.MakeRelativeUri(combined);
			return combined;
		}
	}

	extension(Uri)
	{
		/// <summary>
		/// Returns URI without query string and hash part.
		/// </summary>
		/// <param name="uri">URI to parse.</param>
		/// <returns>URI without query string and hash part</returns>
		public static ReadOnlySpan<char> GetWithoutQueryAndHash(ReadOnlySpan<char> uri)
		{
			int index = uri.IndexOf('?');
			if (index == -1)
				index = uri.IndexOf('#');
			return index == -1 ? uri : uri[..index];
		}
	}
}