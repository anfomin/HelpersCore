namespace HelpersCore;

/// <summary>
/// Provides dictionary creation methods.
/// </summary>
public static class Dictionary
{
	/// <summary>
	/// Creates dictionary where keys are object public properties.
	/// </summary>
	/// <param name="obj">Object to get properties from.</param>
	public static Dictionary<string, object?> FromProperties(object obj)
	{
		Dictionary<string, object?> dic = [];
		dic.Set(obj);
		return dic;
	}
}