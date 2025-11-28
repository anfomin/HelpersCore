using NpgsqlTypes;

namespace HelpersCore;

public static partial class NpgsqlExtensions
{
	extension(DateOnly date)
	{
		/// <summary>
		/// Creates a new <see cref="NpgsqlRange&lt;DateOnly&gt;"/> for hole month from <see cref="DateOnly"/>.
		/// </summary>
		public NpgsqlRange<DateOnly> ToMonthRange()
			=> new(date.GetMonthBegin(), date.GetMonthEnd());

		/// <summary>
		/// Creates a new <see cref="NpgsqlRange&lt;DateTime&gt;"/> for hole month converting
		/// <see cref="DateOnly"/> from the specified <paramref name="sourceTimeZone"/> to UTC.
		/// </summary>
		/// <param name="sourceTimeZone">Source timezone.</param>
		public NpgsqlRange<DateTime> ToUniversalMonthRange(TimeZoneInfo sourceTimeZone)
			=> new(
				date.GetMonthBegin().ToUniversalTime(TimeOnly.MinValue, sourceTimeZone),
				date.GetMonthEnd().ToUniversalTime(TimeOnly.MaxValue, sourceTimeZone)
			);

		/// <summary>
		/// Creates a new <see cref="NpgsqlRange&lt;DateTime&gt;"/> converting <see cref="DateOnly"/>
		/// and <see cref="TimeOnly"/> from the specified <paramref name="sourceTimeZone"/> to UTC.
		/// </summary>
		/// <param name="sourceTimeZone">Source timezone.</param>
		/// <param name="timeFrom">Lower bound time in <paramref name="sourceTimeZone"/>.</param>
		/// <param name="timeTo">Upper bound time in <paramref name="sourceTimeZone"/>.</param>
		public NpgsqlRange<DateTime> ToUniversalRange(TimeZoneInfo sourceTimeZone, TimeOnly? timeFrom = null, TimeOnly? timeTo = null)
			=> new(
				date.ToUniversalTime(timeFrom ?? TimeOnly.MinValue, sourceTimeZone),
				date.ToUniversalTime(timeTo ?? TimeOnly.MaxValue, sourceTimeZone)
			);
	}
}