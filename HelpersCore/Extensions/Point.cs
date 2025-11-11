using System.Drawing;

namespace HelpersCore;

public static partial class Extensions
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
}