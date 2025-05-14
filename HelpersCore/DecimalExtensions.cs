namespace HelpersCore;

public static class DecimalExtensions
{
	/// <summary>
	/// Returns integer digits count.
	/// </summary>
	public static int GetIntegerPlaces(this decimal v)
	{
		v = decimal.Truncate(Math.Abs(v));
		int places = 0;
		while (v > 1)
		{
			places++;
			v = decimal.Truncate(v / 10);
		}
		return places;
	}

	/// <summary>
	/// Returns fraction digits count.
	/// </summary>
	public static int GetFractionPlaces(this decimal v)
	{
		v = Math.Abs(v); // make sure it is positive.
		v -= (int)v; // remove the integer part of the number.
		int places = 0;
		while (v > 0)
		{
			places++;
			v *= 10;
			v -= (int)v;
		}
		return places;
	}

	/// <summary>
	/// Rounds decimal to specified decimal places.
	/// </summary>
	/// <param name="decimals">Decimal places to round to.</param>
	/// <param name="mode">Rounding mode.</param>
	public static decimal Round(this decimal v, int decimals = 0, MidpointRounding mode = MidpointRounding.AwayFromZero)
		=> decimal.Round(v, decimals, mode);
}