using System.Text.Json;
using System.Text.Json.Nodes;

namespace HelpersCore;

/// <summary>
/// Provides extensions for JSON.
/// </summary>
public static class JsonExtensions
{
	/// <summary>
	/// Merges <paramref name="source"/> into <paramref name="node"/>.
	/// </summary>
	/// <param name="node">Destination node to merge to.</param>
	/// <param name="source">Source node to merge.</param>
	/// <param name="redefine"><c>True</c> to override existing properties. Otherwise <c>False</c>.</param>
	public static void Merge(this JsonNode node, JsonNode source, bool redefine)
	{
		switch (node)
		{
			case JsonObject obj:
				if (source is not JsonObject sourceObj)
					throw new JsonException($"Can not merge {source.GetValueKind()} into Object");
				foreach (var (key, sourceValue) in sourceObj)
				{
					var current = obj[key];
					if (current is JsonObject && sourceValue is JsonObject
						|| current is JsonArray && sourceValue is JsonArray)
						current.Merge(sourceValue, redefine);
					else if (current is null || redefine)
						obj[key] = sourceValue?.DeepClone();
				}
				break;
			case JsonArray array:
				if (source is not JsonArray sourceArray)
					throw new JsonException($"Can not merge {source.GetValueKind()} into Array");
				foreach (var sourceValue in sourceArray)
				{
					if (array.None(item => JsonNode.DeepEquals(item, sourceValue)))
						array.Add(sourceValue?.DeepClone());
				}
				break;
			case JsonValue value:
				throw new NotSupportedException($"Can not merge {value.GetValueKind()}");
			default:
				throw new NotSupportedException($"Json {node.GetValueKind()} not supported");
		}
	}
}