using System.Diagnostics.CodeAnalysis;

namespace HelpersCore;

#if NET11_0_OR_GREATER

/// <summary>
/// Provides extension methods for the <see cref="Result"/> type.
/// </summary>
public static class ResultExtensions
{
	extension(Result)
	{
		/// <summary>
		/// Creates a new <see cref="ResultSuccess{T}"/> instance with the specified value, representing a successful result.
		/// </summary>
		/// <param name="value">The value of the successful result.</param>
		/// <typeparam name="T">The type of the result.</typeparam>
		/// <returns>A <see cref="ResultSuccess{T}"/> instance representing a successful result.</returns>
		public static ResultSuccess<T> Success<T>(T value) => new(value);

		/// <summary>
		/// Creates a new <see cref="ResultFailure"/> instance with the specified failure message, representing a failed result.
		/// </summary>
		/// <param name="message">The exception representing the failure.</param>
		/// <returns>A <see cref="ResultFailure"/> instance representing a failed result.</returns>
		public static ResultFailure Failure(Exception message) => new(message);

		/// <summary>
		/// Creates a new <see cref="ResultFailure"/> instance with the specified failure message, representing a failed result.
		/// </summary>
		/// <param name="message">The failure message representing the failure.</param>
		/// <returns>A <see cref="ResultFailure"/> instance representing a failed result.</returns>
		public static ResultFailure Failure(string message) => new(message);

		/// <summary>
		/// Creates a new <see cref="ResultFailure"/> instance with the specified failure messages, representing a failed result.
		/// </summary>
		/// <param name="messages">The failure messages representing the failure.</param>
		/// <returns>A <see cref="ResultFailure"/> instance representing a failed result.</returns>
		public static ResultFailure Failure(params IEnumerable<string> messages) => new(messages);
	}

	extension(IResultUnion result)
	{
		/// <summary>
		/// Determines whether the operation has succeeded.
		/// </summary>
		public bool IsSuccess => result.Value is ResultSuccess;

		/// <summary>
		/// Determines whether the operation is pending.
		/// </summary>
		public bool IsPending => result.Value is ResultPending;

		/// <summary>
		/// Determines whether the operation has failed.
		/// </summary>
		public bool IsFailure => result.Value is ResultFailure;

		/// <summary>
		/// Returns the <see cref="ResultFailure"/> instance if the result is in failure state, or <see langword="null"/> if it is in success or pending state.
		/// </summary>
		public ResultFailure? GetFailureOrNull()
			=> result.Value as ResultFailure;
	}

	extension<T>(Result<T> result) where T : struct
	{
		/// <summary>
		/// Returns the value if the result is in success state, or <see langword="null"/> if it is in pending or failure state.
		/// </summary>
		/// <returns>The value if the result is in success state, or <see langword="null"/> if it is in pending or failure state.</returns>
		public T? GetValueOrNull()
			=> result.Value is ResultSuccess<T> s ? s.Data : null;
	}
}

#endif