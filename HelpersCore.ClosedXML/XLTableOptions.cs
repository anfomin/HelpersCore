using ClosedXML.Excel;

namespace HelpersCore;

/// <summary>
/// Provides options for Excel table creation.
/// </summary>
public record XLTableOptions<T>
{
	/// <summary>
	/// Header horizontal alignment.
	/// </summary>
	public XLAlignmentHorizontalValues? HeaderHorizontal { get; init; }

	/// <summary>
	/// Header vertical alignment.
	/// </summary>
	public XLAlignmentVerticalValues? HeaderVertical { get; init; }

	/// <summary>
	/// Data rows vertical alignment.
	/// </summary>
	public XLAlignmentVerticalValues? DataVertical { get; init; }

	/// <summary>
	/// Action invoked for each data row to customize row cells.
	/// </summary>
	public Action<T, IXLRangeRow, IReadOnlyList<XLTableColumn>>? RowAction { get; init; }
}