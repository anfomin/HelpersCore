using System.Collections;

namespace HelpersCore;

/// <summary>
/// Caches the items of an enumerable as they are enumerated.
/// </summary>
/// <param name="source">Source enumerable for caching.</param>
/// <typeparam name="T">The type of objects to enumerate.</typeparam>
internal sealed class CachedEnumerable<T>(IEnumerable<T> source) : IEnumerable<T>, IDisposable
{
	readonly IEnumerator<T> _enumerator = source.GetEnumerator();
	readonly List<T> _cache = [];
	bool _disposed;

	/// <inheritdoc />
	public IEnumerator<T> GetEnumerator()
	{
		foreach (var cached in _cache)
			yield return cached;

		if (_disposed)
			yield break;
		while (_enumerator.MoveNext())
		{
			var item = _enumerator.Current;
			_cache.Add(item);
			yield return item;
		}

		Dispose();
	}

	/// <inheritdoc />
	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	/// <inheritdoc />
	public void Dispose()
	{
		if (_disposed)
			return;
		_enumerator.Dispose();
		_disposed = true;
	}
}