using System.Drawing;
using SkiaSharp;

namespace HelpersCore;

public static partial class SKExtensions
{
	extension(SKBitmap bitmap)
	{
		/// <summary>
		/// Decodes <see cref="SKBitmap"/> with RGBA8888 or BGRA8888 color type.
		/// </summary>
		/// <param name="stream">Image stream.</param>
		/// <param name="disposeStream"><c>True</c> to dispose stream when codec is disposed. Otherwise, <c>false</c>.</param>
		public static SKBitmap DecodeColored(Stream stream, bool disposeStream = false)
			=> ImageHelper.DecodeColored(stream, disposeStream);

		/// <summary>
		/// Encodes bitmap into specified format.
		/// </summary>
		/// <param name="format">Encode format.</param>
		/// <param name="quality">encode quality.</param>
		public SKData Encode(SKEncodedImageFormat format, int quality = 80)
		{
			using var image = SKImage.FromBitmap(bitmap);
			return image.Encode(format, quality);
		}

		/// <summary>
		/// Encodes bitmap into specified format. When target format is JPEG then uses white background color.
		/// </summary>
		/// <param name="format">Encode format.</param>
		/// <param name="quality">encode quality.</param>
		public SKData EncodeDefaultWhite(SKEncodedImageFormat format, int quality = 80)
		{
			// for jpeg we need to render image on white background
			if (format == SKEncodedImageFormat.Jpeg)
			{
				using var temp = new SKBitmap(bitmap.Width, bitmap.Height, true);
				using var canvas = new SKCanvas(temp);
				canvas.Clear(SKColors.White);
				canvas.DrawBitmap(bitmap, 0, 0);
				return temp.Encode(format, quality);
			}
			return bitmap.Encode(format, quality);
		}

		/// <summary>
		/// Returns if bitmap has any transparent pixel.
		/// </summary>
		public bool HasAlpha()
			=> bitmap.Pixels.Any(pixel => pixel.Alpha != byte.MaxValue);

		/// <summary>
		/// Resizes <see cref="SKBitmap"/> using the specified resize method.
		/// </summary>
		/// <param name="size">Desired image size.</param>
		/// <param name="sampling">Resize sampling.</param>
		public SKBitmap Resize(Size size, SKSamplingOptions sampling)
		{
			SKBitmap resized;
			SKImageInfo resizeInfo = new(size.Width, size.Height);
			if (bitmap.ColorType != SKImageInfo.PlatformColorType)
			{
				// SkiaSharp sometimes does not support resizing non-platform color types
				using var temp = bitmap.Copy(SKImageInfo.PlatformColorType);
				resized = temp.Resize(resizeInfo, sampling);
			}
			else
				resized = bitmap.Resize(resizeInfo, sampling);
			return resized ?? throw new InvalidOperationException("Can not resize image");
		}

		/// <summary>
		/// Creates downscaled <see cref="SKBitmap"/> saving original proportions.
		/// Returns null if source bitmap size is the same as resized one.
		/// </summary>
		/// <param name="maxSize">Desired maximum width and height.</param>
		/// <param name="mode">Resize mode.</param>
		/// <param name="sampling">Resize sampling.</param>
		public SKBitmap? Downscale(Size maxSize, ResizeMode mode, SKSamplingOptions sampling)
		{
			var sourceSize = new Size(bitmap.Width, bitmap.Height);
			var resultSize = maxSize.Downscale(bitmap.Width, bitmap.Height, mode);
			if (sourceSize == resultSize)
				return null;

			if (mode == ResizeMode.Fill)
			{
				double scaleWidth = (double)resultSize.Width / sourceSize.Width;
				double scaleHeight = (double)resultSize.Height / sourceSize.Height;
				double scale = Math.Max(scaleWidth, scaleHeight);
				var fillSize = new Size((int)Math.Round(sourceSize.Width * scale), (int)Math.Round(sourceSize.Height * scale));
				int left = (fillSize.Width - resultSize.Width) / 2;
				int top = (fillSize.Height - resultSize.Height) / 2;
				var rect = new SKRectI(left, top, left + resultSize.Width, top + resultSize.Height);
				var fillBitmap = new SKBitmap(resultSize.Width, resultSize.Height);
				using var filled = bitmap.Resize(fillSize, sampling);
				filled.ExtractSubset(fillBitmap, rect);
				return fillBitmap;
			}

			var fitSize = maxSize.Downscale(bitmap.Width, bitmap.Height);
			var resized = bitmap.Resize(fitSize, sampling);
			if (mode == ResizeMode.Fit)
				return resized;

			var padBitmap = new SKBitmap(resultSize.Width, resultSize.Height);
			using (var canvas = new SKCanvas(padBitmap))
			{
				int left = (resultSize.Width - fitSize.Width) / 2;
				int top = (resultSize.Height - fitSize.Height) / 2;
				canvas.Clear(SKColors.Transparent);
				canvas.DrawBitmap(resized, left, top);
			}
			resized.Dispose();
			return padBitmap;
		}

		/// <summary>
		/// Scales <see cref="SKBitmap"/>.
		/// </summary>
		/// <param name="scale">Scale factor.</param>
		/// <param name="sampling">Resize sampling.</param>
		public SKBitmap Scale(double scale, SKSamplingOptions sampling)
		{
			Validate.GreaterThan(scale, 0);
			var size = new Size((int)(bitmap.Width * scale), (int)(bitmap.Height * scale));
			return bitmap.Resize(size, sampling);
		}

		/// <summary>
		/// Rotates <see cref="SKBitmap"/> 90 degrees.
		/// </summary>
		/// <param name="clockwise">Rotate 90 degrees clockwise or counterclockwise</param>
		/// <returns>Rotated bitmap.</returns>
		public SKBitmap Rotate90(bool clockwise)
		{
			var rotated = new SKBitmap(bitmap.Height, bitmap.Width);
			using var canvas = new SKCanvas(rotated);
			if (clockwise)
			{
				canvas.Translate(rotated.Width, 0);
				canvas.RotateDegrees(90);
			}
			else
			{
				canvas.Translate(0, rotated.Height);
				canvas.RotateDegrees(-90);
			}
			canvas.DrawBitmap(bitmap, 0, 0);
			return rotated;
		}

		/// <summary>
		/// Rotates <see cref="SKBitmap"/> according EXIF orientation.
		/// </summary>
		/// <returns>Rotated bitmap or <c>null</c> if rotation does not require.</returns>
		public SKBitmap? HandleOrientation(SKEncodedOrigin orientation)
		{
			SKBitmap result;
			switch (orientation)
			{
				case SKEncodedOrigin.BottomRight: // rotated 180
					result = new SKBitmap(bitmap.Width, bitmap.Height);
					using (var surface = new SKCanvas(result))
					{
						surface.RotateDegrees(180, (float)bitmap.Width / 2, (float)bitmap.Height / 2);
						surface.DrawBitmap(bitmap, 0, 0);
					}
					return result;
				case SKEncodedOrigin.RightTop: // rotated 90 cw
					result = new SKBitmap(bitmap.Height, bitmap.Width);
					using (var surface = new SKCanvas(result))
					{
						surface.Translate(result.Width, 0);
						surface.RotateDegrees(90);
						surface.DrawBitmap(bitmap, 0, 0);
					}
					return result;
				case SKEncodedOrigin.LeftBottom: // rotated 90 ccw
					result = new SKBitmap(bitmap.Height, bitmap.Width);
					using (var surface = new SKCanvas(result))
					{
						surface.Translate(0, result.Height);
						surface.RotateDegrees(270);
						surface.DrawBitmap(bitmap, 0, 0);
					}
					return result;
				default:
					return null;
			}
		}

		/// <summary>
		/// Trims bitmap transparent pixels and returns new <see cref="SKBitmap"/>.
		/// </summary>
		/// <returns>Trimmed bitmap or <c>null</c> if there is no transparent pixels.</returns>
		public SKBitmap? TrimTransparency()
		{
			var nonTransparentPixels = bitmap.Pixels
				.Index()
				.Where(px => px.Item.Alpha != byte.MinValue)
				.ToList();
			if (nonTransparentPixels.Count == 0)
				return null;

			int left = nonTransparentPixels.Min(px => px.Index % bitmap.Width);
			int top = nonTransparentPixels.Min(px => px.Index / bitmap.Width);
			int right = nonTransparentPixels.Max(px => px.Index % bitmap.Width) + 1;
			int bottom = nonTransparentPixels.Max(px => px.Index / bitmap.Width) + 1;
			var rect = new SKRectI(left, top, right, bottom);
			var result = new SKBitmap(rect.Width, rect.Height);
			bitmap.ExtractSubset(result, rect);
			return result;
		}
	}
}