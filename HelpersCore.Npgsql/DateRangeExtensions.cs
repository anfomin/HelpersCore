namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="DateRange"/>.
/// </summary>
public static class DateRangeExtensions
{
	extension(DateOnly date)
	{
		/// <summary>
		/// Creates a new <see cref="DateRange"/> for hole month from <see cref="DateOnly"/>.
		/// </summary>
		/// <returns></returns>
		public DateRange GetMonthRange()
			=> new(date.MonthStart, date.MonthEnd);
	}

	extension(DateRange range)
	{
		/// <summary>
		/// Gets number of days in the range.
		/// </summary>
		/// <returns>Number of days.</returns>
		public int Days
			=> range.End.DayNumber - range.Start.DayNumber + 1;

		/// <summary>
		/// Gets number of days in the range.
		/// </summary>
		/// <returns>Number of days or <c>null</c> if range is empty or any bound is infinite.</returns>
		public int? DaysOrNull
			=> range.IsStartInfinite || range.IsEndInfinite ? null : range.Days;

		/// <summary>
		/// Creates a new <see cref="DateRange"/> for the hole range months.
		/// </summary>
		public DateRange GetMonthRange()
			=> new(range.Start.MonthStart, range.End.MonthEnd);

		/// <summary>
		/// Enumerates days in the range.
		/// </summary>
		/// <returns><see cref="IEnumerable{DateOnly}"/> for days in range or empty collection if range empty or any bound is infinite.</returns>
		public IEnumerable<DateOnly> EnumerateDays()
			=> range.IsFinite ? DateOnly.EnumerateDays(range.Start, range.End) : [];

		/// <summary>
		/// Enumerates months in the range.
		/// </summary>
		/// <param name="wholeMonths">
		/// <c>True</c> to return whole months.
		/// <c>False</c> to return period in range only.
		/// </param>
		/// <returns>
		/// <see cref="IEnumerable{DateRange}"/> for months in range or empty collection if range empty or any bound is infinite.
		/// </returns>
		public IEnumerable<DateRange> EnumerateMonths(bool wholeMonths = false)
		{
			if (range is not { StartOrNull: { } start, EndOrNull: { } end })
				yield break;
			while (start <= end)
			{
				if (wholeMonths && start.Day != 1)
					start = start.MonthStart;
				var monthEnd = wholeMonths ? start.MonthEnd : DateOnly.Min(start.MonthEnd, end);
				yield return new(start, monthEnd);
				start = monthEnd.AddDays(1);
			}
		}

		/// <summary>
		/// Enumerates weeks in the range.
		/// </summary>
		/// <returns><see cref="IEnumerable{DateRange}"/> for weeks in range or empty collection if range empty or any bound is infinite.</returns>
		public IEnumerable<DateRange> EnumerateWeeks()
		{
			if (range is not { StartOrNull: { } start, EndOrNull: { } end })
				yield break;
			while (start <= end)
			{
				var weekEnd = start.GetNextDayOfWeek(DayOfWeek.Sunday);
				yield return new(start, weekEnd);
				start = weekEnd.AddDays(1);
			}
		}
	}
}