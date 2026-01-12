using System.Collections;

namespace HelpersCore;

/// <summary>
/// Caches the items of an enumerable as they are enumerated.
/// </summary>
/// <param name="source">Source enumerable for caching.</param>
/// <typeparam name="T">The type of objects to enumerate.</typeparam>
internal sealed class CachedEnumerable<T>(IEnumerable<T> source) : ICachedEnumerable<T>
{
	readonly IEnumerable<T> _source = source;
	readonly List<T> _cache = [];
	bool _completed;

	public IEnumerator<T> GetEnumerator()
		=> new CacheEnumerator(this);

	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	struct CacheEnumerator(CachedEnumerable<T> enumerable) : IEnumerator<T>
	{
		int _index = -1;
		IEnumerator<T>? _sourceEnumerator;

		public T Current => enumerable._cache[_index];

		object IEnumerator.Current => Current!;

		public bool MoveNext()
		{
			int indexNext = _index + 1;
			if (indexNext + 1 < enumerable._cache.Count)
			{
				_index = indexNext;
				return true;
			}
			if (enumerable._completed)
				return false;

			if (_sourceEnumerator == null)
			{
				_sourceEnumerator = enumerable._source.GetEnumerator();
				for (int i = 0; i < indexNext; i++)
				{
					if (!_sourceEnumerator.MoveNext())
						return false;
				}
			}

			if (_sourceEnumerator.MoveNext())
			{
				_index = indexNext;
				enumerable._cache.Add(_sourceEnumerator.Current);
				return true;
			}
			enumerable._completed = true;
			_sourceEnumerator.Dispose();
			_sourceEnumerator = null;
			return false;
		}

		public void Reset()
			=> _index = -1;

		public void Dispose()
			=> _sourceEnumerator?.Dispose();
	}
}