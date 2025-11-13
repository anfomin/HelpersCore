using ClosedXML.Excel;

namespace HelpersCore;

public static partial class XLExtensions
{
	/// <summary>
	/// Sets centered document title (bold) and subtitles (normal) into the Excel workbook.
	/// </summary>
	/// <param name="columns">Number of columns title should occupy.</param>
	/// <param name="fontSize">title font size.</param>
	/// <param name="title">Title text.</param>
	/// <param name="subtitles">Optional subtitles.</param>
	/// <returns>Title and subtitles range.</returns>
	public static IXLRange SetDocumentTitle(this IXLCell cell, int columns, double fontSize, string title, params IEnumerable<string> subtitles)
	{
		int grow = columns - 1;
		var initial = cell;
		cell.RangeRight(grow).Merge()
			.SetValue(title)
			.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
			.Font.SetFontSize(fontSize)
			.Font.SetBold();
		foreach (string subtitle in subtitles)
		{
			cell = cell.CellBelow();
			cell.RangeRight(grow).Merge()
				.SetValue(subtitle)
				.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
				.Font.SetFontSize(fontSize - 1);
		}
		return initial.RangeTo(cell.CellRight(grow));
	}

	/// <summary>
	/// Sets centered document title (bold, 13px) and subtitles (normal, 12px) into the Excel workbook.
	/// </summary>
	/// <param name="columns">Number of columns title should occupy.</param>
	/// <param name="title">Title text.</param>
	/// <param name="subtitles">Optional subtitles.</param>
	/// <returns>Title and subtitles range.</returns>
	public static IXLRange SetDocumentTitle(this IXLCell cell, int columns, string title, params IEnumerable<string> subtitles)
		=> SetDocumentTitle(cell, columns, 13, title, subtitles);

	/// <summary>
	/// Sets a gap in the Excel worksheet by setting the row height.
	/// </summary>
	/// <param name="height">Row height.</param>
	public static IXLCell SetRowGap(this IXLCell cell, int height = 24)
	{
		cell.WorksheetRow().Height = height;
		return cell;
	}
}