using NpgsqlTypes;

namespace HelpersCore;

public static partial class NpgsqlExtensions
{
	const double EarthRadius = 6_378_137; // in meters

	extension(NpgsqlPoint point)
	{
		/// <summary>
		/// Calculates the distance between current point and other one on the Earth's surface.
		/// </summary>
		/// <param name="lng">Other point longitude on the Earth's surface.</param>
		/// <param name="lat">Other point latitude on the Earth's surface.</param>
		/// <returns>Distance in meters.</returns>
		public double DistanceTo(double lng, double lat)
		{
			var d1 = point.Y * (Math.PI / 180.0);
			var num1 = point.X * (Math.PI / 180.0);
			var d2 = lat * (Math.PI / 180.0);
			var num2 = lng * (Math.PI / 180.0) - num1;
			var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
				Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
			return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
		}

		/// <summary>
		/// Calculates the distance between current point and other one on the Earth's surface.
		/// </summary>
		/// <param name="other">Other point on the Earth's surface.</param>
		/// <returns>Distance in meters.</returns>
		public double DistanceTo(NpgsqlPoint other)
			=> DistanceTo(point, other.X, other.Y);

		/// <summary>
		/// Adds specified <paramref name="x"/> and <paramref name="y"/> to the point's coordinates.
		/// </summary>
		public NpgsqlPoint Add(double x, double y)
			=> new(
				point.X + (x / EarthRadius) * (180 / Math.PI) / Math.Cos(point.Y * Math.PI / 180),
				point.Y + (y / EarthRadius) * (180 / Math.PI)
			);
	}

	extension(NpgsqlCircle circle)
	{
		/// <summary>
		/// Returns if the circle contains specified <paramref name="point"/>.
		/// </summary>
		public bool Contains(NpgsqlPoint point)
			=> circle.Center.DistanceTo(point) <= circle.Radius;

		/// <summary>
		/// Returns the bounding box of the circle.
		/// </summary>
		public NpgsqlBounds GetBounds()
			=> new(
				circle.Center.Add(-circle.Radius, circle.Radius),
				circle.Center.Add(circle.Radius, -circle.Radius)
			);
	}

	/// <summary>
	/// Returns the bounding box of all points or <c>null</c> if the collection is empty.
	/// </summary>
	public static NpgsqlBounds? GetBounds(this IEnumerable<NpgsqlPoint> points)
		=> points.Any()
			? new(
				new(points.Select(p => p.X).Min(), points.Select(p => p.Y).Max()),
				new(points.Select(p => p.X).Max(), points.Select(p => p.Y).Min())
			)
			: null;
}