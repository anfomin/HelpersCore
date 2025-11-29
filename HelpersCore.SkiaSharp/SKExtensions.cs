using System.Drawing;
using SkiaSharp;

namespace HelpersCore;

/// <summary>
/// Provides extension methods for working with SkiaSharp.
/// </summary>
public static partial class SKExtensions
{
	static readonly SKSamplingOptions SamplingUpscale = new(SKCubicResampler.Mitchell);
	static readonly SKSamplingOptions SamplingDownscale = new(SKFilterMode.Linear, SKMipmapMode.Linear);

	/// <summary>
	/// Returns content-type string for specified image format.
	/// </summary>
	public static string ToContentType(this SKEncodedImageFormat format)
		=> $"image/{format.ToString().ToLower()}";

	/// <summary>
	/// Decodes <see cref="SKBitmap"/> from HTTP content.
	/// </summary>
	/// <param name="content">HTTP content to decode.</param>
	public static async Task<SKBitmap> ReadAsImageAsync(this HttpContent content, CancellationToken cancellationToken = default)
	{
		await using var stream = await content.ReadAsStreamAsync(cancellationToken);
		return SKBitmap.DecodeColored(stream);
	}

	extension(SKSamplingOptions)
	{
		/// <summary>
		/// For upscaling it is recommended to use <see cref="SKCubicResampler.Mitchell"/>.
		/// </summary>
		public static SKSamplingOptions Upscale => SamplingUpscale;

		/// <summary>
		/// For downscaling it is recommended to use <see cref="SKFilterMode.Linear"/> and <see cref="SKMipmapMode.Linear"/>.
		/// </summary>
		public static SKSamplingOptions Downscale => SamplingDownscale;
	}

	extension(Size)
	{
		/// <summary>
		/// Tries to parse <see cref="Size"/> from <c>{width}x{height}</c> string.
		/// </summary>
		/// <param name="s">Source string to parse.</param>
		/// <param name="result">Parsed <see cref="Size"/> if successful.</param>
		/// <returns><c>True</c> if parse successful.</returns>
		public static bool TryParse(ReadOnlySpan<char> s, out Size result)
		{
			int index = s.IndexOf('x');
			if (index != -1
				&& int.TryParse(s[..index], out int width)
				&& int.TryParse(s[(index + 1)..], out int height))
			{
				result = new(width, height);
				return true;
			}
			result = default;
			return false;
		}

		/// <summary>
		/// Tries to parse <see cref="Size"/> from <c>{width}x{height}</c> string.
		/// </summary>
		/// <param name="s">Source string to parse.</param>
		/// <param name="result">Parsed <see cref="Size"/> if successful.</param>
		/// <returns><c>True</c> if parse successful.</returns>
		public static bool TryParse(string? s, out Size result)
			=> TryParse(s.AsSpan(), out result);

		/// <summary>
		/// Parses <see cref="Size"/> from <c>{width}x{height}</c> string.
		/// </summary>
		public static Size Parse(ReadOnlySpan<char> s)
			=> TryParse(s, out var result) ? result : throw new FormatException("Input string was not in correct format");

		/// <summary>
		/// Parses <see cref="Size"/> from <c>{width}x{height}</c> string.
		/// </summary>
		public static Size Parse(string s)
			=> Parse(s.AsSpan());
	}
}