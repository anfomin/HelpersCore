using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.Serialization;

namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="Enum"/>.
/// </summary>
public static class EnumExtensions
{
	static readonly ConcurrentDictionary<Enum, bool> EnumReadOnly = new();
	static readonly ConcurrentDictionary<Enum, bool> EnumIgnore = new();

	extension(Enum enm)
	{
		/// <summary>
		/// Returns if <see cref="Enum"/> value has <see cref="ReadOnlyAttribute"/> with <c>true</c> value.
		/// </summary>
		public bool IsReadOnly()
			=> EnumReadOnly.GetOrAdd(enm,
				v => v.GetMemberInfo()?.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly == true
			);

		/// <summary>
		/// Returns if <see cref="Enum"/> value has <see cref="IgnoreDataMemberAttribute"/>.
		/// </summary>
		public bool IsIgnore()
			=> EnumIgnore.GetOrAdd(enm,
				v => v
					.GetType()
					.GetMember(enm.ToString())
					.FirstOrDefault()
					?.GetCustomAttribute<IgnoreDataMemberAttribute>(true)
				is not null
			);

		/// <summary>
		/// Returns order index for <see cref="Enum"/> value.
		/// </summary>
		public int GetOrder()
			=> enm.GetDisplayAttribute()?.GetOrder() ?? 0;

		/// <summary>
		/// Returns display name for <see cref="Enum"/> value.
		/// Enums with <see cref="FlagsAttribute"/> are supported.
		/// </summary>
		/// <param name="separator">Separator for flag enums only.</param>
		public string GetDisplayName(string separator = ", ")
			=> GetDisplayNameInternal(enm, separator, (enumValue, attr) => attr?.Name ?? enumValue.ToString());

		/// <summary>
		/// Returns display short name  for <see cref="Enum"/> value (or name if short name == <c>null</c>).
		/// Enums with <see cref="FlagsAttribute"/> are supported.
		/// </summary>
		/// <param name="separator">Separator for flag enums only.</param>
		public string GetDisplayShortName(string separator = ", ")
			=> GetDisplayNameInternal(enm, separator, (enumValue, attr) => attr?.ShortName ?? attr?.Name ?? enumValue.ToString());

		/// <summary>
		/// Returns display name for <see cref="Enum"/> value.
		/// Enums with <see cref="FlagsAttribute"/> are supported.
		/// </summary>
		/// <param name="separator">Separator for flag enum values.</param>
		/// <param name="getNameFunc">Function that should return display name from enum value and <see cref="DisplayAttribute"/>.</param>
		string GetDisplayNameInternal(string separator, Func<Enum, DisplayAttribute?, string> getNameFunc)
		{
			Type type = enm.GetType();
			if (type.IsEnumFlags() && !Enum.IsDefined(type, enm))
			{
				var strs = Enum.GetValues(type)
					.Cast<Enum>()
					.Where(flag => Convert.ToInt64(flag) != 0 && enm.HasFlag(flag!))
					.Select(flag =>
						{
							var flagAttr = GetDisplayAttribute(flag!);
							return getNameFunc(enm, flagAttr);
						}
					);
				return string.Join(separator, strs);
			}

			var attr = GetDisplayAttribute(enm);
			return getNameFunc(enm, attr);
		}

		/// <summary>
		/// Returns description for <see cref="Enum"/> value.
		/// </summary>
		public string? GetDescription()
			=> enm.GetDisplayAttribute()?.Description;

		/// <summary>
		/// Returns display attribute for <see cref="Enum"/> value.
		/// </summary>
		DisplayAttribute? GetDisplayAttribute()
			=> enm.GetMemberInfo()?.GetCustomAttribute<DisplayAttribute>(true);

		MemberInfo? GetMemberInfo()
			=> enm.GetType().GetMember(enm.ToString()).FirstOrDefault();
	}

	extension<T>(T enm) where T : struct, Enum
	{
		/// <summary>
		/// Returns value with only flags existing in enum type.
		/// </summary>
		public T FilterExistingFlags()
			=> Enum.GetValues<T>()
				.Where(flag => enm.HasFlag(flag))
				.Aggregate((long)0, (result, flag) => result | Convert.ToInt64(flag))
				.ConvertTo<T>();
	}
}