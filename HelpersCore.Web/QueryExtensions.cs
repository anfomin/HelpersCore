using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;

namespace HelpersCore;

/// <summary>
/// Provides extension methods for query string.
/// </summary>
public static class QueryExtensions
{
	extension(QueryHelpers)
	{
		/// <summary>
		/// Returns query string from the specified URL.
		/// </summary>
		public static ReadOnlyMemory<char> GetFromUrl(string? url)
		{
			if (url is null)
				return default;

			var queryStartPos = url.IndexOf('?');
			if (queryStartPos < 0)
				return default;

			var queryEndPos = url.IndexOf('#', queryStartPos);
			return url.AsMemory(queryStartPos..(queryEndPos < 0 ? url.Length : queryEndPos));
		}
	}

	extension(QueryString)
	{
		/// <summary>
		/// Creates <see cref="QueryString"/> from <paramref name="obj"/> properties.
		/// Ignores properties with <c>null</c> values.
		/// </summary>
		/// <param name="obj">Object to get properties from.</param>
		/// <param name="camelCase">Use camel case for property names.</param>
		/// <param name="enumAsInt">Represent enum value as int.</param>
		public static QueryString FromObject(object obj, bool camelCase = true, bool enumAsInt = true)
		{
			QueryBuilder qs = new();
			foreach (var (name, value) in GetObjectKeyValues(obj, camelCase))
				qs.Add(name, GetValueString(value, enumAsInt));
			return qs.ToQueryString();
		}
	}

	static IEnumerable<KeyValuePair<string, object>> GetObjectKeyValues(object obj, bool camelCase)
	{
		foreach (var prop in obj.GetType()
			.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
			.Where(IncludeProperty))
		{
			object? value = prop.GetValue(obj);
			if (value is null)
				continue;

			var nameAttribute = prop.GetCustomAttributes().OfType<IModelNameProvider>().FirstOrDefault();
			string? name = nameAttribute?.Name;
			if (name is null)
			{
				name = prop.Name;
				if (camelCase)
					name = name.Uncapitalize();
			}

			var valueType = value.GetType();
			valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;
			if (valueType.IsSimpleType()
				|| value is IEnumerable
				|| value is IFormattable)
				yield return new(name, value);
			else
			{
				foreach (var (subName, subValue) in GetObjectKeyValues(value, camelCase))
					yield return new($"{name}.{subName}", subValue);
			}
		}
	}

	static bool IncludeProperty(PropertyInfo property)
		=> property.CanRead && property.GetCustomAttribute<JsonIgnoreAttribute>() is not { Condition: JsonIgnoreCondition.Always };

	static string GetValueString(object? value, bool enumAsInt) => value switch
	{
		DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
		DateOnly date => date.ToString("O", CultureInfo.InvariantCulture),
		TimeOnly time => time.ToString("O", CultureInfo.InvariantCulture),
		TimeSpan time => time.ToString("c", CultureInfo.InvariantCulture),
		Enum enm when enumAsInt => Convert.ToInt32(enm).ToString(),
		IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
		IEnumerable enumerable => enumerable
			.Cast<object?>()
			.Select(v => GetValueString(v, enumAsInt))
			.Join(UrlHelper.ArraySeparator),
		object => string.Format(CultureInfo.InvariantCulture, "{0}", value),
		_ => UrlHelper.NullValue
	};
}