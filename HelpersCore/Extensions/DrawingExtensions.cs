using System.Drawing;

namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="Point"/> and <see cref="Size"/>.
/// </summary>
public static class DrawingExtensions
{
	extension(ref PointF point)
	{
		/// <summary>
		/// Multiplies point's X and Y by <paramref name="factor"/>.
		/// </summary>
		/// <param name="factor">Multiply factor.</param>
		public static PointF operator *(PointF p, float factor)
			=> new(p.X * factor, p.Y * factor);

		/// <summary>
		/// Multiplies point's X and Y by <paramref name="factor"/>.
		/// </summary>
		/// <param name="factor">Multiply factor.</param>
		public void operator *= (float factor)
		{
			point.X *= factor;
			point.Y *= factor;
		}

		/// <summary>
		/// Divides point's X and Y by <paramref name="divider"/>.
		/// </summary>
		/// <param name="divider">Division divider.</param>
		public static PointF operator /(PointF p, float divider)
			=> new(p.X / divider, p.Y / divider);

		/// <summary>
		/// Divides point's X and Y by <paramref name="divider"/>.
		/// </summary>
		/// <param name="divider">Division divider.</param>
		public void operator /= (float divider)
		{
			point.X /= divider;
			point.Y /= divider;
		}
	}

	extension(PointF point)
	{
		/// <summary>
		/// Returns distance from current points to <paramref name="other"/>.
		/// </summary>
		public double DistanceTo(Point other)
		{
			double dx = other.X - point.X;
			double dy = other.Y - point.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}
	}

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
			Validate.GreaterThan(maxWidth, 0);
			Validate.GreaterThan(maxHeight, 0);

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
	}
}