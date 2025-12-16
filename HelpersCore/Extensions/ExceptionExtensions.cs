using System.Runtime.CompilerServices;

namespace HelpersCore;

/// <summary>
/// Provides extension methods for exceptions.
/// </summary>
public static class ExceptionExtensions
{
	extension(ArgumentOutOfRangeException)
	{
		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException"/> if the given value is not in the specified range <c>[min, max]</c>.
		/// </summary>
		/// <param name="value">Value to validate.</param>
		/// <param name="min">Minimum inclusive value.</param>
		/// <param name="max">Maximum inclusive value.</param>
		/// <param name="paramName">The name of the validated parameter.</param>
		public static void ThrowIfNotInRange<T>(T value, T min, T max, [CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(min) < 0)
				throw new ArgumentOutOfRangeException(paramName, value, $"Value must be greater than or equals to {min}.");
			if (value.CompareTo(max) > 0)
				throw new ArgumentOutOfRangeException(paramName, value, $"Value must be less than or equals to {max}.");
		}
	}
}