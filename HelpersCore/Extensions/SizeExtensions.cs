using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="Size"/>.
/// </summary>
public static class SizeExtensions
{
	extension(ref Size size)
	{
		/// <summary>
		/// Multiplies size width and height by <paramref name="factor"/>.
		/// </summary>
		/// <param name="factor">Multiply factor.</param>
		public static Size operator *(Size s, double factor)
			=> new((int)Math.Round(s.Width * factor), (int)Math.Round(s.Height * factor));

		/// <summary>
		/// Multiplies size width and height by <paramref name="factor"/>.
		/// </summary>
		/// <param name="factor">Multiply factor.</param>
		public void operator *= (double factor)
		{
			size.Width = (int)Math.Round(size.Width * factor);
			size.Height = (int)Math.Round(size.Height * factor);
		}

		/// <summary>
		/// Divides size width and height by <paramref name="divider"/>.
		/// </summary>
		/// <param name="divider">Division divider.</param>
		public static PointF operator /(Size s, double divider)
			=> new((int)Math.Round(s.Width / divider), (int)Math.Round(s.Height / divider));

		/// <summary>
		/// Divides size width and height by <paramref name="divider"/>.
		/// </summary>
		/// <param name="divider">Division divider.</param>
		public void operator /= (double divider)
		{
			size.Width = (int)Math.Round(size.Width / divider);
			size.Height = (int)Math.Round(size.Height / divider);
		}
	}

	extension(Size size)
	{
		/// <summary>
		/// Returns string in <c>{width}x{height}</c> format.
		/// </summary>
		public string ToResolutionString()
			=> $"{size.Width}x{size.Height}";

		/// <summary>
		/// Returns downscaled size saving original proportions.
		/// </summary>
		/// <param name="maxSize">Desired maximum width and height.</param>
		public Size Downscale(Size maxSize, ResizeMode mode = ResizeMode.Fit)
			=> size.Downscale(maxSize.Width, maxSize.Height, mode);

		/// <summary>
		/// Returns downscaled size saving original proportions.
		/// </summary>
		/// <param name="maxWidth">Desired maximum width.</param>
		/// <param name="maxHeight">Desired maximum height.</param>
		public Size Downscale(int maxWidth, int maxHeight, ResizeMode mode = ResizeMode.Fit)
		{
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxWidth);
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxHeight);

			if (size.IsEmpty)
				return new Size(maxWidth, maxHeight);
			if (mode == ResizeMode.Pad)
				return new Size(size.Width == 0 ? maxWidth : size.Width, size.Height == 0 ? maxHeight : size.Height);

			int width = Math.Min(maxWidth, size.Width);
			int height = Math.Min(maxHeight, size.Height);
			if (width == 0)
				width = (int)Math.Round((double)maxWidth * height / maxHeight);
			else if (height == 0)
				height = (int)Math.Round((double)maxHeight * width / maxWidth);

			double scaleWidth = width / (double)maxWidth;
			double scaleHeight = height / (double)maxHeight;
			double scale = mode == ResizeMode.Fit ? Math.Min(scaleWidth, scaleHeight) : Math.Max(scaleWidth, scaleHeight);
			int resultWidth = (int)Math.Round(maxWidth * scale);
			int resultHeight = (int)Math.Round(maxHeight * scale);
			return new Size(Math.Min(resultWidth, width), Math.Min(resultHeight, height));
		}

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
		public static bool TryParse([NotNullWhen(true)] string? s, out Size result)
			=> TryParse(s.AsSpan(), out result);

		/// <summary>
		/// Parses <see cref="Size"/> from <c>{width}x{height}</c> string.
		/// </summary>
		public static Size Parse(ReadOnlySpan<char> s)
		{
			int index = s.IndexOf('x');
			if (index == -1)
				throw new FormatException("Size string must contain 'x' separator.");
			return new Size(int.Parse(s[..index]), int.Parse(s[(index + 1)..]));
		}

		/// <summary>
		/// Parses <see cref="Size"/> from <c>{width}x{height}</c> string.
		/// </summary>
		public static Size Parse(string s)
			=> Parse(s.AsSpan());
	}
}