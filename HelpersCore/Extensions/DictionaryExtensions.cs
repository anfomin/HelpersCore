using System.Collections;
using System.Reflection;

namespace HelpersCore;

/// <summary>
/// Provides extensions for dictionaries.
/// </summary>
public static class DictionaryExtensions
{
	extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
	{
		/// <summary>
		/// Gets value by key. If key does not present then creates value using factory.
		/// </summary>
		/// <param name="key">Key to get or create value for.</param>
		/// <param name="factory">Factory function to create value.</param>
		public TValue GetOrCreate(TKey key, Func<TKey, TValue> factory)
			=> dictionary.TryGetValue(key, out TValue? value) ? value
				: dictionary[key] = factory(key);

		/// <summary>
		/// Gets value by key. If key does not present then creates value using factory.
		/// </summary>
		/// <param name="key">Key to get or create value for.</param>
		/// <param name="factory">Factory function to create value.</param>
		public async ValueTask<TValue> GetOrCreateAsync(TKey key, Func<TKey, Task<TValue>> factory)
			=> dictionary.TryGetValue(key, out TValue? value) ? value
				: dictionary[key] = await factory(key);

		/// <summary>
		/// Gets value by key. If key does not present then creates value using factory.
		/// </summary>
		/// <param name="key">Key to get or create value for.</param>
		/// <param name="factory">Factory function to create value.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		public async ValueTask<TValue> GetOrCreateAsync(TKey key, Func<TKey, CancellationToken, Task<TValue>> factory, CancellationToken cancellationToken)
			=> dictionary.TryGetValue(key, out TValue? value) ? value
				: dictionary[key] = await factory(key, cancellationToken);

		/// <summary>
		/// Updates dictionary with key/values.
		/// </summary>
		public void Set(params IEnumerable<(TKey Key, TValue Value)> keyValues)
		{
			foreach (var (key, value) in keyValues)
				dictionary[key] = value;
		}
	}

	/// <summary>
	/// Updates dictionary value. If <paramref name="value" /> is <c>null</c> then key is removed from dictionary.
	/// </summary>
	public static void SetOrRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue? value)
		where TValue : class
	{
		if (value is null)
			dictionary.Remove(key);
		else
			dictionary[key] = value;
	}

	/// <summary>
	/// Updates dictionary with key/values from object public properties. Property name becomes dictionary key.
	/// </summary>
	/// <param name="obj">Object to get properties from.</param>
	/// <param name="removeNulls"><c>True</c> to remove <c>null</c> values from dictionary.</param>
	public static void Set(this IDictionary<string, object?> dictionary, object? obj, bool removeNulls = false)
	{
		if (obj is IDictionary objDic)
		{
			foreach (object key in objDic.Keys)
			{
				string keyStr = key.ToString() ?? throw new ArgumentException("Dictionary key must be non-null", nameof(obj));
				object? value = objDic[key];
				if (value is null && removeNulls)
					dictionary.Remove(keyStr);
				else
					dictionary[keyStr] = value;
			}
		}
		else if (obj is not null)
		{
			foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty))
			{
				object? value = prop.GetValue(obj);
				if (value is null && removeNulls)
					dictionary.Remove(prop.Name);
				else
					dictionary[prop.Name] = value;
			}
		}
	}

	/// <summary>
	/// Provides <see cref="KeyValuePair{TKey,TValue}"/> deconstruction.
	/// </summary>
	public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
	{
		key = pair.Key;
		value = pair.Value;
	}

	/// <summary>
	/// Creates dictionary where keys are object public properties.
	/// </summary>
	public static IDictionary<string, object?> ToPropertiesDictionary(this object obj)
	{
		if (obj is IDictionary<string, object?> objDic)
			return objDic;
		Dictionary<string, object?> dic = [];
		dic.Set(obj);
		return dic;
	}
}