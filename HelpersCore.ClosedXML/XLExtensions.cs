using System.Reflection;
using ClosedXML.Attributes;
using ClosedXML.Excel;

namespace HelpersCore;

/// <summary>
/// Provides extension methods for working with ClosedXML.
/// </summary>
public static class XLExtensions
{
	/// <summary>
	/// Saves <see cref="XLWorkbook"/> as a <see cref="MemoryStream"/>.
	/// </summary>
	/// <param name="workbook">Workbook to save.</param>
	/// <param name="validate"><c>True</c> to validate workbook before save.</param>
	/// <param name="evaluateFormulas">
	/// <c>True</c> to evaluate formulas and the calculated values are saved to the file.
	/// <c>False</c> (default) – formulas are not evaluated and the formula cells don't have their values saved to the file.
	/// </param>
	public static MemoryStream SaveAsStream(this XLWorkbook workbook, bool validate = true, bool evaluateFormulas = false)
	{
		MemoryStream ms = new();
		workbook.SaveAs(ms, validate, evaluateFormulas);
		ms.Seek(0, SeekOrigin.Begin);
		return ms;
	}

	/// <summary>
	/// Saves <see cref="XLWorkbook"/> as a <see cref="MemoryStream"/>.
	/// </summary>
	/// <param name="workbook">Workbook to save.</param>
	/// <param name="options">Save options.</param>
	public static MemoryStream SaveAsStream(this XLWorkbook workbook, SaveOptions options)
	{
		MemoryStream ms = new();
		workbook.SaveAs(ms, options);
		ms.Seek(0, SeekOrigin.Begin);
		return ms;
	}

	/// <summary>
	/// Returns range from current cell to the <paramref name="lastCell"/>.
	/// </summary>
	/// <param name="cell">Range start cell.</param>
	/// <param name="lastCell">Range last cell.</param>
	public static IXLRange RangeTo(this IXLCell cell, IXLCell lastCell)
		=> cell.Worksheet.Range(cell, lastCell);

	/// <summary>
	/// Extends the range to the right by the specified number of columns.
	/// </summary>
	/// <param name="range">Cell range to extend.</param>
	/// <param name="step">Number of columns to extend range for.</param>
	public static IXLRange GrowRight(this IXLRange range, int step)
		=> range.FirstCell().RangeTo(range.LastCell().CellRight(step));

	/// <summary>
	/// Extends the range down by the specified number of rows.
	/// </summary>
	/// <param name="range">Cell range to extend.</param>
	/// <param name="step">Number of rows to extend range for.</param>
	public static IXLRange GrowDown(this IXLRange range, int step)
		=> range.FirstCell().RangeTo(range.LastCell().CellBelow(step));

	/// <summary>
	/// Returns range by extending the cell to the right by the specified number of columns.
	/// </summary>
	/// <param name="cell">Range starting cell.</param>
	/// <param name="step">Number of columns to extend range for.</param>
	public static IXLRange RangeRight(this IXLCell cell, int step)
		=> cell.AsRange().GrowRight(step);

	/// <summary>
	/// Returns range by extending the cell down by the specified number of rows.
	/// </summary>
	/// <param name="cell">Range starting cell.</param>
	/// <param name="step">Number of rows to extend range for.</param>
	public static IXLRange RangeDown(this IXLCell cell, int step)
		=> cell.AsRange().GrowDown(step);

	/// <summary>
	/// Returns the top-right cell of the specified range.
	/// </summary>
	public static IXLCell TopRightCell(this IXLRange range)
		=> range.LastColumn().FirstCell();

	/// <summary>
	/// Returns the bottom-left cell of the specified range.
	/// </summary>
	public static IXLCell BottomLeftCell(this IXLRange range)
		=> range.FirstColumn().LastCell();

	/// <summary>
	/// Inserts the <paramref name="data"/> elements as a table and returns it.
	/// Applies extended styles from <see cref="XLColumnExAttribute"/>.
	/// The new table will receive a generic name: Table#.
	/// </summary>
	/// <param name="cell">Table starting cell.</param>
	/// <param name="data">Table data.</param>
	/// <param name="headerAlignment">If specified, overrides header alignment.</param>
	/// <typeparam name="T">Table data type.</typeparam>
	public static IXLTable InsertTableStyled<T>(this IXLCell cell, IEnumerable<T> data, XLAlignmentHorizontalValues? headerAlignment = null)
	{
		var table = cell.InsertTable(data);
		table.ApplyStylesFromExtendedAttributes<T>(headerAlignment);
		return table;
	}

	/// <summary>
	/// Inserts the <paramref name="data"/> elements as a table and returns it.
	/// Applies extended styles from <see cref="XLColumnExAttribute"/>.
	/// The new table will receive a generic name: Table#.
	/// </summary>
	/// <param name="cell">Table starting cell.</param>
	/// <param name="data">Table data.</param>
	/// <param name="createTable">
	/// If set to <c>true</c> it will create an Excel table.
	/// If set to <c>false</c> the table will be created in memory.
	/// </param>
	/// <param name="headerAlignment">If specified, overrides header alignment.</param>
	/// <typeparam name="T">Table data type.</typeparam>
	public static IXLTable InsertTableStyled<T>(this IXLCell cell, IEnumerable<T> data, bool createTable, XLAlignmentHorizontalValues? headerAlignment = null)
	{
		var table = cell.InsertTable(data, createTable);
		table.ApplyStylesFromExtendedAttributes<T>(headerAlignment);
		return table;
	}

	/// <summary>
	/// Inserts the <paramref name="data"/> elements as a table and returns it.
	/// Applies extended styles from <see cref="XLColumnExAttribute"/>.
	/// </summary>
	/// <param name="cell">Table starting cell.</param>
	/// <param name="data">Table data.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="headerAlignment">If specified, overrides header alignment.</param>
	/// <typeparam name="T">Table data type.</typeparam>
	public static IXLTable InsertTableStyled<T>(this IXLCell cell, IEnumerable<T> data, string tableName, XLAlignmentHorizontalValues? headerAlignment = null)
	{
		var table = cell.InsertTable(data, tableName);
		table.ApplyStylesFromExtendedAttributes<T>(headerAlignment);
		return table;
	}

	/// <summary>
	/// Inserts the <paramref name="data"/> elements as a table and returns it.
	/// Applies extended styles from <see cref="XLColumnExAttribute"/>.
	/// </summary>
	/// <param name="cell">Table starting cell.</param>
	/// <param name="data">Table data.</param>
	/// <param name="tableName">Name of the table.</param>
	/// <param name="createTable">
	/// If set to <c>true</c> it will create an Excel table.
	/// If set to <c>false</c> the table will be created in memory.
	/// </param>
	/// <param name="headerAlignment">If specified, overrides header alignment.</param>
	/// <typeparam name="T">Table data type.</typeparam>
	public static IXLTable InsertTableStyled<T>(this IXLCell cell, IEnumerable<T> data, string tableName, bool createTable, XLAlignmentHorizontalValues? headerAlignment = null)
	{
		var table = cell.InsertTable(data, tableName, createTable);
		table.ApplyStylesFromExtendedAttributes<T>(headerAlignment);
		return table;
	}

	/// <summary>
	/// Applies styles from <see cref="XLColumnExAttribute"/> attributes to the columns of the specified range.
	/// </summary>
	/// <typeparam name="T">Data type to get properties with <see cref="XLColumnExAttribute"/> attribute from.</typeparam>
	static void ApplyStylesFromExtendedAttributes<T>(this IXLRange range, XLAlignmentHorizontalValues? headerAlignment)
	{
		foreach (var (index, (_, attr)) in typeof(T)
			.GetProperties(BindingFlags.Instance | BindingFlags.Public)
			.Select(p => (Prop: p, Attr: p.GetCustomAttribute<XLColumnAttribute>()))
			.Where(r => r.Attr?.Ignore != true)
			.OrderBy(r => r.Attr?.Order ?? 0)
			.Index()
		) {
			if (attr is not XLColumnExAttribute attr2)
				continue;

			var column = range.Column(index + 1);
			var style = column.Style;
			style.Alignment.SetHorizontal(attr2.Horizontal);
			if (attr2.FormatId != -1)
				style.NumberFormat.SetNumberFormatId(attr2.FormatId);
			else if (attr2.Format != null)
				style.NumberFormat.SetFormat(attr2.Format);
			if (attr2.WrapText)
				style.Alignment.SetWrapText();

			if (attr2.HyperlinkLabel is string label)
			{
				foreach (var cell in column.Cells().Skip(1))
				{
					string url = cell.GetValue<string>();
					cell.SetValue(Blank.Value);
					if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out var uri))
					{
						cell.SetValue(label);
						cell.CreateHyperlink().ExternalAddress = uri;
					}
				}
			}
		}

		if (headerAlignment is { } alignment)
			range.Row(1).Style.Alignment.Horizontal = alignment;
	}

	/// <summary>
	/// Sets centered document title (bold) and subtitles (normal) into the Excel workbook.
	/// </summary>
	/// <param name="cell">Starting cell to write to.</param>
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
	/// <param name="cell">Starting cell to write to.</param>
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