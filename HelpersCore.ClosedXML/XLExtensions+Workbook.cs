using ClosedXML.Excel;

namespace HelpersCore;

/// <summary>
/// Provides extension methods for working with ClosedXML.
/// </summary>
public static partial class XLExtensions
{
	extension(XLWorkbook workbook)
	{
		/// <summary>
		/// Saves <see cref="XLWorkbook"/> as a <see cref="MemoryStream"/>.
		/// </summary>
		/// <param name="validate"><c>True</c> to validate workbook before save.</param>
		/// <param name="evaluateFormulas">
		/// <c>True</c> to evaluate formulas and the calculated values are saved to the file.
		/// <c>False</c> (default) – formulas are not evaluated and the formula cells don't have their values saved to the file.
		/// </param>
		public MemoryStream SaveAsStream(bool validate = true, bool evaluateFormulas = false)
		{
			MemoryStream ms = new();
			workbook.SaveAs(ms, validate, evaluateFormulas);
			ms.Seek(0, SeekOrigin.Begin);
			return ms;
		}

		/// <summary>
		/// Saves <see cref="XLWorkbook"/> as a <see cref="MemoryStream"/>.
		/// </summary>
		/// <param name="options">Save options.</param>
		public MemoryStream SaveAsStream(SaveOptions options)
		{
			MemoryStream ms = new();
			workbook.SaveAs(ms, options);
			ms.Seek(0, SeekOrigin.Begin);
			return ms;
		}
	}
}