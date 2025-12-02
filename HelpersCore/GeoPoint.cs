using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json.Serialization;

namespace HelpersCore;

/// <summary>
/// Represents geographic point.
/// </summary>
public record struct GeoPoint : IParsable<GeoPoint>
{
	/// <summary>
	/// Gets a <see cref="GeoPoint"/> with <see cref="Lng"/> and <see cref="Lat"/> set to <see cref="double.NaN"/>.
	/// </summary>
	public static GeoPoint NaN { get; } = new(double.NaN, double.NaN);

	/// <summary>
	/// Gets or sets the point longitude (x-coordinate).
	/// </summary>
	[Range(-180, 180)]
	public double Lng { get; set; }

	/// <summary>
	/// Gets or sets the point latitude (y-coordinate).
	/// </summary>
	[Range(-90, 90)]
	public double Lat { get; set; }

	/// <summary>
	/// Gets or sets the point altitude (z-coordinate).
	/// </summary>
	public double? Alt { get; set; }

	/// <summary>
	/// Gets if <see cref="Lng"/> or <see cref="Lat"/> is <see cref="double.NaN"/>.
	/// </summary>
	[JsonIgnore]
	public readonly bool IsNaN => double.IsNaN(Lng) || double.IsNaN(Lat);

	/// <summary>
	/// Initializes new instance.
	/// </summary>
	/// <param name="lng">Point longitude (x-coordinate).</param>
	/// <param name="lat">Point latitude (y-coordinate).</param>
	/// <param name="altitude">Point altitude (z-coordinate).</param>
	public GeoPoint(double lng, double lat, double? altitude = null)
	{
		Validate.Range(lng, -180, 180);
		Validate.Range(lat, -90, 90);
		Lng = lng;
		Lat = lat;
	}

	/// <summary>
	/// Returns string representation in <c>lng,lat</c> format.
	/// </summary>
	public readonly override string ToString()
		=> IsNaN ? "NaN" : $"{Lng.ToString(CultureInfo.InvariantCulture)},{Lat.ToString(CultureInfo.InvariantCulture)}";

	/// <summary>
	/// Returns the distance between the current point <see cref="Lng"/> and <see cref="Lat"/>
	/// coordinates and <paramref name="other"/> one on the Earth's surface.
	/// </summary>
	/// <param name="other">The <see cref="GeoPoint"/> to calculate distance to.</param>
	/// <returns>The distance between the two coordinates, in meters.</returns>
	/// <remarks>
	/// Source: https://github.com/ghuntley/geocoordinate/blob/master/src/GeoCoordinatePortable/GeoCoordinate.cs
	/// </remarks>
	public readonly double GetDistanceTo(GeoPoint other)
		=> GetDistance(Lng, Lat, other.Lng, other.Lat);

	/// <summary>
	/// Deconstructs the point into longitude and latitude.
	/// </summary>
	public readonly void Deconstruct(out double lng, out double lat)
		=> (lng, lat) = (Lng, Lat);

	/// <summary>
	/// Deconstructs the point into longitude, latitude and altitude.
	/// </summary>
	public readonly void Deconstruct(out double lng, out double lat, out double? altitude)
		=> (lng, lat, altitude) = (Lng, Lat, Alt);

	/// <summary>
	/// Tries to parse <see cref="GeoPoint"/> from <c>lng,lat</c> or <c>lng;lat</c> string.
	/// </summary>
	/// <param name="s">Source string to parse.</param>
	/// <param name="result">Parsed <see cref="GeoPoint"/> if successful.</param>
	/// <returns><c>True</c> if parse successful.</returns>
	public static bool TryParse(ReadOnlySpan<char> s, out GeoPoint result)
	{
		int index = s.IndexOf(',');
		if (index == -1)
			index = s.IndexOf(';');
		if (index != -1
			&& double.TryParse(s[..index], NumberStyles.Any, CultureInfo.InvariantCulture, out double lng)
			&& double.TryParse(s[(index + 1)..], NumberStyles.Any, CultureInfo.InvariantCulture, out double lat)
			&& lng is >= -180 and <= 180
			&& lat is >= -90 and <= 90)
		{
			result = new(lng, lat);
			return true;
		}
		result = NaN;
		return false;
	}

	/// <summary>
	/// Tries to parse <see cref="GeoPoint"/> from <c>lng,lat</c> or <c>lng;lat</c> string.
	/// </summary>
	/// <param name="s">Source string to parse.</param>
	/// <param name="result">Parsed <see cref="GeoPoint"/> if successful.</param>
	/// <returns><c>True</c> if parse successful.</returns>
	public static bool TryParse([NotNullWhen(true)] string? s, out GeoPoint result)
		=> TryParse(s.AsSpan(), out result);

	static bool IParsable<GeoPoint>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out GeoPoint result)
		=> TryParse(s.AsSpan(), out result);

	/// <summary>
	/// Parses <see cref="GeoPoint"/> from <c>lng,lat</c> or <c>lng;lat</c> string.
	/// </summary>
	public static GeoPoint Parse(ReadOnlySpan<char> s)
		=> TryParse(s, out var result) ? result : throw new FormatException("Input string was not in correct format");

	/// <summary>
	/// Parses <see cref="GeoPoint"/> from <c>lng,lat</c> or <c>lng;lat</c> string.
	/// </summary>
	public static GeoPoint Parse(string s)
		=> Parse(s.AsSpan());

	static GeoPoint IParsable<GeoPoint>.Parse(string s, IFormatProvider? provider)
		=> Parse(s.AsSpan());

	/// <summary>
	/// Returns the distance between two geo-coordinates on the Earth's surface
	/// that are specified by their longitude and latitude.
	/// </summary>
	/// <param name="lng1">First geo-coordinate longitude (x-coordinate).</param>
	/// <param name="lat1">First geo-coordinate latitude (y-coordinate).</param>
	/// <param name="lng2">Second geo-coordinate longitude (x-coordinate).</param>
	/// <param name="lat2">Second geo-coordinate latitude (y-coordinate).</param>
	/// <returns>The distance between the two coordinates, in meters.</returns>
	/// <remarks>
	/// Source: https://github.com/ghuntley/geocoordinate/blob/master/src/GeoCoordinatePortable/GeoCoordinate.cs
	/// </remarks>
	public static double GetDistance(double lng1, double lat1, double lng2, double lat2)
	{
		if (double.IsNaN(lng1))
			throw new ArgumentException("Longitude1 is NaN", nameof(lng1));
		if (double.IsNaN(lat1))
			throw new ArgumentException("Latitude1 is NaN", nameof(lat1));
		if (double.IsNaN(lng2))
			throw new ArgumentException("Longitude2 is NaN", nameof(lng2));
		if (double.IsNaN(lat2))
			throw new ArgumentException("Latitude2 is NaN", nameof(lat2));

		double d1 = lat1 * (Math.PI / 180.0);
		double num1 = lng1 * (Math.PI / 180.0);
		double d2 = lat2 * (Math.PI / 180.0);
		double num2 = lng2 * (Math.PI / 180.0) - num1;
		double d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
			Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
		return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
	}
}