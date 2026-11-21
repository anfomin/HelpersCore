using System.Collections.Immutable;

namespace HelpersCore;

#if NET11_0_OR_GREATER

/// <summary>
/// Represents operation result with one of 3 states: success, pending or error.
/// </summary>
public union Result(ResultSuccess, ResultPending, ResultError)
{
	/// <summary>
	/// Determines whether the operation has succeeded.
	/// </summary>
	public bool IsSuccess => Value is ResultSuccess;

	/// <summary>
	/// Determines whether the operation is pending.
	/// </summary>
	public bool IsPending => Value is ResultPending;

	/// <summary>
	/// Determines whether the operation has failed.
	/// </summary>
	public bool IsFailure => Value is ResultError;

	/// <summary>
	/// Converts success, pending or error state to specified result.
	/// </summary>
	/// <param name="onSuccess">Function to handle success state.</param>
	/// <param name="onPending">Function to handle pending state.</param>
	/// <param name="onError">Function to handle error state.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The result of the matching function.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is uninitialized (default).</exception>
	public TOut Match<TOut>(Func<TOut> onSuccess, Func<TOut> onPending, Func<ResultError, TOut> onError) => this switch
	{
		ResultSuccess => onSuccess(),
		ResultPending => onPending(),
		ResultError e => onError(e),
		null => throw new InvalidOperationException("Uninitialized Result (default)")
	};

	/// <summary>
	/// Converts success or error state to specified result, throwing an exception if the result is pending.
	/// </summary>
	/// <param name="onSuccess">Function to handle success state.</param>
	/// <param name="onError">Function to handle error state.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The result of the matching function.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is pending or uninitialized (default).</exception>
	public TOut Match<TOut>(Func<TOut> onSuccess, Func<ResultError, TOut> onError)
		=> Match(onSuccess, () => throw new InvalidOperationException("Can not match pending"), onError);

	/// <summary>
	/// Maps success state to specified <typeparamref name="TOut"/>, returning the pending or error state as is.
	/// </summary>
	/// <param name="map">Function to map success state.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The mapped result.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is uninitialized (default).</exception>
	public Result<TOut> Map<TOut>(Func<TOut> map) => this switch
	{
		ResultSuccess => new ResultSuccess<TOut>(map()),
		ResultPending p => p,
		ResultError e => e,
		null => throw new InvalidOperationException("Uninitialized Result (default)")
	};

	/// <summary>
	/// Binds success state to specified result, returning the pending or error state as is.
	/// </summary>
	/// <param name="next">Function to bind success state to specified result.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The bound result.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is uninitialized (default).</exception>
	public Result<TOut> Bind<TOut>(Func<Result<TOut>> next) => this switch
	{
		ResultSuccess => next(),
		ResultPending p => p,
		ResultError e  => e,
		null => throw new InvalidOperationException("Uninitialized Result (default)")
	};

	/// <summary>
	/// Executes the specified action if the result is in success state.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <returns>The original result.</returns>
	public Result Tap(Action action)
	{
		if (Value is ResultSuccess s)
			action();
		return this;
	}

	/// <summary>
	/// Throws an exception if the result is in pending or error state.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown when the result is pending, has an error, or is uninitialized.</exception>
	/// <exception cref="Exception">Thrown when the result has an error with an exception.</exception>
	public void ThrowIfFailed()
	{
		switch (this)
		{
			case ResultSuccess: return;
			case ResultPending: throw new InvalidOperationException("Operation is pending, value is not available");
			case ResultError { Exception: { } ex }: throw ex;
			case ResultError e: throw new InvalidOperationException($"Operation failed: {e.ErrorMessage}");
			case null: throw new InvalidOperationException("Uninitialized Result (default)");
		}
	}
}

/// <summary>
/// Represents operation result with one of 3 states: success and some data, pending or error.
/// </summary>
/// <typeparam name="T">Success data type</typeparam>
public union Result<T>(ResultSuccess<T>, ResultPending, ResultError)
{
	/// <summary>
	/// Determines whether the operation has succeeded and has data.
	/// </summary>
	public bool IsSuccess => Value is ResultSuccess<T>;

	/// <summary>
	/// Determines whether the operation is pending.
	/// </summary>
	public bool IsPending => Value is ResultPending;

	/// <summary>
	/// Determines whether the operation has failed.
	/// </summary>
	public bool IsFailure => Value is ResultError;

	/// <summary>
	/// Converts success, pending or error state to specified result.
	/// </summary>
	/// <param name="onSuccess">Function to handle success state.</param>
	/// <param name="onPending">Function to handle pending state.</param>
	/// <param name="onError">Function to handle error state.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The result of the matching function.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is uninitialized (default).</exception>
	public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<TOut> onPending, Func<ResultError, TOut> onError) => this switch
	{
		ResultSuccess<T> s => onSuccess(s.Value),
		ResultPending => onPending(),
		ResultError e => onError(e),
		null => throw new InvalidOperationException("Uninitialized Result (default)")
	};

	/// <summary>
	/// Converts success or error state to specified result, throwing an exception if the result is pending.
	/// </summary>
	/// <param name="onSuccess">Function to handle success state.</param>
	/// <param name="onError">Function to handle error state.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The result of the matching function.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is pending or uninitialized (default).</exception>
	public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<ResultError, TOut> onError)
		=> Match(onSuccess, () => throw new InvalidOperationException("Can not match pending"), onError);

	/// <summary>
	/// Maps success state to specified <typeparamref name="TOut"/>, returning the pending or error state as is.
	/// </summary>
	/// <param name="map">Function to map success state.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The mapped result.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is uninitialized (default).</exception>
	public Result<TOut> Map<TOut>(Func<T, TOut> map) => this switch
	{
		ResultSuccess<T> s => new ResultSuccess<TOut>(map(s.Value)),
		ResultPending p => p,
		ResultError e => e,
		null => throw new InvalidOperationException("Uninitialized Result (default)")
	};

	/// <summary>
	/// Binds success state to specified result, returning the pending or error state as is.
	/// </summary>
	/// <param name="next">Function to bind success state to specified result.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The bound result.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is uninitialized (default).</exception>
	public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> next) => this switch
	{
		ResultSuccess<T> s => next(s.Value),
		ResultPending p => p,
		ResultError e  => e,
		null => throw new InvalidOperationException("Uninitialized Result (default)")
	};

	/// <summary>
	/// Executes the specified action if the result is in success state.
	/// </summary>
	/// <param name="action">The action to execute.</param>
	/// <returns>The original result.</returns>
	public Result<T> Tap(Action<T> action)
	{
		if (Value is ResultSuccess<T> s)
			action(s.Value);
		return this;
	}

	/// <summary>
	/// Returns the value if the result is in success state, or the specified default value if it is in pending or error state.
	/// </summary>
	/// <param name="defaultValue">The default value to return if the result is not in success state.</param>
	/// <returns>The value if the result is in success state, or the specified default value if it is in pending or error state.</returns>
	public T GetValueOrDefault(T defaultValue) =>
		Value is ResultSuccess<T> s ? s.Value : defaultValue;

	/// <summary>
	/// Returns the value if the result is in success state, or throws an exception if it is in pending or error state.
	/// </summary>
	/// <returns>The value if the result is in success state.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is pending, has an error, or is uninitialized.</exception>
	/// <exception cref="Exception">Thrown when the result has an error with an exception.</exception>
	public T GetValueOrThrow() => this switch
	{
		ResultSuccess<T> s => s.Value,
		ResultPending => throw new InvalidOperationException("Operation is pending, value is not available"),
		ResultError { Exception: { } ex } => throw ex,
		ResultError e => throw new InvalidOperationException($"Operation failed: {e.ErrorMessage}"),
		null => throw new InvalidOperationException("Uninitialized Result (default)")
	};
}

/// <summary>
/// Represents operation result with success state.
/// </summary>
public record ResultSuccess;

/// <summary>
/// Represents operation result with success state and some data.
/// </summary>
/// <param name="Value">Result data.</param>
/// <typeparam name="T">Result data type.</typeparam>
public sealed record ResultSuccess<T>(T Value) : ResultSuccess;

/// <summary>
/// Represents operation result with pending state.
/// </summary>
public sealed record ResultPending;

/// <summary>
/// Represents operation result with error state.
/// </summary>
public sealed record ResultError
{
	/// <summary>
	/// Gets exception if operation has failed with exception.
	/// </summary>
	public Exception? Exception { get; }

	/// <summary>
	/// Gets error messages.
	/// </summary>
	public ImmutableArray<string> ErrorMessages { get; }

	/// <summary>
	/// Gets first error message.
	/// </summary>
	public string? ErrorMessage
		=> ErrorMessages.Length > 0 ? ErrorMessages[0] : null;

	/// <summary>
	/// Initializes with specified exception.
	/// </summary>
	/// <param name="ex">Exception occured.</param>
	public ResultError(Exception ex)
	{
		Exception = ex;
		ErrorMessages = [ex.Message];
	}

	/// <summary>
	/// Initializes with specified error message.
	/// </summary>
	/// <param name="errorMessage">Error message.</param>
	public ResultError(string errorMessage)
		=> ErrorMessages = [errorMessage];

	/// <summary>
	/// Initializes with specified error messages.
	/// </summary>
	/// <param name="errorMessages">Error messages.</param>
	public ResultError(params IEnumerable<string> errorMessages)
		=> ErrorMessages = errorMessages.ToImmutableArray();
}

#endif