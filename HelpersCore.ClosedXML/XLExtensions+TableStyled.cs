using System.Reflection;
using ClosedXML.Attributes;
using ClosedXML.Excel;

namespace HelpersCore;

public static partial class XLExtensions
{
	extension(IXLCell cell)
	{
		/// <summary>
		/// Inserts the <paramref name="data"/> elements as a table and returns it.
		/// Applies extended styles from <see cref="XLColumnExAttribute"/>.
		/// The new table will receive a generic name: Table#.
		/// </summary>
		/// <param name="data">Table data.</param>
		/// <param name="headerAlignment">If specified, overrides header alignment.</param>
		/// <typeparam name="T">Table data type.</typeparam>
		public IXLTable InsertTableStyled<T>(IEnumerable<T> data, XLAlignmentHorizontalValues? headerAlignment = null)
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
		/// <param name="data">Table data.</param>
		/// <param name="createTable">
		/// If set to <c>true</c> it will create an Excel table.
		/// If set to <c>false</c> the table will be created in memory.
		/// </param>
		/// <param name="headerAlignment">If specified, overrides header alignment.</param>
		/// <typeparam name="T">Table data type.</typeparam>
		public IXLTable InsertTableStyled<T>(IEnumerable<T> data, bool createTable, XLAlignmentHorizontalValues? headerAlignment = null)
		{
			var table = cell.InsertTable(data, createTable);
			table.ApplyStylesFromExtendedAttributes<T>(headerAlignment);
			return table;
		}

		/// <summary>
		/// Inserts the <paramref name="data"/> elements as a table and returns it.
		/// Applies extended styles from <see cref="XLColumnExAttribute"/>.
		/// </summary>
		/// <param name="data">Table data.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="headerAlignment">If specified, overrides header alignment.</param>
		/// <typeparam name="T">Table data type.</typeparam>
		public IXLTable InsertTableStyled<T>(IEnumerable<T> data, string tableName, XLAlignmentHorizontalValues? headerAlignment = null)
		{
			var table = cell.InsertTable(data, tableName);
			table.ApplyStylesFromExtendedAttributes<T>(headerAlignment);
			return table;
		}

		/// <summary>
		/// Inserts the <paramref name="data"/> elements as a table and returns it.
		/// Applies extended styles from <see cref="XLColumnExAttribute"/>.
		/// </summary>
		/// <param name="data">Table data.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="createTable">
		/// If set to <c>true</c> it will create an Excel table.
		/// If set to <c>false</c> the table will be created in memory.
		/// </param>
		/// <param name="headerAlignment">If specified, overrides header alignment.</param>
		/// <typeparam name="T">Table data type.</typeparam>
		public IXLTable InsertTableStyled<T>(IEnumerable<T> data, string tableName, bool createTable, XLAlignmentHorizontalValues? headerAlignment = null)
		{
			var table = cell.InsertTable(data, tableName, createTable);
			table.ApplyStylesFromExtendedAttributes<T>(headerAlignment);
			return table;
		}
	}

	/// <summary>
	/// Applies styles from <see cref="XLColumnExAttribute"/> attributes to the columns of the specified range.
	/// </summary>
	/// <typeparam name="T">Data type to get properties with <see cref="XLColumnExAttribute"/> attribute from.</typeparam>
	static void ApplyStylesFromExtendedAttributes<T>(this IXLRange range, XLAlignmentHorizontalValues? headerAlignment)
	{
		var columns = typeof(T)
			.GetProperties(BindingFlags.Instance | BindingFlags.Public)
			.Select(p => (Prop: p, Attr: p.GetCustomAttribute<XLColumnAttribute>()))
			.Where(r => r.Attr?.Ignore != true)
			.OrderBy(r => r.Attr?.Order ?? 0)
			.ToList();
		foreach (var (index, (_, attr)) in columns.Index())
		{
			if (attr is not XLColumnExAttribute attr2)
				continue;

			var column = range.Column(index + 1);
			var style = column.Style;
			style.Alignment.SetHorizontal(attr2.Horizontal);
			if (attr2.FormatId != -1)
				style.NumberFormat.SetNumberFormatId(attr2.FormatId);
			else if (attr2.Format is { } format)
				style.NumberFormat.SetFormat(format);
			if (attr2.WrapText)
				style.Alignment.SetWrapText();

			if (attr2.FormulaR1C1 is { } formula)
			{
				foreach (var cell in column.Cells().Skip(1))
					cell.SetFormulaR1C1(formula);
			}

			if (attr2.HyperlinkLabel is { } label)
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
		foreach (var (index, (_, attr)) in columns.Index())
		{
			if (attr is not XLColumnExAttribute attr2)
				continue;

			var col = range.Column(index + 1).WorksheetColumn();
			if (attr2.WidthFit)
			{
				if (attr2.Width > 0)
					col.AdjustToContents(Math.Max(attr2.WidthMin, .0), attr2.Width);
				else
					col.AdjustToContents();
			}
			else if (attr2.Width > 0)
				col.Width = attr2.Width;
		}
	}
}