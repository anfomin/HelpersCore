using SkiaSharp;

namespace HelpersCore;

public static partial class SKExtensions
{
	extension(SKCodec)
	{
		/// <summary>
		/// Creates <see cref="SKCodec"/> from stream.
		/// </summary>
		/// <param name="disposeStream"><c>True</c> to dispose stream when codec is disposed. Otherwise, <c>false</c>.</param>
		public static SKCodec Create(Stream stream, bool disposeStream, out SKCodecResult result)
			=> SKCodec.Create(WrapManagedStream(stream, disposeStream), out result);

		/// <summary>
		/// Creates <see cref="SKCodec"/> from stream.
		/// </summary>
		/// <param name="disposeStream"><c>True</c> to dispose stream when codec is disposed. Otherwise, <c>false</c>.</param>
		public static SKCodec Create(Stream stream, bool disposeStream)
		{
			var codec = Create(stream, disposeStream, out var result);
			return codec ?? throw new InvalidDataException($"Invalid image stream: {result}");
		}
	}

	extension(SKCodec codec)
	{
		/// <summary>
		/// Decodes image from codec with RGBA8888 or BGRA8888 color type.
		/// </summary>
		public SKBitmap DecodeColored()
		{
			var bitmap = SKBitmap.Decode(codec, new SKImageInfo(codec.Info.Width, codec.Info.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul));
			return bitmap ?? throw new FormatException("Error decoding image");
		}
	}

	static SKStream WrapManagedStream(Stream stream, bool dispose)
		=> stream is null ? throw new ArgumentNullException(nameof(stream))
			: stream.CanSeek ? new SKManagedStream(stream, dispose)
			: new SKFrontBufferedManagedStream(stream, SKCodec.MinBufferedBytesNeeded, dispose);
}