using System.Drawing;
using SkiaSharp;

namespace HelpersCore;

/// <summary>
/// Provides extension methods for working with SkiaSharp.
/// </summary>
public static partial class SKExtensions
{
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
		return ImageHelper.DecodeColored(stream);
	}

	extension(SKCodec codec)
	{
		/// <summary>
		/// Creates <see cref="SKCodec"/> from stream.
		/// </summary>
		/// <param name="disposeStream"><c>True</c> to dispose stream when codec is disposed. Otherwise, <c>false</c>.</param>
		public static SKCodec Create(Stream stream, bool disposeStream, out SKCodecResult result)
			=> ImageHelper.CreateCodec(stream, disposeStream, out result);

		/// <summary>
		/// Creates <see cref="SKCodec"/> from stream.
		/// </summary>
		/// <param name="disposeStream"><c>True</c> to dispose stream when codec is disposed. Otherwise, <c>false</c>.</param>
		public static SKCodec Create(Stream stream, bool disposeStream)
			=> ImageHelper.CreateCodec(stream, disposeStream);

		/// <summary>
		/// Decodes image from codec with RGBA8888 or BGRA8888 color type.
		/// </summary>
		public SKBitmap DecodeColored()
		{
			var bitmap = SKBitmap.Decode(codec, new SKImageInfo(codec.Info.Width, codec.Info.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul));
			return bitmap ?? throw new FormatException("Error decoding image");
		}
	}

	extension(SKSamplingOptions)
	{
		/// <summary>
		/// For upscaling it is recommended to use <see cref="SKCubicResampler.Mitchell"/>.
		/// </summary>
		public static SKSamplingOptions Upscale => ImageHelper.SamplingUpscale;

		/// <summary>
		/// For downscaling it is recommended to use <see cref="SKFilterMode.Linear"/> and <see cref="SKMipmapMode.Linear"/>.
		/// </summary>
		public static SKSamplingOptions Downscale => ImageHelper.SamplingDownscale;
	}

	extension(Size)
	{
		/// <summary>
		/// Parses <see cref="Size"/> from string <c>{width}x{height}</c>.
		/// </summary>
		public static Size Parse(string s)
			=> ImageHelper.ParseSize(s);
	}
}