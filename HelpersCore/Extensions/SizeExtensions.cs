using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="Size"/>.
/// </summary>
[SuppressMessage("ReSharper", "InvokeAsExtensionMemberFromSameClass")]
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
		public void operator *=(double factor)
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
		public void operator /=(double divider)
		{
			size.Width = (int)Math.Round(size.Width / divider);
			size.Height = (int)Math.Round(size.Height / divider);
		}
	}

	extension(Size size)
	{
		/// <summary>
		/// Deconstructs size to width and height.
		/// </summary>
		public void Deconstruct(out int width, out int height)
		{
			width = size.Width;
			height = size.Height;
		}

		/// <summary>
		/// Returns string in <c>{width}x{height}</c> format.
		/// </summary>
		public string ToResolutionString()
			=> $"{size.Width}x{size.Height}";

		/// <summary>
		/// Returns if <paramref name="size"/> and <paramref name="other"/> have the same aspect ratio.
		/// </summary>
		public bool AspectEquals(Size other)
			=> (int)Math.Round((double)size.Width / other.Width * other.Height) == size.Height;

		/// <summary>
		/// Returns if <paramref name="size"/> fits in <paramref name="other"/>.
		/// </summary>
		public bool FitsIn(Size other)
			=> size.Width <= other.Width && size.Height <= other.Height;

		/// <summary>
		/// Returns clipped size to <paramref name="maxWidth"/> and <paramref name="maxHeight"/>.
		/// </summary>
		/// <param name="maxWidth">Maximum width.</param>
		/// <param name="maxHeight">Maximum height.</param>
		/// <returns>Clipped size.</returns>
		public Size Clip(int maxWidth, int maxHeight)
			=> new(Math.Min(size.Width, maxWidth), Math.Min(size.Height, maxHeight));

		/// <summary>
		/// Returns clipped size to <paramref name="max"/> size.
		/// </summary>
		/// <param name="max">Maximum size.</param>
		/// <returns>Clipped size.</returns>
		public Size Clip(Size max)
			=> size.Clip(max.Width, max.Height);

		/// <summary>
		/// Returns downscaled size saving original proportions.
		/// </summary>
		/// <param name="desired">Desired width and height.</param>
		/// <param name="fill">
		/// If <c>false</c> then fits within the desired size.
		/// If <c>true</c> then fills the desired size and resulting width or height can be greater that desired size.
		/// </param>
		public Size Downscale(Size desired, bool fill = false)
			=> size.Downscale(desired.Width, desired.Height, fill);

		/// <summary>
		/// Returns downscaled size saving original proportions.
		/// </summary>
		/// <param name="desiredWidth">Desired width. If zero then does not used.</param>
		/// <param name="desiredHeight">Desired height. If zero then does not used.</param>
		/// <param name="fill">
		/// If <c>false</c> then fits within the desired size.
		/// If <c>true</c> then fills the desired size and resulting width or height can be greater that desired size.
		/// </param>
		public Size Downscale(int desiredWidth, int desiredHeight, bool fill = false)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(desiredWidth);
			ArgumentOutOfRangeException.ThrowIfNegative(desiredHeight);
			if (size.IsEmpty)
				return new(desiredWidth, desiredHeight);

			if (desiredWidth == 0)
				desiredWidth = size.Width;
			if (desiredHeight == 0)
				desiredHeight = size.Height;

			double scaleWidth = Math.Min((double)desiredWidth / size.Width, 1);
			double scaleHeight = Math.Min((double)desiredHeight / size.Height, 1);
			double scale = fill
				? Math.Max(scaleWidth, scaleHeight)
				: Math.Min(scaleWidth, scaleHeight);
			int resWidth = (int)Math.Round(size.Width * scale);
			int resHeight = (int)Math.Round(size.Height * scale);
			return new(resWidth, resHeight);
		}

		/// <summary>
		/// Returns downscaled size saving original proportions and clipped to <paramref name="maxWidth"/> and <paramref name="maxHeight"/>.
		/// </summary>
		/// <param name="maxWidth">Maximum width.</param>
		/// <param name="maxHeight">Maximum height.</param>
		/// <param name="mode">Resize mode.</param>
		/// <returns>Downscaled and clipped size.</returns>
		public Size DownscaleAndClip(int maxWidth, int maxHeight, ResizeMode mode)
			=> size.Downscale(maxWidth, maxHeight, fill: mode == ResizeMode.Fill).Clip(new Size(maxWidth, maxHeight));

		/// <summary>
		/// Returns downscaled size saving original proportions and clipped to <paramref name="max"/> size.
		/// </summary>
		/// <param name="max">Maximum size.</param>
		/// <param name="mode">Resize mode.</param>
		/// <returns>Downscaled and clipped size.</returns>
		public Size DownscaleAndClip(Size max, ResizeMode mode)
			=> size.Downscale(max, mode == ResizeMode.Fill).Clip(max);

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

	extension(SizeF size)
	{
		/// <summary>
		/// Deconstructs size to width and height.
		/// </summary>
		public void Deconstruct(out float width, out float height)
		{
			width = size.Width;
			height = size.Height;
		}
	}
}