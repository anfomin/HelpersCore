namespace HelpersCore;

/// <summary>
/// Represents geographic bounding box with top left and bottom right points.
/// </summary>
/// <param name="TopLeft">Top left point.</param>
/// <param name="BottomRight">Bottom right point.</param>
public readonly record struct GeoBounds(GeoPoint TopLeft, GeoPoint BottomRight)
{
	/// <summary>
	/// Returns string representation in <c>[TopLeft;BottomRight]</c> format.
	/// </summary>
	public override string ToString()
		=> $"[{TopLeft};{BottomRight}]";

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
}