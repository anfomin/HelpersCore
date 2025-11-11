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
	/// Sampling options for high quality image resizing.
	/// </summary>
	public static readonly SKSamplingOptions SamplingHigh = new(SKCubicResampler.Mitchell);

	/// <summary>
	/// Creates <see cref="SKCodec"/> from stream not taking stream ownership.
	/// </summary>
	public static SKCodec CreateCodec(Stream stream)
	{
		var codec = SKCodec.Create(WrapManagedStream(stream), out var result);
		if (codec == null)
			throw new InvalidDataException($"Invalid image stream: {result}");
		return codec;
	}

	/// <summary>
	/// Decodes image from codec with RGBA8888 or BGRA8888 color type.
	/// </summary>
	public static SKBitmap DecodeColored(Stream stream)
	{
		using var codec = CreateCodec(stream);
		return codec.DecodeColored();
	}

	/// <summary>
	/// Parses size from string "{width}x{height}".
	/// </summary>
	public static Size ParseSize(string s)
	{
		if (SizeRegex.Match(s) is not { Success: true } match)
			throw new FormatException($"Can not parse {s} into Size. Supported format is '{{width}}x{{height}}'.");
		return new Size(int.Parse(match.Groups["width"].Value), int.Parse(match.Groups["height"].Value));
	}

	static SKStream WrapManagedStream(Stream stream)
		=> stream == null ? throw new ArgumentNullException(nameof(stream))
		: stream.CanSeek ? new SKManagedStream(stream, false)
		: new SKFrontBufferedManagedStream(stream, SKCodec.MinBufferedBytesNeeded, false);

	[GeneratedRegex(@"^(?<width>\d+)x(?<height>\d+)$")]
	private static partial Regex SizeRegex { get; }
}