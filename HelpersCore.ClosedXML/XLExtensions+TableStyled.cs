using System.Linq.Expressions;
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
		/// <param name="options">Provides table options.</param>
		/// <typeparam name="T">Table data type.</typeparam>
		public IXLTable InsertTableStyled<T>(IEnumerable<T> data, XLTableOptions<T>? options = null)
		{
			var table = cell.InsertTable(data);
			table.ApplyStylesFromExtendedAttributes(data, options);
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
		/// <param name="options">Provides table options.</param>
		/// <typeparam name="T">Table data type.</typeparam>
		public IXLTable InsertTableStyled<T>(IEnumerable<T> data, bool createTable, XLTableOptions<T>? options = null)
		{
			var table = cell.InsertTable(data, createTable);
			table.ApplyStylesFromExtendedAttributes(data, options);
			return table;
		}

		/// <summary>
		/// Inserts the <paramref name="data"/> elements as a table and returns it.
		/// Applies extended styles from <see cref="XLColumnExAttribute"/>.
		/// </summary>
		/// <param name="data">Table data.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="options">Provides table options.</param>
		/// <typeparam name="T">Table data type.</typeparam>
		public IXLTable InsertTableStyled<T>(IEnumerable<T> data, string tableName, XLTableOptions<T>? options = null)
		{
			var table = cell.InsertTable(data, tableName);
			table.ApplyStylesFromExtendedAttributes(data, options);
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
		/// <param name="options">Provides table options.</param>
		/// <typeparam name="T">Table data type.</typeparam>
		public IXLTable InsertTableStyled<T>(IEnumerable<T> data, string tableName, bool createTable, XLTableOptions<T>? options = null)
		{
			var table = cell.InsertTable(data, tableName, createTable);
			table.ApplyStylesFromExtendedAttributes(data, options);
			return table;
		}
	}

	/// <summary>
	/// Applies styles from <see cref="XLColumnExAttribute"/> attributes to the columns of the specified range.
	/// </summary>
	/// <typeparam name="T">Data type to get properties with <see cref="XLColumnExAttribute"/> attribute from.</typeparam>
	static void ApplyStylesFromExtendedAttributes<T>(this IXLRange range, IEnumerable<T> data, XLTableOptions<T>? options)
	{
		var columns = XLTableHelper.GetColumns<T>().ToList();
		foreach (var (index, _, attr) in columns)
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

		foreach (var (index, _, attr) in columns)
		{
			if (attr is not XLColumnExAttribute attr2)
				continue;

			var col = range.Column(index + 1).WorksheetColumn();
			if (attr2.WidthFit)
			{
				if (attr2.Width > 0)
					col.AdjustToContents(Math.Max(attr2.WidthMin, 0), attr2.Width);
				else
					col.AdjustToContents();
			}
			else if (attr2.Width > 0)
				col.Width = attr2.Width;
		}

		if (options?.HeaderHorizontal is { } headerHorizontal)
			range.Row(1).Style.Alignment.SetHorizontal(headerHorizontal);
		if (options?.HeaderVertical is { } headerVertical)
			range.Row(1).Style.Alignment.SetVertical(headerVertical);
		if (options?.DataVertical is { } dataVertical)
			range.FirstCell().CellBelow().RangeTo(range.LastCell())
				.Style.Alignment.SetVertical(dataVertical);
		if (options?.RowAction is { } action)
		{
			foreach (var (index, item) in data.Index())
			{
				var row = range.Row(index + 2);
				action(item, row, columns);
			}
		}
	}

	/// <summary>
	/// Gets the cell for property specified by <paramref name="propertyExpression"/>.
	/// </summary>
	/// <param name="row">Table row.</param>
	/// <param name="columns">Table columns.</param>
	/// <param name="propertyExpression">Data item property accessor.</param>
	public static IXLCell? Cell<TProp>(this IXLRangeRow row, IReadOnlyList<XLTableColumn> columns, Expression<Func<TProp?>> propertyExpression)
		=> Expression.GetMember(propertyExpression) is { } member
			&& columns.FirstOrNull(c => c.Name.Equals(member.Name, StringComparison.InvariantCultureIgnoreCase)) is { } column
			? row.Cell(column.Index + 1)
			: null;
}