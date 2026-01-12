namespace HelpersCore;

/// <summary>
/// Invokes custom dispose method when disposing.
/// </summary>
public class Disposer(Action disposing) : IDisposable
{
	/// <inheritdoc />
	public void Dispose()
		=> disposing();
}

/// <summary>
/// Invokes custom async dispose method when disposing.
/// </summary>
public class DisposerAsync(Func<ValueTask> disposing) : IAsyncDisposable
{
	/// <inheritdoc />
	public ValueTask DisposeAsync()
		=> disposing();
}