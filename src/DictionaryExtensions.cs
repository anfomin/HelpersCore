using System.Collections;
using System.Reflection;

namespace HelpersCore;

public static class DictionaryExtensions
{
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

	/// <summary>
	/// Gets value by key. If key does not present then creates value using factory.
	/// </summary>
	/// <param name="key">Key to get or create value for.</param>
	/// <param name="factory">Factory function to create value.</param>
	public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
	{
		if (dictionary.TryGetValue(key, out TValue? value))
			return value;
		return dictionary[key] = factory(key);
	}

	/// <summary>
	/// Gets value by key. If key does not present then creates value using factory.
	/// </summary>
	/// <param name="key">Key to get or create value for.</param>
	/// <param name="factory">Factory function to create value.</param>
	public static async ValueTask<TValue> GetOrCreateAsync<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, Task<TValue>> factory)
	{
		if (dictionary.TryGetValue(key, out TValue? value))
			return value;
		return dictionary[key] = await factory(key);
	}

	/// <summary>
	/// Gets value by key. If key does not present then creates value using factory.
	/// </summary>
	/// <param name="key">Key to get or create value for.</param>
	/// <param name="factory">Factory function to create value.</param>
	/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
	public static async ValueTask<TValue> GetOrCreateAsync<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
		TKey key,
		Func<TKey, CancellationToken, Task<TValue>> factory,
		CancellationToken cancellationToken)
	{
		if (dictionary.TryGetValue(key, out TValue? value))
			return value;
		return dictionary[key] = await factory(key, cancellationToken);
	}

	/// <summary>
	/// Updates dictionary value. If <paramref name="value" /> is null then key is removed from dictionary.
	/// </summary>
	public static void SetOrRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue? value)
		where TValue : class
	{
		if (value == null)
			dictionary.Remove(key);
		else
			dictionary[key] = value;
	}

	/// <summary>
	/// Updates dictionary with key/values.
	/// </summary>
	public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, params IEnumerable<(TKey Key, TValue Value)> keyValues)
	{
		foreach (var (key, value) in keyValues)
			dictionary[key] = value;
	}

	/// <summary>
	/// Updates dictionary with key/values from object public properties. Property name becomes dictionary key.
	/// </summary>
	/// <param name="obj">Object to get properties from.</param>
	/// <param name="removeNulls"><c>True</c> to remove null values from dictionary.</param>
	public static void Set(this IDictionary<string, object?> dictionary, object? obj, bool removeNulls = false)
	{
		if (obj is IDictionary objDic)
		{
			foreach (object key in objDic.Keys)
			{
				string keyStr = key.ToString() ?? throw new ArgumentException("Dictionary key must be non-null", nameof(obj));
				object? value = objDic[key];
				if (value == null && removeNulls)
					dictionary.Remove(keyStr);
				else
					dictionary[keyStr] = value;
			}
		}
		else if (obj != null)
		{
			foreach (var prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty))
			{
				object? value = prop.GetValue(obj);
				if (value == null && removeNulls)
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
}