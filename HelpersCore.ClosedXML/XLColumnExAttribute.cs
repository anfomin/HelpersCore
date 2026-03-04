using ClosedXML.Attributes;
using ClosedXML.Excel;

namespace HelpersCore;

/// <summary>
/// Provides extended information for Excel column.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class XLColumnExAttribute : XLColumnAttribute
{
	/// <summary>
	/// Gets or sets cell horizontal alignment.
	/// </summary>
	public XLAlignmentHorizontalValues Horizontal { get; set; } = XLAlignmentHorizontalValues.General;

	/// <summary>
	/// Gets or sets cell built-in number format identifier.
	/// </summary>
	public int FormatId { get; set; } = -1;

	/// <summary>
	/// Gets or sets cell custom format string.
	/// </summary>
	public string? Format { get; set; }

	/// <summary>
	/// Gets or sets if cell text should be wrapped.
	/// </summary>
	public bool WrapText { get; set; }

	/// <summary>
	/// Gets or sets cell hyperlink label.
	/// If <c>true</c> then property content meant to be an external hyperlink.
	/// </summary>
	public string? HyperlinkLabel { get; set; }

	/// <summary>
	/// Gets or sets if column width should be adjusted to fit content.
	/// </summary>
	public bool WidthFit { get; set; }

	/// <summary>
	/// Gets or sets column width. If <see cref="WidthFit"/> is <c>true</c> then this value is used as maximum width.
	/// </summary>
	public int Width { get; set; }

	/// <summary>
	/// Gets or sets column minimum width if <see cref="WidthFit"/> is <c>true</c>.
	/// </summary>
	public int WidthMin { get; set; }

	/// <summary>
	/// Gets or sets cell formula in R1C1 format.
	/// </summary>
	public string? FormulaR1C1 { get; set; }

	/// <summary>
	/// Initializes new instance of <see cref="XLColumnExAttribute"/>.
	/// </summary>
	public XLColumnExAttribute() { }

	/// <summary>
	/// Initializes new instance of <see cref="XLColumnExAttribute"/> with specified header.
	/// </summary>
	/// <param name="header">Column header.</param>
	public XLColumnExAttribute(string header)
	{
		Header = header;
	}
}