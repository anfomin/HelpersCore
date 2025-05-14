using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace HelpersCore;

/// <summary>
/// Represents operation result with one of 3 states: success, pending or error.
/// </summary>
public record Result
{
	/// <summary>
	/// Gets if operation is successful.
	/// </summary>
	public virtual bool IsSuccess { get; init; }

	/// <summary>
	/// Gets if operation is pending.
	/// </summary>
	public bool IsPending { get; init; }

	/// <summary>
	/// Gets if operation has failed.
	/// </summary>
	[MemberNotNullWhen(true, nameof(ErrorMessage))]
	public bool IsError { get; init; }

	/// <summary>
	/// Gets error messages if operation has failed.
	/// </summary>
	public ImmutableArray<string> ErrorMessages { get; init; } = [];

	/// <summary>
	/// Gets first error message if operation has failed.
	/// </summary>
	public string? ErrorMessage
		=> ErrorMessages.Length > 0 ? ErrorMessages[0] : null;

	internal Result() { }

	/// <summary>
	/// Returns succeeded operation result.
	/// </summary>
	public static Result Success { get; } = new() { IsSuccess = true };

	/// <summary>
	/// Returns pending operation result.
	/// </summary>
	public static Result Pending { get; } = new() { IsPending = true };

	/// <summary>
	/// Returns operation result with specified error messages.
	/// </summary>
	/// <param name="errors">Error messages.</param>
	/// <exception cref="ArgumentException">If errors is empty.</exception>
	public static Result Error(params IEnumerable<string> errors)
	{
		var errorMessages = errors.ToImmutableArray();
		return errorMessages.Length == 0
			? throw new ArgumentException("At least one error required", nameof(errors))
			: new() { IsError = true, ErrorMessages = errorMessages };
	}

	/// <summary>
	/// Returns operation result with <paramref name="ex"/> error message.
	/// </summary>
	/// <param name="ex">Exception for error message.</param>
	public static Result Error(Exception ex) => new() { IsError = true, ErrorMessages = [ex.Message] };
}

/// <summary>
/// Represents operation result with some data if succeeded.
/// </summary>
/// <typeparam name="T">Success data type.</typeparam>
public sealed record Result<T> : Result
{
	/// <inheritdoc/>
	[MemberNotNullWhen(true, nameof(Data))]
	public override bool IsSuccess
	{
		get => base.IsSuccess;
		init => base.IsSuccess = value;
	}

	/// <summary>
	/// Gets operation data if succeeded.
	/// </summary>
	public T? Data { get; init; }

	internal Result() { }

	/// <summary>
	/// Returns succeeded operation result.
	/// </summary>
	/// <param name="data">Result data.</param>
	public new static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };

	/// <summary>
	/// Returns pending operation result.
	/// </summary>
	public new static Result<T> Pending { get; } = new() { IsPending = true };

	/// <summary>
	/// Returns operation result with specified error messages.
	/// </summary>
	/// <param name="errors">Error messages.</param>
	/// <exception cref="ArgumentException">If errors is empty.</exception>
	public new static Result<T> Error(params IEnumerable<string> errors)
	{
		var errorMessages = errors.ToImmutableArray();
		return errorMessages.Length == 0
			? throw new ArgumentException("At least one error required", nameof(errors))
			: new() { IsError = true, ErrorMessages = errorMessages };
	}

	/// <summary>
	/// Returns operation result with <paramref name="ex"/> error message.
	/// </summary>
	/// <param name="ex">Exception for error message.</param>
	public new static Result<T> Error(Exception ex) => new() { IsError = true, ErrorMessages = [ex.Message] };

	/// <summary>
	/// Converts data to succeeded operation result.
	/// </summary>
	/// <param name="data">Result data.</param>
	public static implicit operator Result<T>(T data) => new() { IsSuccess = true, Data = data };
}