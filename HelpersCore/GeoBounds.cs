namespace HelpersCore;

/// <summary>
/// Represents geographic bounding box with top left and bottom right points.
/// </summary>
/// <param name="TopLeft">Top left point.</param>
/// <param name="BottomRight">Bottom right point.</param>
public readonly record struct GeoBounds(GeoPoint TopLeft, GeoPoint BottomRight)
{
	/// <summary>
	/// Gets bottom left point.
	/// </summary>
	public GeoPoint BottomLeft => new(TopLeft.Lng, BottomRight.Lat);

	/// <summary>
	/// Gets top right point.
	/// </summary>
	public GeoPoint TopRight => new(BottomRight.Lng, TopLeft.Lat);

	/// <summary>
	/// Initializes new instance.
	/// </summary>
	/// <param name="topLeftLng">Top left point longitude.</param>
	/// <param name="topLeftLat">Top left point latitude.</param>
	/// <param name="bottomRightLng">Bottom right point longitude.</param>
	/// <param name="bottomRightLat">Bottom right point latitude.</param>
	public GeoBounds(double topLeftLng, double topLeftLat, double bottomRightLng, double bottomRightLat)
		: this(new GeoPoint(topLeftLng, topLeftLat), new GeoPoint(bottomRightLng, bottomRightLat)) { }

	/// <summary>
	/// Returns string representation in <c>[TopLeft;BottomRight]</c> format.
	/// </summary>
	public override string ToString()
		=> $"[{TopLeft};{BottomRight}]";

	/// <summary>
	/// Gets center point of the bounds.
	/// </summary>
	public GeoPoint GetCenter() => new(
		(TopLeft.Lng + BottomRight.Lng) / 2,
		(TopLeft.Lat + BottomRight.Lat) / 2
	);

	/// <summary>
	/// Extends the bounds by specified <paramref name="x"/> and <paramref name="y"/> amounts.
	/// </summary>
	/// <param name="x">Amount to extend longitude (x-coordinate).</param>
	/// <param name="y">Amount to extend latitude (y-coordinate).</param>
	/// <returns>Extended bounds.</returns>
	public GeoBounds Extend(double x, double y) =>new(
		new(TopLeft.Lng - x, TopLeft.Lat + y),
		new(BottomRight.Lng + x, BottomRight.Lat - y)
	);

	/// <summary>
	/// Extends the bounds by specified <paramref name="factor"/> of its size.
	/// </summary>
	/// <param name="factor">Size factor.</param>
	/// <returns>Extended bounds.</returns>
	public GeoBounds ExtendBy(double factor)
	{
		double x = (BottomRight.Lng - TopLeft.Lng) * factor;
		double y = (TopLeft.Lat - BottomRight.Lat) * factor;
		return Extend(x, y);
	}

	/// <summary>
	/// Combines the current bounds with specified <paramref name="bounds"/>.
	/// </summary>
	/// <param name="bounds">Bounds to combine with.</param>
	/// <returns>Bounding box that contains both the current and specified bounds.</returns>
	public GeoBounds CombineWith(params IEnumerable<GeoBounds> bounds)
	{
		double topLeftLng = TopLeft.Lng;
		double topLeftLat = TopLeft.Lat;
		double bottomRightLng = BottomRight.Lng;
		double bottomRightLat = BottomRight.Lat;
		foreach (var b in bounds)
		{
			topLeftLng = Math.Min(topLeftLng, b.TopLeft.Lng);
			topLeftLat = Math.Max(topLeftLat, b.TopLeft.Lat);
			bottomRightLng = Math.Max(bottomRightLng, b.BottomRight.Lng);
			bottomRightLat = Math.Min(bottomRightLat, b.BottomRight.Lat);
		}
		return new(topLeftLng, topLeftLat, bottomRightLng, bottomRightLat);
	}

	/// <summary>
	/// Returns the bounding box of <paramref name="points"/>.
	/// </summary>
	/// <exception cref="ArgumentException"><paramref name="points"/> enumeration is empty.</exception>
	public static GeoBounds FromPoints(IEnumerable<GeoPoint> points)
	{
		bool has = false;
		double xMin = double.MaxValue;
		double xMax = double.MinValue;
		double yMin = double.MaxValue;
		double yMax = double.MinValue;
		foreach (var p in points)
		{
			has = true;
			xMin = Math.Min(xMin, p.Lng);
			xMax = Math.Max(xMax, p.Lng);
			yMin = Math.Min(yMin, p.Lat);
			yMax = Math.Max(yMax, p.Lat);
		}
		if (!has)
			throw new ArgumentException("Points collection is empty.", nameof(points));
		return new(
			new(xMin, yMax),
			new(xMax, yMin)
		);
	}

	/// <summary>
	/// Combines multiple <paramref name="bounds"/> into one.
	/// </summary>
	/// <param name="bounds">Bounds to combine.</param>
	/// <returns>Bounding box that contains specified bounds.</returns>
	/// <exception cref="ArgumentException"><paramref name="bounds"/> enumeration is empty.</exception>
	public static GeoBounds Combine(params IEnumerable<GeoBounds> bounds)
	{
		bool has = false;
		double topLeftLng = double.MaxValue;
		double topLeftLat = double.MinValue;
		double bottomRightLng = double.MinValue;
		double bottomRightLat = double.MaxValue;
		foreach (var b in bounds)
		{
			has = true;
			topLeftLng = Math.Min(topLeftLng, b.TopLeft.Lng);
			topLeftLat = Math.Max(topLeftLat, b.TopLeft.Lat);
			bottomRightLng = Math.Max(bottomRightLng, b.BottomRight.Lng);
			bottomRightLat = Math.Min(bottomRightLat, b.BottomRight.Lat);
		}
		if (!has)
			throw new ArgumentException("Bounds collection is empty.", nameof(bounds));
		return new(topLeftLng, topLeftLat, bottomRightLng, bottomRightLat);
	}
}