namespace HelpersCore;

/// <summary>
/// Invokes custom dispose method when disposing.
/// </summary>
public class Disposer(Action onDispose) : IDisposable
{
	/// <inheritdoc />
	public void Dispose()
		=> onDispose();
}

/// <summary>
/// Invokes custom async dispose method when disposing.
/// </summary>
public class DisposerAsync(Func<ValueTask> onDispose) : IAsyncDisposable
{
	/// <inheritdoc />
	public ValueTask DisposeAsync()
		=> onDispose();
}