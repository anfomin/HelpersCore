namespace HelpersCore;

/// <summary>
/// Provides methods for retrying operations.
/// </summary>
public static class Retry
{
	/// <summary>
	/// Invokes the specified asynchronous <paramref name="action"/>, retrying it on <see cref="Exception"/> and <paramref name="retryWhen"/> returns <c>true</c>.
	/// If max retry count is exceeded, the last <see cref="Exception"/> is thrown.
	/// </summary>
	/// <param name="action">Action to invoke.</param>
	/// <param name="retryCount">Max retry count.</param>
	/// <param name="retryDelay">Delay before retrying the action.</param>
	/// <param name="retryWhen">Condition to determine whether to retry on the given <see cref="Exception"/>.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <typeparam name="T">Action return type.</typeparam>
	public static async Task<T> InvokeAsync<T>(
		Func<CancellationToken, Task<T>> action,
		int retryCount,
		TimeSpan retryDelay,
		Func<Exception, bool> retryWhen,
		CancellationToken cancellationToken = default)
	{
		int attempts = 0;
		do
		{
			attempts++;
			try
			{
				return await action(cancellationToken);
			}
			catch (Exception ex) when (ex is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
			{
				if (retryWhen(ex) && attempts <= retryCount)
				{
					await Task.Delay(retryDelay, cancellationToken);
					continue;
				}
				throw;
			}
		}
		while (true);
	}

	/// <summary>
	/// Invokes the specified asynchronous <paramref name="action"/>, retrying it on any <see cref="Exception"/>.
	/// If max retry count is exceeded, the last <see cref="Exception"/> is thrown.
	/// </summary>
	/// <param name="action">Action to invoke.</param>
	/// <param name="retryCount">Max retry count.</param>
	/// <param name="retryDelay">Delay before retrying the action.</param>
	/// <param name="cancellationToken">Token to cancel the operation.</param>
	/// <typeparam name="T">Action return type.</typeparam>
	public static async Task<T> InvokeAsync<T>(
		Func<CancellationToken, Task<T>> action,
		int retryCount,
		TimeSpan retryDelay,
		CancellationToken cancellationToken = default)
		=> await InvokeAsync(
			action,
			retryCount,
			retryDelay,
			_ => true,
			cancellationToken);
}