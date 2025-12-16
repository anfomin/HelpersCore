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
}