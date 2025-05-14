using System.Drawing;

namespace HelpersCore;

public static class DrawingExtensions
{
	/// <summary>
	/// Multiplies point's X and Y by <paramref name="factor"/>.
	/// </summary>
	/// <param name="factor">Multiply factor.</param>
	public static PointF Multiply(this PointF point, float factor)
		=> new(point.X * factor, point.Y * factor);

	/// <summary>
	/// Divides point's X and Y by <paramref name="divider"/>.
	/// </summary>
	/// <param name="divider">Division divider.</param>
	public static PointF Divide(this PointF point, float divider)
		=> new(point.X / divider, point.Y / divider);

	// /// <summary>
	// /// Multiplies sкщщьize width and height by <paramref name="factor"/>.
	// /// </summary>
	// /// <param name="factor">Multiply factor.</param>
	// public static Size Multiply(this Size size, double factor)
	// 	=> new((int)Math.Round(size.Width * factor), (int)Math.Round(size.Height * factor));

	/// <summary>
	/// Returns distance from current points to <paramref name="other"/>.
	/// </summary>
	public static double DistanceTo(this Point point, Point other)
	{
		double dx = other.X - point.X;
		double dy = other.Y - point.Y;
		return Math.Sqrt(dx * dx + dy * dy);
	}

	/// <summary>
	/// Returns downscaled size saving original proportions.
	/// </summary>
	/// <param name="maxSize">Desired maximum width and height.</param>
	public static Size Downscale(this Size size, Size maxSize, ResizeMode mode = ResizeMode.Fit)
		=> Downscale(size, maxSize.Width, maxSize.Height, mode);

	/// <summary>
	/// Returns downscaled size saving original proportions.
	/// </summary>
	/// <param name="maxWidth">Desired maximum width.</param>
	/// <param name="maxHeight">Desired maximum height.</param>
	public static Size Downscale(this Size size, int maxWidth, int maxHeight, ResizeMode mode = ResizeMode.Fit)
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