using System.Reflection;
using ClosedXML.Attributes;

namespace HelpersCore;

/// <summary>
/// Provides helper methods for working with Excel tables and columns.
/// </summary>
public static class XLTableHelper
{
	/// <summary>
	/// Returns column information for the specified type <typeparamref name="T"/>.
	/// </summary>
	public static IEnumerable<XLTableColumn> GetColumns<T>() => typeof(T)
		.GetProperties(BindingFlags.Instance | BindingFlags.Public)
		.Select(p => (Property: p, Attribute: p.GetCustomAttribute<XLColumnAttribute>()))
		.Where(r => r.Attribute?.Ignore != true)
		.OrderBy(r => r.Attribute?.Order ?? 0)
		.Select((r, index) => new XLTableColumn(index, r.Property, r.Attribute));
}

/// <summary>
/// Represents information about a column in an Excel table.
/// </summary>
/// <param name="Index">Column index.</param>
/// <param name="Property">Column property.</param>
/// <param name="Attribute">Attribute with additional information.</param>
public readonly record struct XLTableColumn(int Index, PropertyInfo Property, XLColumnAttribute? Attribute)
{
	/// <summary>
	/// Gets column name.
	/// </summary>
	public string Name => Property.Name;

	public override string ToString()
		=> Attribute?.Header ?? Property.GetDisplayName();
}