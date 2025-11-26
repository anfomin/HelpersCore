using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace HelpersCore;

/// <summary>
/// Provides extensions for object conversion.
/// </summary>
public static class ConvertExtensions
{
	/// <summary>
	/// Tries convert <paramref name="value"/> to the type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">Type to convert to.</typeparam>
	/// <param name="value">Object to convert.</param>
	/// <param name="result">Converted result.</param>
	/// <returns><c>True</c> if conversion success, otherwise <c>false</c>.</returns>
	public static bool TryConvertTo<T>(this object value, [MaybeNullWhen(false)] out T result)
	{
		try
		{
			result = ConvertTo<T>(value)!;
			return true;
		}
		catch
		{
			result = default;
			return false;
		}
	}

	/// <summary>
	/// Converts <paramref name="value"/> to the type <typeparamref name="T"/>.
	/// If conversion fails throws exception.
	/// </summary>
	/// <typeparam name="T">Type to convert to.</typeparam>
	/// <param name="value">Object to convert.</param>
	public static T? ConvertTo<T>(this object? value)
	{
		var type = typeof(T);
		if (value is null)
		{
			T def = default!;
			if (def is not null)
				throw new ArgumentNullException(nameof(value));
			return def;
		}
		if (value is T valueTyped)
			return valueTyped;

		// check string & enum
		if (type == typeof(string))
			return (T)(object)value.ToString()!;

		// check nullable
		type = Nullable.GetUnderlyingType(type) ?? type;

		// check for enum
		if (type.IsEnum)
			return (T)Enum.Parse(type, value.ToString()!, true);

		// type converter
		var converter = TypeDescriptor.GetConverter(typeof(T));
		if (converter.CanConvertFrom(type))
			return (T)converter.ConvertFrom(value)!;

		// standard converter
		return (T)Convert.ChangeType(value, type);
	}
}