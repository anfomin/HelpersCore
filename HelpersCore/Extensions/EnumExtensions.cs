using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;

namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="Enum"/>.
/// </summary>
[SuppressMessage("ReSharper", "InvokeAsExtensionMemberFromSameClass")]
public static class EnumExtensions
{
	static readonly ConcurrentDictionary<Enum, bool> EnumReadOnly = new();
	static readonly ConcurrentDictionary<Enum, bool> EnumIgnore = new();
	static readonly ConcurrentDictionary<Enum, DisplayAttribute?> EnumDisplay = new();

	extension(Enum)
	{
		/// <summary>
		/// Converts the specified object with an integer value to an enumeration member.
		/// </summary>
		/// <param name="value">The value convert to an enumeration member.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="value" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="value" /> is not type <see cref="sbyte" />, <see cref="short" />, <see cref="int" />, <see cref="long" />, <see cref="byte" />, <see cref="ushort" />, <see cref="uint" />, or <see cref="ulong" />.
		/// </exception>
		/// <returns>An enumeration object whose value is <paramref name="value" />.</returns>
		public static T ToObject<T>(object value)
			where T : struct, Enum
			=> (T)Enum.ToObject(typeof(T), value);

		/// <summary>
		/// Converts the specified 8-bit signed integer to an enumeration member.
		/// </summary>
		/// <param name="value">The value to convert to an enumeration member.</param>
		/// <returns>An instance of the enumeration set to <paramref name="value" />.</returns>
		public static T ToObject<T>(byte value)
			where T : struct, Enum
			=> (T)Enum.ToObject(typeof(T), value);

		/// <summary>
		/// Converts the specified 16-bit signed integer to an enumeration member.
		/// </summary>
		/// <param name="value">The value to convert to an enumeration member.</param>
		/// <returns>An instance of the enumeration set to <paramref name="value" />.</returns>
		public static T ToObject<T>(short value)
			where T : struct, Enum
			=> (T)Enum.ToObject(typeof(T), value);

		/// <summary>
		/// Converts the specified 32-bit signed integer to an enumeration member.
		/// </summary>
		/// <param name="value">The value to convert to an enumeration member.</param>
		/// <returns>An instance of the enumeration set to <paramref name="value" />.</returns>
		public static T ToObject<T>(int value)
			where T : struct, Enum
			=> (T)Enum.ToObject(typeof(T), value);

		/// <summary>
		/// Converts the specified 64-bit signed integer to an enumeration member.
		/// </summary>
		/// <param name="value">The value to convert to an enumeration member.</param>
		/// <returns>An instance of the enumeration set to <paramref name="value" />.</returns>
		public static T ToObject<T>(long value)
			where T : struct, Enum
			=> (T)Enum.ToObject(typeof(T), value);
	}

	extension(Enum enm)
	{
		/// <summary>
		/// Returns if <see cref="Enum"/> value has <see cref="ReadOnlyAttribute"/> with <c>true</c> value.
		/// </summary>
		public bool IsReadOnly => EnumReadOnly.GetOrAdd(enm,
			v => v.GetMemberInfo()?.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly == true
		);

		/// <summary>
		/// Returns if <see cref="Enum"/> value has <see cref="IgnoreDataMemberAttribute"/>.
		/// </summary>
		public bool IsIgnore => EnumIgnore.GetOrAdd(enm,
			v => v.GetMemberInfo()?.GetCustomAttribute<IgnoreDataMemberAttribute>() is not null
		);

		/// <summary>
		/// Returns order index for <see cref="Enum"/> value.
		/// </summary>
		public int Order => enm.GetDisplayAttribute()?.GetOrder() ?? 0;

		/// <summary>
		/// Returns description for <see cref="Enum"/> value.
		/// </summary>
		public string? Description => enm.GetDisplayAttribute()?.Description;

		/// <summary>
		/// Returns display name for <see cref="Enum"/> value.
		/// Enums with <see cref="FlagsAttribute"/> are supported.
		/// </summary>
		public string DisplayName => enm.GetDisplayName(", ");

		/// <summary>
		/// Returns display short name  for <see cref="Enum"/> value (or name if short name is <c>null</c>).
		/// Enums with <see cref="FlagsAttribute"/> are supported.
		/// </summary>
		public string DisplayShortName => enm.GetDisplayShortName(", ");

		/// <summary>
		/// Returns display name for <see cref="Enum"/> value.
		/// Enums with <see cref="FlagsAttribute"/> are supported.
		/// </summary>
		/// <param name="separator">Separator for flag enums only.</param>
		public string GetDisplayName(string separator)
			=> enm.GetDisplayInternal(separator, (enumValue, attr) => attr?.Name ?? enumValue.ToString());

		/// <summary>
		/// Returns display short name  for <see cref="Enum"/> value (or name if short name is <c>null</c>).
		/// Enums with <see cref="FlagsAttribute"/> are supported.
		/// </summary>
		/// <param name="separator">Separator for flag enums only.</param>
		public string GetDisplayShortName(string separator)
			=> enm.GetDisplayInternal(separator, (enumValue, attr) => attr?.ShortName ?? attr?.Name ?? enumValue.ToString());

		string GetDisplayInternal(string separator, Func<Enum, DisplayAttribute?, string> fn)
		{
			Type type = enm.GetType();
			if (type.IsEnumFlags && !Enum.IsDefined(type, enm))
			{
				var strs = Enum.GetValues(type)
					.Cast<Enum>()
					.Where(flag => Convert.ToInt64(flag) != 0 && enm.HasFlag(flag!))
					.Select(flag =>
						{
							var flagAttr = GetDisplayAttribute(flag!);
							return fn(enm, flagAttr);
						}
					);
				return string.Join(separator, strs);
			}

			var attr = enm.GetDisplayAttribute();
			return fn(enm, attr);
		}

		DisplayAttribute? GetDisplayAttribute() => EnumDisplay.GetOrAdd(enm,
			v => v.GetMemberInfo()?.GetCustomAttribute<DisplayAttribute>()
		);

		MemberInfo? GetMemberInfo()
			=> enm.GetType().GetMember(enm.ToString()).FirstOrDefault();
	}

	extension<T>(T enm) where T : struct, Enum
	{
		/// <summary>
		/// Returns value with only flags existing in enum type.
		/// </summary>
		public T FilterExistingFlags()
			=> Enum.ToObject<T>(
				Enum.GetValues<T>()
					.Where(flag => enm.HasFlag(flag))
					.Aggregate(0L, (result, flag) => result | Convert.ToInt64(flag))
			);
	}
}