namespace HelpersCore;

public static class StreamExtensions
{
	/// <summary>
	/// Default buffer size of 4 KB.
	/// </summary>
	public const int BufferSize = 1024 * 4; // 4 KB

	/// <summary>
	/// Copies source stream to memory stream.
	/// </summary>
	public static async Task<MemoryStream> CopyToMemoryAsync(this Stream stream, CancellationToken cancellationToken = default)
	{
		var ms = new MemoryStream();
		await stream.CopyToAsync(ms, cancellationToken);
		ms.Seek(0, SeekOrigin.Begin);
		return ms;
	}

	/// <summary>
	/// Copies source stream to destination stream with progress callback.
	/// </summary>
	/// <param name="source">Source stream.</param>
	/// <param name="destination">Destination stream.</param>
	/// <param name="progress">Copied size progress callback.</param>
	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
	public static Task CopyToAsync(this Stream source, Stream destination, Action<long> progress, CancellationToken cancellationToken = default)
		=> CopyToAsync(source, destination, BufferSize, progress, cancellationToken);

	/// <summary>
	/// Copies source stream to destination stream with progress callback.
	/// </summary>
	/// <param name="source">Source stream.</param>
	/// <param name="destination">Destination stream.</param>
	/// <param name="progress">Copied size async progress callback.</param>
	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
	public static Task CopyToAsync(this Stream source, Stream destination, Func<long, Task> progress, CancellationToken cancellationToken = default)
		=> CopyToAsync(source, destination, BufferSize, progress, cancellationToken);

	/// <summary>
	/// Copies source stream to destination stream with the specified buffer size and progress callback.
	/// </summary>
	/// <param name="source">Source stream.</param>
	/// <param name="destination">Destination stream.</param>
	/// <param name="bufferSize">Buffer size to use.</param>
	/// <param name="progress">Copied size progress callback.</param>
	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
	public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, Action<long> progress, CancellationToken cancellationToken = default)
	{
		int read;
		long total = 0;
		var buffer = new byte[bufferSize];
		while ((read = await source.ReadAsync(buffer, cancellationToken)) != 0)
		{
			await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
			total += read;
			progress(total);
		}
	}

	/// <summary>
	/// Copies source stream to destination stream with the specified buffer size and progress callback.
	/// </summary>
	/// <param name="source">Source stream.</param>
	/// <param name="destination">Destination stream.</param>
	/// <param name="bufferSize">Buffer size to use.</param>
	/// <param name="progress">Copied size async progress callback.</param>
	/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
	public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, Func<long, Task> progress, CancellationToken cancellationToken = default)
	{
		int read;
		long total = 0;
		var buffer = new byte[bufferSize];
		while ((read = await source.ReadAsync(buffer, cancellationToken)) != 0)
		{
			await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
			total += read;
			await progress(total);
		}
	}
}