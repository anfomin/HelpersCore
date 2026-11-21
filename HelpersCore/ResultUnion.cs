using System.Diagnostics.CodeAnalysis;

namespace HelpersCore;

#if NET11_0_OR_GREATER

/// <summary>
/// Represents a type that can be used as a result of an operation, which can be in one of three states: success, pending, or failure.
/// </summary>
public interface IResultUnion
{
	/// <summary>
	/// Gets the value of the result, which can be a success, pending, or failure state.
	/// </summary>
	object? Value { get; }
}

/// <summary>
/// Represents operation result with one of 3 states: success, pending or failure.
/// </summary>
public union Result(ResultSuccess, ResultPending, ResultFailure) : IResultUnion
{
	/// <summary>
	/// Gets the success state instance.
	/// </summary>
	public static ResultSuccess Success { get; } = new();

	/// <summary>
	/// Gets the pending state instance.
	/// </summary>
	public static ResultPending Pending { get; } = new();

	/// <summary>
	/// Implicitly converts an <see cref="Exception"/> to a <see cref="Result"/> in the failure state.
	/// </summary>
	/// <param name="ex">The exception representing the failure.</param>
	/// <returns>A <see cref="Result"/> representing the failure state with the specified exception.</returns>
	public static implicit operator Result(Exception ex)
		=> new ResultFailure(ex);

	/// <summary>
	/// Converts success, pending or failure state to specified result.
	/// </summary>
	/// <param name="onSuccess">Function to handle success state.</param>
	/// <param name="onPending">Function to handle pending state.</param>
	/// <param name="onFailure">Function to handle failure state.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The result of the matching function.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is uninitialized (default).</exception>
	public TOut Match<TOut>(Func<TOut> onSuccess, Func<TOut> onPending, Func<ResultFailure, TOut> onFailure) => this switch
	{
		ResultSuccess => onSuccess(),
		ResultPending => onPending(),
		ResultFailure e => onFailure(e),
		null => throw new InvalidOperationException("Uninitialized Result (default)")
	};

	/// <summary>
	/// Converts success or failure state to specified result, throwing an exception if the result is pending.
	/// </summary>
	/// <param name="onSuccess">Function to handle success state.</param>
	/// <param name="onFailure">Function to handle failure state.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The result of the matching function.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is pending or uninitialized (default).</exception>
	public TOut Match<TOut>(Func<TOut> onSuccess, Func<ResultFailure, TOut> onFailure)
		=> Match(onSuccess, () => throw new InvalidOperationException("Can not match pending"), onFailure);

	/// <summary>
	/// Maps success state to specified <typeparamref name="TOut"/>, returning the pending or failure state as is.
	/// </summary>
	/// <param name="map">Function to map success state.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The mapped result.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is uninitialized (default).</exception>
	public Result<TOut> Map<TOut>(Func<TOut> map) => this switch
	{
		ResultSuccess => new ResultSuccess<TOut>(map()),
		ResultPending p => p,
		ResultFailure e => e,
		null => throw new InvalidOperationException("Uninitialized Result (default)")
	};

	/// <summary>
	/// Binds success state to specified result, returning the pending or failure state as is.
	/// </summary>
	/// <param name="next">Function to bind success state to specified result.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The bound result.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is uninitialized (default).</exception>
	public Result<TOut> Bind<TOut>(Func<Result<TOut>> next) => this switch
	{
		ResultSuccess => next(),
		ResultPending p => p,
		ResultFailure e  => e,
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
	/// Throws an exception if the result is in pending or failure state.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown when the result is pending, has a failure, or is uninitialized.</exception>
	/// <exception cref="Exception">Thrown when the result has a failure with an exception.</exception>
	public void ThrowIfFailed()
	{
		switch (this)
		{
			case ResultSuccess: return;
			case ResultPending: throw new InvalidOperationException("Operation is pending, value is not available");
			case ResultFailure { Exception: { } ex }: throw ex;
			case ResultFailure e: throw new InvalidOperationException($"Operation failed: {e.Message}");
			case null: throw new InvalidOperationException("Uninitialized Result (default)");
		}
	}
}

/// <summary>
/// Represents operation result with one of 3 states: success and some data, pending or failure.
/// </summary>
/// <typeparam name="T">Success data type</typeparam>
public union Result<T>(ResultSuccess<T>, ResultPending, ResultFailure) : IResultUnion
{
	/// <summary>
	/// Implicitly converts a value of type <typeparamref name="T"/> to a <see cref="Result{T}"/> in the success state.
	/// </summary>
	/// <param name="data">The success data.</param>
	/// <returns>A <see cref="Result{T}"/> representing the success state with the specified data.</returns>
	public static implicit operator Result<T>(T data)
		=> new ResultSuccess<T>(data);

	/// <summary>
	/// Implicitly converts an <see cref="Exception"/> to a <see cref="Result{T}"/> in the failure state.
	/// </summary>
	/// <param name="ex">The exception representing the failure.</param>
	/// <returns>A <see cref="Result{T}"/> representing the failure state with the specified exception.</returns>
	public static implicit operator Result<T>(Exception ex)
		=> new ResultFailure(ex);

	/// <summary>
	/// Converts success, pending or failure state to specified result.
	/// </summary>
	/// <param name="onSuccess">Function to handle success state.</param>
	/// <param name="onPending">Function to handle pending state.</param>
	/// <param name="onFailure">Function to handle failure state.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The result of the matching function.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is uninitialized (default).</exception>
	public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<TOut> onPending, Func<ResultFailure, TOut> onFailure) => this switch
	{
		ResultSuccess<T> s => onSuccess(s.Data),
		ResultPending => onPending(),
		ResultFailure e => onFailure(e),
		null => throw new InvalidOperationException("Uninitialized Result (default)")
	};

	/// <summary>
	/// Converts success or failure state to specified result, throwing an exception if the result is pending.
	/// </summary>
	/// <param name="onSuccess">Function to handle success state.</param>
	/// <param name="onFailure">Function to handle failure state.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The result of the matching function.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is pending or uninitialized (default).</exception>
	public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<ResultFailure, TOut> onFailure)
		=> Match(onSuccess, () => throw new InvalidOperationException("Can not match pending"), onFailure);

	/// <summary>
	/// Maps success state to specified <typeparamref name="TOut"/>, returning the pending or failure state as is.
	/// </summary>
	/// <param name="map">Function to map success state.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The mapped result.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is uninitialized (default).</exception>
	public Result<TOut> Map<TOut>(Func<T, TOut> map) => this switch
	{
		ResultSuccess<T> s => new ResultSuccess<TOut>(map(s.Data)),
		ResultPending p => p,
		ResultFailure e => e,
		null => throw new InvalidOperationException("Uninitialized Result (default)")
	};

	/// <summary>
	/// Binds success state to specified result, returning the pending or failure state as is.
	/// </summary>
	/// <param name="next">Function to bind success state to specified result.</param>
	/// <typeparam name="TOut">The type of the result.</typeparam>
	/// <returns>The bound result.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is uninitialized (default).</exception>
	public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> next) => this switch
	{
		ResultSuccess<T> s => next(s.Data),
		ResultPending p => p,
		ResultFailure e  => e,
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
			action(s.Data);
		return this;
	}

	/// <summary>
	/// Returns the value if the result is in success state, or the specified default value if it is in pending or failure state.
	/// </summary>
	/// <param name="defaultValue">The default value to return if the result is not in success state.</param>
	/// <returns>The value if the result is in success state, or the specified default value if it is in pending or failure state.</returns>
	[return: NotNullIfNotNull(nameof(defaultValue))]
	public T GetValueOrDefault(T defaultValue = default!)
		=> Value is ResultSuccess<T> s ? s.Data : defaultValue;

	/// <summary>
	/// Returns the value if the result is in success state, or throws an exception if it is in pending or failure state.
	/// </summary>
	/// <returns>The value if the result is in success state.</returns>
	/// <exception cref="InvalidOperationException">Thrown when the result is pending, has a failure, or is uninitialized.</exception>
	/// <exception cref="Exception">Thrown when the result has a failure with an exception.</exception>
	public T GetValueOrThrow() => this switch
	{
		ResultSuccess<T> s => s.Data,
		ResultPending => throw new InvalidOperationException("Operation is pending, value is not available"),
		ResultFailure { Exception: { } ex } => throw ex,
		ResultFailure e => throw new InvalidOperationException($"Operation failed: {e.Message}"),
		null => throw new InvalidOperationException("Uninitialized Result (default)")
	};
}

#endif