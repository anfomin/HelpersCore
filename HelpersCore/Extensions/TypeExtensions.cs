using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="Type"/> and <see cref="PropertyInfo"/>.
/// </summary>
public static class TypeExtensions
{
	static readonly ConcurrentDictionary<Type, string> TypeNames = new();
	static readonly ConcurrentDictionary<PropertyInfo, string> PropertyNames = new();

	extension(Type type)
	{
		/// <summary>
		/// Returns if type is <see cref="Nullable"/>.
		/// </summary>
		public bool IsNullable
			=> !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;

		/// <summary>
		/// Determines if type is simple.
		/// Simple type is one of: all primitives, enums, <see cref="string"/>, <see cref="decimal"/>, <see cref="Guid"/>, <see cref="DateTime"/>, <see cref="DateOnly"/>, <see cref="TimeOnly"/>, <see cref="TimeSpan"/> and their nullable types.
		/// </summary>
		public bool IsSimpleType
		{
			get
			{
				type = Nullable.GetUnderlyingType(type) ?? type;
				return type.IsPrimitive || type.IsEnum ||
					type.OneOf(typeof(string), typeof(decimal), typeof(Guid), typeof(DateTime), typeof(DateOnly), typeof(TimeOnly), typeof(TimeSpan));
			}
		}

		/// <summary>
		/// Determines if type is <see cref="Enum"/> and contains <see cref="FlagsAttribute"/>.
		/// </summary>
		public bool IsEnumFlags
			=> type.IsEnum && type.GetCustomAttribute<FlagsAttribute>(true) is not null;

		/// <summary>
		/// Gets type display name. If <see cref="DisplayAttribute"/> or <see cref="DisplayNameAttribute"/> is not set then uses type name.
		/// </summary>
		public string DisplayName => TypeNames.GetOrAdd(type,
			t => t.GetCustomAttribute<DisplayAttribute>()?.Name
				?? t.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
				?? t.Name
		);

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
			return prop?.DisplayName ?? throw new MissingFieldException(type.FullName, propertyPath);
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
		/// Returns if <see cref="Type"/> is included in <paramref name="others"/>.
		/// </summary>
		public bool OneOf(params IEnumerable<Type> others)
			=> others.Contains(type);
	}

	extension(PropertyInfo property)
	{
		/// <summary>
		/// Gets property display name. If <see cref="DisplayAttribute"/> or <see cref="DisplayNameAttribute"/> is not set then uses property name.
		/// </summary>
		public string DisplayName => PropertyNames.GetOrAdd(property,
			p => p.GetCustomAttribute<DisplayAttribute>()?.Name
				?? p.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
				?? p.Name
		);
	}
}