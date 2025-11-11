using ClosedXML.Excel;

namespace HelpersCore;

public static partial class XLExtensions
{
	extension(IXLCell cell)
	{
		/// <summary>
		/// Returns range from current cell to the <paramref name="lastCell"/>.
		/// </summary>
		/// <param name="lastCell">Range last cell.</param>
		public IXLRange RangeTo(IXLCell lastCell)
			=> cell.Worksheet.Range(cell, lastCell);

		/// <summary>
		/// Returns range by extending current cell to the right by the specified number of columns.
		/// </summary>
		/// <param name="step">Number of columns to extend range for.</param>
		public IXLRange RangeRight(int step)
			=> cell.AsRange().GrowRight(step);

		/// <summary>
		/// Returns range by extending current cell down by the specified number of rows.
		/// </summary>
		/// <param name="step">Number of rows to extend range for.</param>
		public IXLRange RangeDown(int step)
			=> cell.AsRange().GrowDown(step);
	}

	extension(IXLRange range)
	{
		/// <summary>
		/// Extends the range to the right by the specified number of columns.
		/// </summary>
		/// <param name="step">Number of columns to extend range for.</param>
		public IXLRange GrowRight(int step)
			=> range.FirstCell().RangeTo(range.LastCell().CellRight(step));

		/// <summary>
		/// Extends the range down by the specified number of rows.
		/// </summary>
		/// <param name="step">Number of rows to extend range for.</param>
		public IXLRange GrowDown(int step)
			=> range.FirstCell().RangeTo(range.LastCell().CellBelow(step));

		/// <summary>
		/// Returns the top-right cell of the specified range.
		/// </summary>
		public IXLCell TopRightCell()
			=> range.LastColumn().FirstCell();

		/// <summary>
		/// Returns the bottom-left cell of the specified range.
		/// </summary>
		public IXLCell BottomLeftCell()
			=> range.FirstColumn().LastCell();
	}
}