namespace HelpersCore;

public static class DecimalExtensions
{
	/// <summary>
	/// Returns integer digits count.
	/// </summary>
	public static int GetIntegerPlaces(this decimal x)
	{
		x = decimal.Truncate(Math.Abs(x));
		int places = 0;
		while (x > 1)
		{
			places++;
			x = decimal.Truncate(x / 10);
		}
		return places;
	}

	/// <summary>
	/// Returns fraction digits count.
	/// </summary>
	public static int GetFractionPlaces(this decimal x)
	{
		x = Math.Abs(x); // make sure it is positive.
		x -= (int)x; // remove the integer part of the number.
		int places = 0;
		while (x > 0)
		{
			places++;
			x *= 10;
			x -= (int)x;
		}
		return places;
	}

	/// <summary>
	/// Rounds decimal to specified decimal places.
	/// </summary>
	/// <param name="decimals">Decimal placees to round to.</param>
	/// <param name="mode">Rounding mode.</param>
	public static decimal Round(this decimal value, int decimals = 0, MidpointRounding mode = MidpointRounding.AwayFromZero)
		=> decimal.Round(value, decimals, mode);
}