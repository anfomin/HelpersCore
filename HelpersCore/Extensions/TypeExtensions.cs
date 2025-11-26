using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="Type"/> and <see cref="PropertyInfo"/>.
/// </summary>
public static class TypeExtensions
{
	/// <summary>
	/// Returns if <see cref="Type"/> is included in <paramref name="others"/>.
	/// </summary>
	public static bool OneOf(this Type type, params IEnumerable<Type> others)
		=> others.Contains(type);

	extension(Type type)
	{
		/// <summary>
		/// Returns if type is <see cref="Nullable"/>.
		/// </summary>
		public bool IsNullable()
			=> !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;

		/// <summary>
		/// Determines if type is simple.
		/// Simple type is one of: all primitives, enums, <see cref="string"/>, <see cref="decimal"/>, <see cref="Guid"/>, <see cref="DateTime"/>, <see cref="DateOnly"/>, <see cref="TimeOnly"/>, <see cref="TimeSpan"/> and their nullable types.
		/// </summary>
		public bool IsSimpleType()
		{
			type = Nullable.GetUnderlyingType(type) ?? type;
			return type.IsPrimitive || type.IsEnum || type.OneOf(typeof(string), typeof(decimal), typeof(Guid), typeof(DateTime), typeof(DateOnly), typeof(TimeOnly), typeof(TimeSpan));
		}

		/// <summary>
		/// Gets type display name. If <see cref="DisplayAttribute"/> or <see cref="DisplayNameAttribute"/> is not set then uses type name.
		/// </summary>
		public string GetDisplayName()
			=> type.GetCustomAttribute<DisplayAttribute>()?.Name
				?? type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
				?? type.Name;

		/// <summary>
		/// Gets property display name. If <see cref="DisplayAttribute"/> or <see cref="DisplayNameAttribute"/> is not set then uses property name.
		/// </summary>
		/// <param name="propertyPath">Path to property separated by '.' or '?.'.</param>
		public string GetDisplayName(string propertyPath)
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
		public object? GetDefaultValue()
			=> type.IsValueType
				? Activator.CreateInstance(type)
				: null;

		/// <summary>
		/// Determines if type is <see cref="Enum"/> and contains <see cref="FlagsAttribute"/>.
		/// </summary>
		public bool IsEnumFlags()
			=> type.IsEnum && type.GetCustomAttribute<FlagsAttribute>(true) is not null;
	}

	extension(PropertyInfo property)
	{
		/// <summary>
		/// Gets property display name. If <see cref="DisplayAttribute"/> or <see cref="DisplayNameAttribute"/> is not set then uses property name.
		/// </summary>
		public string GetDisplayName()
			=> property.GetCustomAttribute<DisplayAttribute>()?.Name
				?? property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
				?? property.Name;
	}
}