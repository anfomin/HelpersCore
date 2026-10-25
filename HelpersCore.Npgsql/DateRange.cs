using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using NpgsqlTypes;

namespace HelpersCore;

/// <summary>
/// Represents date range.
/// </summary>
public struct DateRange
	: IEquatable<DateRange>, IComparable, IComparable<DateRange>, IFormattable, ISpanParsable<DateRange>
{
	const char Separator = ':';

	/// <summary>
	/// Gets or sets range inclusive start date. If infinite, it is <see cref="DateOnly.MinValue"/>.
	/// </summary>
	[JsonRequired]
	public DateOnly Start { get; set; }

	/// <summary>
	/// Gets or sets range inclusive end date. If infinite, it is <see cref="DateOnly.MaxValue"/>.
	/// </summary>
	[JsonRequired]
	public DateOnly End { get; set; }

	/// <summary>
	/// Gets range start date or <c>null</c> if it is infinite.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public readonly DateOnly? StartOrNull => Start == DateOnly.MinValue ? null : Start;

	/// <summary>
	/// Gets range end date or <c>null</c> if it is infinite.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public readonly DateOnly? EndOrNull => End == DateOnly.MaxValue ? null : End;

	/// <summary>
	/// Gets if range start and end is finite.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public bool IsFinite => Start != DateOnly.MinValue && End != DateOnly.MaxValue;

	/// <summary>
	/// Gets if range is infinite.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public readonly bool IsInfinite => Start == DateOnly.MinValue && End == DateOnly.MaxValue;

	/// <summary>
	/// Gets if range start or end is infinite.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public readonly bool IsInfiniteAny => Start == DateOnly.MinValue || End == DateOnly.MaxValue;

	/// <summary>
	/// Gets if range start is infinite.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public readonly bool IsStartInfinite => Start == DateOnly.MinValue;

	/// <summary>
	/// Gets if range end is infinite.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public readonly bool IsEndInfinite => End == DateOnly.MaxValue;

	/// <summary>
	/// Gets if range is infinite or empty.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public readonly bool IsEmpty => IsInfinite || Start > End;

	/// <summary>
	/// Gets if the range represents a full month.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public readonly bool IsMonth => Start.Day == 1 && Start.MonthEnd == End;

	/// <summary>
	/// Gets if then range starts from the first day of month and ends on the last day of month.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public readonly bool IsHoleMonths => Start.Day == 1 && End.Day == End.DaysInMonth;

	/// <summary>
	/// Gets if the range represents a single day.
	/// </summary>
	[IgnoreDataMember, JsonIgnore]
	public readonly bool IsDay => Start == End;

	/// <summary>
	/// Initializes a new instance of the <see cref="DateRange"/> with the specified start and end dates.
	/// </summary>
	/// <param name="start">The start date of the range.</param>
	/// <param name="end">The end date of the range.</param>
	public DateRange(DateOnly? start, DateOnly? end)
	{
		Start = start ?? DateOnly.MinValue;
		End = end ?? DateOnly.MaxValue;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DateRange"/> with the specified single date.
	/// </summary>
	/// <param name="date">The single date of the range.</param>
	public DateRange(DateOnly date)
	{
		Start = End = date;
	}

	/// <summary>
	/// Deconstructs the <see cref="DateRange"/> into its start and end dates.
	/// </summary>
	/// <param name="start">The start date of the range.</param>
	/// <param name="end">The end date of the range.</param>
	public readonly void Deconstruct(out DateOnly start, out DateOnly end)
		=> (start, end) = (Start, End);

	public readonly override int GetHashCode()
		=> Start.GetHashCode() ^ End.GetHashCode();

	public readonly override bool Equals(object? obj)
		=> obj is DateRange range && Equals(range);

	public readonly bool Equals(DateRange other)
		=> Start == other.Start && End == other.End;

	public readonly int CompareTo(object? obj)
		=> obj is DateRange other ? CompareTo(other) : throw new ArgumentException(@"Object is not DateRange", nameof(obj));

	public readonly int CompareTo(DateRange other)
		=> Start < other.Start ? -1
			: Start > other.Start ? 1
			: End < other.End ? -1
			: End > other.End ? 1
			: 0;

	/// <summary>
	/// Formats the value of the current instance using the specified format.
	/// </summary>
	/// <param name="format">
	/// <para>The format to use.</para>
	/// <para>For <c>O</c> format, the range is formatted as <c>YYYY-MM-DD:YYYY-MM-DD</c> or <c>YYYY-MM-DD:</c> or <c>:YYYY-MM-DD</c>.</para>
	/// <para>For <c>Y</c> format, the range is formatted as <c>Y</c> or <c>MMMM – Y</c> or <c>Y – Y</c>.</para>
	/// <para>For other formats, the range is formatted as <c>{Start} – {End}</c>.</para>
	/// </param>
	/// <param name="provider">An object that supplies culture-specific formatting information.</param>
	/// <returns>The value of the current instance in the specified format.</returns>
	public readonly string ToString([StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string? format, IFormatProvider? provider) => format switch
	{
		_ when IsDay => Start.ToString(format, provider),
		null or "Y" or "y" when IsMonth => Start.ToString("Y", provider),
		null or "Y" or "y" when IsHoleMonths => $"{Start.ToString(Start.Year == End.Year ? "MMMM" : "Y", provider)} – {End.ToString("Y", provider)}",
		"O" or "o" when IsEmpty => Separator.ToString(),
		"O" or "o" when IsEndInfinite => string.Create(provider, $"{Start:format}{Separator}"),
		"O" or "o" when IsStartInfinite => string.Create(provider, $"{Separator}{End:format}"),
		"O" or "o" => string.Create(provider, $"{Start:format}{Separator}{End:format}"),
		_ => DateOnly.ToRangeString(Start, End, format, provider)
	};

	public readonly override string ToString()
		=> ToString(null, CultureInfo.CurrentCulture);

	/// <summary>
	/// Formats the value of the current instance using the specified format.
	/// </summary>
	/// <param name="format">
	/// <para>The format to use.</para>
	/// <para>For <c>O</c> format, the range is formatted as <c>YYYY-MM-DD:YYYY-MM-DD</c> or <c>YYYY-MM-DD:</c> or <c>:YYYY-MM-DD</c>.</para>
	/// <para>For <c>Y</c> format, the range is formatted as <c>Y</c> or <c>MMMM – Y</c> or <c>Y – Y</c>.</para>
	/// <para>For other formats, the range is formatted as <c>{Start} – {End}</c>.</para>
	/// </param>
	/// <returns>The value of the current instance in the specified format.</returns>
	public readonly string ToString([StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string? format)
		=> ToString(format, CultureInfo.CurrentCulture);

	/// <summary>
	/// Formats the value of the current instance.
	/// </summary>
	/// <param name="provider">An object that supplies culture-specific formatting information.</param>
	/// <returns>The value of the current instance in the specified format.</returns>
	public readonly string ToString(IFormatProvider? provider)
		=> ToString(null, provider);

	/// <summary>
	/// Returns if <paramref name="date"/> is included in the range.
	/// </summary>
	/// <param name="date">The date to check for inclusion within the current range.</param>
	public readonly bool Contains(DateOnly date)
		=> Start <= date && date <= End;

	/// <summary>
	/// Returns if <paramref name="other"/> is included in the range.
	/// </summary>
	/// <param name="other">The other range to check for inclusion within the current range.</param>
	public readonly bool Contains(DateRange other)
		=> Contains(other.Start) && Contains(other.End);

	/// <summary>
	/// Returns if <paramref name="other"/> intersects with the range.
	/// </summary>
	/// <param name="other">The other range to check for intersection with the current range.</param>
	public readonly bool Overlaps(DateRange other)
		=> Contains(other.Start) || Contains(other.End) || other.Contains(Start);

	/// <summary>
	/// Returns intersection of current range and <paramref name="other"/> range.
	/// </summary>
	/// <param name="other">The other range to intersect with the current range.</param>
	public readonly DateRange Intersect(DateRange other)
		=> new(DateOnly.Max(Start, other.Start), DateOnly.Min(End, other.End));

	/// <summary>
	/// Excludes specified period from current period.
	/// </summary>
	/// <param name="range">Range to exclude.</param>
	/// <returns>List of periods. Will be empty if hole period is excluded.</returns>
	public readonly IEnumerable<DateRange> Exclude(DateRange range)
		=> Exclude([range]);

	/// <summary>
	/// Excludes specified <paramref name="ranges"/> from the range.
	/// </summary>
	/// <param name="ranges">Ranges to exclude.</param>
	/// <returns>List of date ranges. Will be empty if hole range is excluded.</returns>
	public readonly IEnumerable<DateRange> Exclude(IEnumerable<DateRange> ranges)
	{
		var res = this;
		foreach (var r in ranges.OrderBy(r => r.Start))
		{
			if (res.IsEmpty)
				break;

			if (r.Start <= res.Start && res.Start <= r.End)
				res.Start = r.End.AddDays(1);
			else if (r.Start <= res.End && res.End <= r.End)
				res.End = r.Start.AddDays(-1);
			else if (res.Start <= r.Start && r.End <= res.End)
			{
				if (res.Start < r.Start)
					yield return new DateRange(res.Start, r.Start.AddDays(-1));
				res.Start = r.End.AddDays(1);
			}
		}

		if (!res.IsEmpty)
			yield return res;
	}

	/// <summary>
	/// Converts the <see cref="DateRange"/> to an equivalent <see cref="NpgsqlRange{DateOnly}"/>.
	/// </summary>
	public readonly NpgsqlRange<DateOnly> ToNpgsql() => new(
		Start, true, IsStartInfinite,
		End, true, IsEndInfinite
	);

	/// <summary>
	/// Gets an empty <see cref="DateRange"/> instance.
	/// Empty range is infinite too.
	/// </summary>
	public static DateRange Empty { get; } = new(null, null);

	public static implicit operator DateRange(NpgsqlRange<DateOnly> range)
		=> range.ToRange();

	public static implicit operator NpgsqlRange<DateOnly>(DateRange range)
		=> range.ToNpgsql();

	public static implicit operator NpgsqlRange<DateOnly>(DateRange? range)
		=> range?.ToNpgsql() ?? NpgsqlRange<DateOnly>.Empty;

	public static bool operator ==(DateRange a, DateRange b)
		=> a.Start == b.Start && a.End == b.End;

	public static bool operator !=(DateRange a, DateRange b)
		=> a.Start != b.Start || a.End != b.End;

	public static bool operator <(DateRange a, DateRange b)
		=> a.Start < b.Start || (a.Start == b.Start && a.End < b.End);

	public static bool operator >(DateRange a, DateRange b)
		=> a.Start > b.Start || (a.Start == b.Start && a.End > b.End);

	public static bool operator <=(DateRange a, DateRange b)
		=> a.Start < b.Start || (a.Start == b.Start && a.End <= b.End);

	public static bool operator >=(DateRange a, DateRange b)
		=> a.Start > b.Start || (a.Start == b.Start && a.End >= b.End);

	/// <summary>
	/// Creates a new <see cref="DateRange"/> for the specified month.
	/// </summary>
	/// <param name="year">The year of the month.</param>
	/// <param name="month">The month.</param>
	/// <returns>A <see cref="DateRange"/> representing the specified month.</returns>
	public static DateRange Month(int year, int month)
		=> new(new(year, month, 1), new(year, month, DateTime.DaysInMonth(year, month)));

	public static DateRange Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
	{
		if (DateOnly.TryParseYearMonth(s, out var month))
			return new(month);
		if (DateOnly.TryParse(s, provider, out var date))
			return new(date);

		int index = s.IndexOf(Separator);
		if (index == -1)
			throw new FormatException($"DateRange string must contain separator '{Separator}'.");
		return new(
			DateOnly.Parse(s[..index], provider),
			DateOnly.Parse(s[(index + 1)..], provider)
		);
	}

	/// <summary>
	/// Parses a span of characters into a value.
	/// </summary>
	/// <param name="s">The span of characters to parse.</param>
	/// <exception cref="T:System.FormatException">
	/// <paramref name="s" /> is not in the correct format.</exception>
	/// <exception cref="T:System.OverflowException">
	/// <paramref name="s" /> is not representable by <see cref="DateRange"/>.</exception>
	/// <returns>The result of parsing <paramref name="s" />.</returns>
	public static DateRange Parse(ReadOnlySpan<char> s)
		=> Parse(s, CultureInfo.InvariantCulture);

	public static DateRange Parse(string s, IFormatProvider? provider)
		=> Parse(s.AsSpan(), provider);

	/// <summary>
	/// Parses a string into a value.
	/// </summary>
	/// <param name="s">The string to parse.</param>
	/// <exception cref="T:System.ArgumentNullException">
	/// <paramref name="s" /> is <see langword="null" />.</exception>
	/// <exception cref="T:System.FormatException">
	/// <paramref name="s" /> is not in the correct format.</exception>
	/// <exception cref="T:System.OverflowException">
	/// <paramref name="s" /> is not representable by <see cref="DateRange"/>.</exception>
	/// <returns>The result of parsing <paramref name="s" />.</returns>
	public static DateRange Parse(string s)
		=> Parse(s.AsSpan(), CultureInfo.InvariantCulture);

	public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out DateRange result)
	{
		if (DateOnly.TryParseYearMonth(s, out var month))
		{
			result = Month(month.Year, month.Month);
			return true;
		}
		if (DateOnly.TryParse(s, provider, out var date))
		{
			result = new(date);
			return true;
		}
		if (!s.IsEmpty
			&& s.IndexOfAny(Separator, '–') is int index and not -1
			&& DateOnly.TryParse(s[..index].Trim(), provider, out var start)
			&& DateOnly.TryParse(s[(index + 1)..].Trim(), provider, out var end))
		{
			result = new(start, end);
			return true;
		}
		result = default;
		return false;
	}

	/// <summary>
	/// Tries to parse a span of characters into a value.
	/// </summary>
	/// <param name="s">The span of characters to parse.</param>
	/// <param name="result">When this method returns, contains the result of successfully parsing <paramref name="s" />, or an undefined value on failure.</param>
	/// <returns>
	/// <see langword="true" /> if <paramref name="s" /> was successfully parsed; otherwise, <see langword="false" />.</returns>
	public static bool TryParse(ReadOnlySpan<char> s, out DateRange result)
		=> TryParse(s, CultureInfo.InvariantCulture, out result);

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out DateRange result)
		=> TryParse(s.AsSpan(), provider, out result);

	/// <summary>
	/// Tries to parse a string into a value.
	/// </summary>
	/// <param name="s">The string to parse.</param>
	/// <param name="result">When this method returns, contains the result of successfully parsing <paramref name="s" /> or an undefined value on failure.</param>
	/// <returns>
	/// <see langword="true" /> if <paramref name="s" /> was successfully parsed; otherwise, <see langword="false" />.</returns>
	public static bool TryParse([NotNullWhen(true)] string? s, out DateRange result)
		=> TryParse(s.AsSpan(), CultureInfo.InvariantCulture, out result);
}