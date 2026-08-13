using System.Drawing;

namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="PointF"/>.
/// </summary>
public static class PointExtensions
{
	extension(Point point)
	{
		/// <summary>
		/// Deconstructs point to X and Y.
		/// </summary>
		public void Deconstruct(out int x, out int y)
		{
			x = point.X;
			y = point.Y;
		}
	}

	extension(PointF point)
	{
		/// <summary>
		/// Deconstructs point to X and Y.
		/// </summary>
		public void Deconstruct(out float x, out float y)
		{
			x = point.X;
			y = point.Y;
		}
	}
}