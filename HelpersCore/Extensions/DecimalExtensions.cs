namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="Decimal"/>.
/// </summary>
public static class DecimalExtensions
{
	extension(decimal v)
	{
		/// <summary>
		/// Returns integer digits count.
		/// </summary>
		public int GetIntegerPlaces()
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
		public int GetFractionPlaces()
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
		public decimal Round(int decimals = 0, MidpointRounding mode = MidpointRounding.AwayFromZero)
			=> decimal.Round(v, decimals, mode);
	}
}