using NpgsqlTypes;

namespace HelpersCore;

public static partial class NpgsqlExtensions
{
	const double EarthRadius = 6_378_137; // in meters

	extension(NpgsqlPoint point)
	{
		/// <summary>
		/// Converts to <see cref="GeoPoint"/>.
		/// </summary>
		public GeoPoint ToGeoPoint()
			=> new(point.X, point.Y);

		/// <summary>
		/// Returns the distance between the current point and other one on the Earth's surface.
		/// </summary>
		/// <param name="lng">Other point longitude (x-coordinate).</param>
		/// <param name="lat">Other point latitude (y-coordinate).</param>
		/// <returns>The distance between the two coordinates, in meters.</returns>
		public double GetDistanceTo(double lng, double lat)
			=> GeoPoint.GetDistance(point.X, point.Y, lng, lat);

		/// <summary>
		/// Returns the distance between the current point and <paramref name="other"/> one on the Earth's surface.
		/// </summary>
		/// <param name="other">Other point to calculate distance to.</param>
		/// <returns>The distance between the two coordinates, in meters.</returns>
		public double GetDistanceTo(NpgsqlPoint other)
			=> GeoPoint.GetDistance(point.X, point.Y, other.X, other.Y);

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
			=> circle.Center.GetDistanceTo(point) <= circle.Radius;

		/// <summary>
		/// Returns the bounding box of the circle.
		/// </summary>
		public GeoBounds GetBounds()
			=> new(
				circle.Center.Add(-circle.Radius, circle.Radius).ToGeoPoint(),
				circle.Center.Add(circle.Radius, -circle.Radius).ToGeoPoint()
			);
	}

	extension(NpgsqlPolygon polygon)
	{
		/// <summary>
		/// Returns the bounding box of the polygon.
		/// </summary>
		public GeoBounds GetBounds()
			=> GeoBounds.FromPoints(polygon.Select(p => p.ToGeoPoint()));
	}

	extension(GeoPoint point)
	{
		/// <summary>
		/// Converts to <see cref="NpgsqlPoint"/>.
		/// </summary>
		public NpgsqlPoint ToNpgsql()
			=> new(point.Lng, point.Lat);
	}
}