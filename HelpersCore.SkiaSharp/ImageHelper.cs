using System.Drawing;
using System.Text.RegularExpressions;
using SkiaSharp;

namespace HelpersCore;

/// <summary>
/// Provides helper methods for SkiaSharp imaging.
/// </summary>
public static partial class ImageHelper
{
	/// <summary>
	/// For upscaling it is recommended to use <see cref="SKCubicResampler.Mitchell"/>.
	/// </summary>
	public static readonly SKSamplingOptions SamplingUpscale = new(SKCubicResampler.Mitchell);

	/// <summary>
	/// For downscaling it is recommended to use <see cref="SKFilterMode.Linear"/> and <see cref="SKMipmapMode.Linear"/>.
	/// </summary>
	public static readonly SKSamplingOptions SamplingDownscale = new(SKFilterMode.Linear, SKMipmapMode.Linear);

	/// <summary>
	/// Creates <see cref="SKCodec"/> from stream.
	/// </summary>
	/// <param name="disposeStream"><c>True</c> to dispose stream when codec is disposed. Otherwise, <c>false</c>.</param>
	public static SKCodec CreateCodec(Stream stream, bool disposeStream, out SKCodecResult result)
		=> SKCodec.Create(WrapManagedStream(stream, disposeStream), out result);

	/// <summary>
	/// Creates <see cref="SKCodec"/> from stream.
	/// </summary>
	/// <param name="disposeStream"><c>True</c> to dispose stream when codec is disposed. Otherwise, <c>false</c>.</param>
	public static SKCodec CreateCodec(Stream stream, bool disposeStream)
	{
		var codec = CreateCodec(stream, disposeStream, out var result);
		return codec ?? throw new InvalidDataException($"Invalid image stream: {result}");
	}

	/// <summary>
	/// Decodes <see cref="SKBitmap"/> with RGBA8888 or BGRA8888 color type.
	/// </summary>
	/// <param name="stream">Image stream.</param>
	/// <param name="disposeStream"><c>True</c> to dispose stream when codec is disposed. Otherwise, <c>false</c>.</param>
	public static SKBitmap DecodeColored(Stream stream, bool disposeStream = false)
	{
		using var codec = CreateCodec(stream, disposeStream);
		return codec.DecodeColored();
	}

	/// <summary>
	/// Parses <see cref="Size"/> from string <c>{width}x{height}</c>.
	/// </summary>
	public static Size ParseSize(string s)
	{
		if (SizeRegex.Match(s) is not { Success: true } match)
			throw new FormatException($"Can not parse {s} into Size. Supported format is '{{width}}x{{height}}'.");
		return new Size(int.Parse(match.Groups["width"].Value), int.Parse(match.Groups["height"].Value));
	}

	static SKStream WrapManagedStream(Stream stream, bool dispose)
		=> stream is null ? throw new ArgumentNullException(nameof(stream))
		: stream.CanSeek ? new SKManagedStream(stream, dispose)
		: new SKFrontBufferedManagedStream(stream, SKCodec.MinBufferedBytesNeeded, dispose);

	[GeneratedRegex(@"^(?<width>\d+)x(?<height>\d+)$")]
	private static partial Regex SizeRegex { get; }
}