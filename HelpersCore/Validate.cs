using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace HelpersCore;

/// <summary>
/// Provides methods to validate arguments.
/// </summary>
public static class Validate
{
	/// <summary>
	/// Throws an <see cref="ArgumentException"/> if the given value is not greater than specified minimum.
	/// </summary>
	[DebuggerHidden]
	public static T GreaterThan<T>(T value, double min, [CallerArgumentExpression("value")] string? name = null)
		where T : struct
	{
		double number = Convert.ToDouble(value);
		if (number <= min)
			throw new ArgumentException($"{name ?? "Value"} = '{value}' and must be greater than {min}");
		return value;
	}

	/// <summary>
	/// Throws an <see cref="ArgumentException"/> if the given value is not greater or equals than specified minimum.
	/// </summary>
	[DebuggerHidden]
	public static T GreaterOrEqualsThan<T>(T value, double min, [CallerArgumentExpression("value")] string? name = null)
		where T : struct
	{
		double number = Convert.ToDouble(value);
		if (number < min)
			throw new ArgumentException($"{name ?? "Value"} = '{value}' and must be greater or equals than {min}");
		return value;
	}

	/// <summary>
	/// Throws an <see cref="ArgumentException"/> if the given value is not less than specified maximum.
	/// </summary>
	[DebuggerHidden]
	public static T LessThan<T>(T value, double max, [CallerArgumentExpression("value")] string? name = null)
		where T : struct
	{
		double number = Convert.ToDouble(value);
		if (number >= max)
			throw new ArgumentException($"{name ?? "Value"} = '{value}' and must be less than {max}");
		return value;
	}

	/// <summary>
	/// Throws an <see cref="ArgumentException"/> if the given value is not less or equals than specified maximum.
	/// </summary>
	[DebuggerHidden]
	public static T LessOrEqualsThan<T>(T value, double max, [CallerArgumentExpression("value")] string? name = null)
		where T : struct
	{
		double number = Convert.ToDouble(value);
		if (number > max)
			throw new ArgumentException($"{name ?? "Value"} = '{value}' and must be less or equals than {max}");
		return value;
	}

	/// <summary>
	/// Throws an <see cref="ArgumentException"/> if the given argument is not in specified [min, max] range.
	/// </summary>
	[DebuggerHidden]
	public static T Range<T>(T value, double min, double max, [CallerArgumentExpression("value")] string? name = null)
		where T : struct
	{
		double number = Convert.ToDouble(value);
		if (number < min || number > max)
			throw new ArgumentException($"{name ?? "Value"} = '{value}' and must be in range [{min}, {max}]");
		return value;
	}

	/// <summary>
	/// Throws an <see cref="ArgumentException"/> if the given argument is not in specified [min, max] range.
	/// </summary>
	[DebuggerHidden]
	public static decimal Range(decimal value, decimal min, decimal max, [CallerArgumentExpression("value")] string? name = null)
	{
		if (value < min || value > max)
			throw new ArgumentException($"{name ?? "Value"} = '{value}' and must be in range [{min}, {max}]");
		return value;
	}

	/// <summary>
	/// Throws an <see cref="ArgumentNullException"/> if the given argument is <c>null</c> or <see cref="ArgumentException"/> if one is empty.
	/// </summary>
	[DebuggerHidden]
	public static IEnumerable<T> NotEmpty<T>(IEnumerable<T>? value, string? message = null, [CallerArgumentExpression("value")] string? name = null)
	{
		if (value is null)
			throw new ArgumentNullException(name, message);
		if (value.None())
			throw new ArgumentException(message ?? $"{name ?? "Value"} is empty", name);
		return value;
	}

	/// <summary>
	/// Throws an <see cref="ArgumentNullException"/> if the given argument is <c>null</c> or <see cref="ArgumentException"/> if one is empty.
	/// </summary>
	[DebuggerHidden]
	public static string NotEmpty(string? value, string? message = null, [CallerArgumentExpression("value")] string? name = null)
	{
		if (value is null)
			throw new ArgumentNullException(name, message);
		if (value == string.Empty)
			throw new ArgumentException(message ?? $"{name ?? "Value"} is empty", name);
		return value;
	}

	/// <summary>
	/// Throws an <see cref="ArgumentNullException"/> if the given argument is <c>null</c> or <see cref="ArgumentException"/> if one is empty.
	/// </summary>
	[DebuggerHidden]
	public static T NotEmpty<T>(T? value, string? message = null, [CallerArgumentExpression("value")] string? name = null)
		where T : ICollection
	{
		if (value is null)
			throw new ArgumentNullException(name, message);
		if (value.Count == 0)
			throw new ArgumentException(message ?? $"{name ?? "Value"} does not contains any element", name);
		return value;
	}
}