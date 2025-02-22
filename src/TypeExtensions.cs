using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HelpersCore;

public static class TypeExtensions
{
	/// <summary>
	/// Returns if type is included in <paramref name="others">.
	/// </summary>
	public static bool OneOf(this Type value, params IEnumerable<Type> others)
		=> others.Contains(value);

	/// <summary>
	/// Returns if type == nullable.
	/// </summary>
	public static bool IsNullable(this Type type)
		=> !type.IsValueType || Nullable.GetUnderlyingType(type) != null;

	/// <summary>
	/// Determines if type is simple.
	/// Simple type is one of: all primitives, enums, <see cref="string"/>, <see cref="decimal"/>, <see cref="Guid"/>, <see cref="DateTime"/>, <see cref="DateOnly"/>, <see cref="TimeOnly"/>, <see cref="TimeSpan"/> and their nullable types.
	/// </summary>
	public static bool IsSimpleType(this Type type)
	{
		type = Nullable.GetUnderlyingType(type) ?? type;
		return type.IsPrimitive || type.IsEnum || type.OneOf(typeof(string), typeof(decimal), typeof(Guid), typeof(DateTime), typeof(DateOnly), typeof(TimeOnly), typeof(TimeSpan));
	}

	/// <summary>
	/// Gets type display name. If <see cref="DisplayAttribute"/> or <see cref="DisplayNameAttribute"/> is not set then uses type name.
	/// </summary>
	public static string GetDisplayName(this Type type)
		=> type.GetCustomAttribute<DisplayAttribute>()?.Name
		?? type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
		?? type.Name;

	/// <summary>
	/// Gets property display name. If <see cref="DisplayAttribute"/> or <see cref="DisplayNameAttribute"/> is not set then uses property name.
	/// </summary>
	public static string GetDisplayName(this PropertyInfo propInfo)
		=> propInfo.GetCustomAttribute<DisplayAttribute>()?.Name
		?? propInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
		?? propInfo.Name;

	/// <summary>
	/// Gets property display name. If <see cref="DisplayAttribute"/> or <see cref="DisplayNameAttribute"/> is not set then uses property name.
	/// </summary>
	/// <param name="propertyPath">Path to property separated by '.' or '?.'.
	public static string GetDisplayName(this Type type, string propertyPath)
	{
		PropertyInfo? prop = null;
		foreach (string key in propertyPath.Split(["?.", "."], StringSplitOptions.None))
		{
			var prevType = prop?.PropertyType ?? type;
			prop = prevType.GetProperty(key) ?? throw new MissingFieldException(prevType.FullName, key);
		}
		return prop?.GetDisplayName() ?? throw new MissingFieldException(type.FullName, propertyPath);
	}

	/// <summary>
	/// Gets default value of the type.
	/// This is runtime equivalent to <c>default</c>.
	/// </summary>
	public static object? GetDefaultValue(this Type type)
		=> type.IsValueType
		? Activator.CreateInstance(type)
		: null;

	/// <summary>
	/// Determines if type is <see cref="Enum"/> and contains <see cref="FlagsAttribute"/>.
	/// </summary>
	public static bool IsEnumFlags(this Type type)
		=> type.IsEnum && type.GetCustomAttribute<FlagsAttribute>(true) != null;
}