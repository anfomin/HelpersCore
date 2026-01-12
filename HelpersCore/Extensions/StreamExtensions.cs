namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="Stream"/>.
/// </summary>
public static class StreamExtensions
{
	extension(Stream source)
	{
		/// <summary>
		/// Default buffer size of 4 KB.
		/// </summary>
		public static int DefaultBufferSize => 1024 * 4; // 4 KB

		/// <summary>
		/// Copies source stream to memory stream.
		/// </summary>
		public async Task<MemoryStream> CopyToMemoryAsync(CancellationToken cancellationToken = default)
		{
			var ms = new MemoryStream();
			await source.CopyToAsync(ms, cancellationToken);
			ms.Seek(0, SeekOrigin.Begin);
			return ms;
		}

		/// <summary>
		/// Copies source stream to destination stream with progress callback.
		/// </summary>
		/// <param name="destination">Destination stream.</param>
		/// <param name="progress">Copied size progress callback.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
		public Task CopyToAsync(Stream destination, Action<long> progress, CancellationToken cancellationToken = default)
			=> source.CopyToAsync(destination, Stream.DefaultBufferSize, progress, cancellationToken);

		/// <summary>
		/// Copies source stream to destination stream with progress callback.
		/// </summary>
		/// <param name="destination">Destination stream.</param>
		/// <param name="progress">Copied size async progress callback.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
		public Task CopyToAsync(Stream destination, Func<long, Task> progress, CancellationToken cancellationToken = default)
			=> source.CopyToAsync(destination, Stream.DefaultBufferSize, progress, cancellationToken);

		/// <summary>
		/// Copies source stream to destination stream with the specified buffer size and progress callback.
		/// </summary>
		/// <param name="destination">Destination stream.</param>
		/// <param name="bufferSize">Buffer size to use.</param>
		/// <param name="progress">Copied size progress callback.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
		public async Task CopyToAsync(Stream destination, int bufferSize, Action<long> progress, CancellationToken cancellationToken = default)
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
		/// <param name="destination">Destination stream.</param>
		/// <param name="bufferSize">Buffer size to use.</param>
		/// <param name="progress">Copied size async progress callback.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
		public async Task CopyToAsync(Stream destination, int bufferSize, Func<long, Task> progress, CancellationToken cancellationToken = default)
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
}