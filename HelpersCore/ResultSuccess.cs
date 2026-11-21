namespace HelpersCore;

#if NET11_0_OR_GREATER

/// <summary>
/// Represents operation result with success state.
/// </summary>
public record ResultSuccess
{
	internal ResultSuccess() { }
}

/// <summary>
/// Represents operation result with success state and some data.
/// </summary>
/// <typeparam name="T">Result data type.</typeparam>
public sealed record ResultSuccess<T> : ResultSuccess
{
	/// <summary>
	/// Gets the value of the result in success state.
	/// </summary>
	public T Data { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ResultSuccess{T}"/> class with the specified value.
	/// </summary>
	/// <param name="data">The value of the result.</param>
	internal ResultSuccess(T data)
	{
		Data = data;
	}

	/// <summary>
	/// Implicitly converts a <see cref="ResultSuccess{T}"/> to its underlying value of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="result">The <see cref="ResultSuccess{T}"/> instance to convert.</param>
	/// <returns>The underlying value of type <typeparamref name="T"/>.</returns>
	public static implicit operator T(ResultSuccess<T> result)
		=> result.Data;
}

#endif