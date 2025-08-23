using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;

namespace HelpersCore;

/// <summary>
/// Extension methods for <see cref="Enum"/> types.
/// </summary>
public static class EnumExtensions
{
	static readonly Dictionary<Enum, bool> ReadOnly = new();
	static readonly Dictionary<Enum, bool> Ignore = new();

	/// <summary>
	/// Returns if <see cref="Enum"/> value has ReadOnlyAttribute with <c>true</c> value.
	/// </summary>
	/// <param name="val">Enum value.</param>
	public static bool IsReadOnly(this Enum val)
		=> ReadOnly.GetOrCreate(val,
			v => v.GetMemberInfo()?.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly == true
		);

	/// <summary>
	/// Returns if <see cref="Enum"/> value has <see cref="IgnoreDataMemberAttribute"/>.
	/// </summary>
	/// <param name="val">Enum value.</param>
	public static bool IsIgnore(this Enum val)
		=> Ignore.GetOrCreate(val, v => v
			.GetType()
			.GetMember(val.ToString())
			.FirstOrDefault()
			?.GetCustomAttribute<IgnoreDataMemberAttribute>(true)
			!= null
		);

	/// <summary>
	/// Returns order index for <see cref="Enum"/> value.
	/// </summary>
	/// <param name="val">Enum value.</param>
	public static int GetOrder(this Enum val)
		=> val.GetDisplayAttribute()?.GetOrder() ?? 0;

	/// <summary>
	/// Returns display name for <see cref="Enum"/> value.
	/// Enums with <see cref="FlagsAttribute"/> are supported.
	/// </summary>
	/// <param name="val">Enum value.</param>
	/// <param name="separator">Separator for flag enums only.</param>
	public static string GetDisplayName(this Enum val, string separator = ", ")
		=> GetDisplayNameInternal(val, separator, (enumValue, attr) => attr?.Name ?? enumValue.ToString());

	/// <summary>
	/// Returns display short name  for <see cref="Enum"/> value (or name if short name == null).
	/// Enums with <see cref="FlagsAttribute"/> are supported.
	/// </summary>
	/// <param name="val">Enum value.</param>
	/// <param name="separator">Separator for flag enums only.</param>
	public static string GetDisplayShortName(this Enum val, string separator = ", ")
		=> GetDisplayNameInternal(val, separator, (enumValue, attr) => attr?.ShortName ?? attr?.Name ?? enumValue.ToString());

	/// <summary>
	/// Returns display name for <see cref="Enum"/> value.
	/// Enums with <see cref="FlagsAttribute"/> are supported.
	/// </summary>
	/// <param name="val">Enum value.</param>
	/// <param name="separator">Separator for flag enum values.</param>
	/// <param name="getNameFunc">Function that should return display name from enum value and <see cref="DisplayAttribute"/>.</param>
	static string GetDisplayNameInternal(this Enum val, string separator, Func<Enum, DisplayAttribute?, string> getNameFunc)
	{
		Type type = val.GetType();
		if (type.IsEnumFlags() && !Enum.IsDefined(type, val))
		{
			var strs = Enum.GetValues(type)
				.Cast<Enum>()
				.Where(flag => Convert.ToInt64(flag) != 0 && val.HasFlag(flag!))
				.Select(flag =>
				{
					var flagAttr = GetDisplayAttribute(flag!);
					return getNameFunc(val, flagAttr);
				});
			return string.Join(separator, strs);
		}

		var attr = GetDisplayAttribute(val);
		return getNameFunc(val, attr);
	}

	/// <summary>
	/// Returns description for <see cref="Enum"/> value.
	/// </summary>
	/// <param name="val">Enum value.</param>
	public static string? GetDescription(this Enum val)
		=> val.GetDisplayAttribute()?.Description;

	/// <summary>
	/// Returns display attribute for <see cref="Enum"/> value.
	/// </summary>
	/// <param name="val">Enum value.</param>
	static DisplayAttribute? GetDisplayAttribute(this Enum val)
		=> val.GetMemberInfo()?.GetCustomAttribute<DisplayAttribute>(true);

	static MemberInfo? GetMemberInfo(this Enum val)
		=> val.GetType().GetMember(val.ToString()).FirstOrDefault();

	/// <summary>
	/// Returns value with only flags existing in enum type.
	/// </summary>
	public static T FilterExistingFlags<T>(this T val)
		where T : struct, Enum
		=> Enum.GetValues<T>()
			.Where(flag => val.HasFlag(flag))
			.Aggregate((long)0, (result, flag) => result | Convert.ToInt64(flag))
			.ConvertTo<T>();
}