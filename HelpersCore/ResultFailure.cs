using System.Collections.Immutable;

namespace HelpersCore;

#if NET11_0_OR_GREATER

/// <summary>
/// Represents operation result with failure state.
/// </summary>
public sealed record ResultFailure
{
	/// <summary>
	/// Gets exception if operation has failed with exception.
	/// </summary>
	public Exception? Exception { get; }

	/// <summary>
	/// Gets failure messages.
	/// </summary>
	public ImmutableArray<string> Messages { get; }

	/// <summary>
	/// Gets first failure message.
	/// </summary>
	public string Message => Messages[0];

	/// <summary>
	/// Initializes with specified exception.
	/// </summary>
	/// <param name="ex">Exception occured.</param>
	internal ResultFailure(Exception ex)
	{
		Exception = ex;
		Messages = [ex.Message];
	}

	/// <summary>
	/// Initializes with specified failure message.
	/// </summary>
	/// <param name="message">Failure message.</param>
	internal ResultFailure(string message)
		=> Messages = [message];

	/// <summary>
	/// Initializes with specified failure messages.
	/// </summary>
	/// <param name="messages">Failure messages.</param>
	internal ResultFailure(params IEnumerable<string> messages)
	{
		Messages = messages.ToImmutableArray();
		if (Messages.Length == 0)
			throw new ArgumentException("Failure message required.", nameof(messages));
	}
}

#endif