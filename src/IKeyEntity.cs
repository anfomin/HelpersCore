namespace HelpersCore;

/// <summary>
/// Represents entity with identity.
/// </summary>
public interface IKeyEntity<TKey>
	where TKey : notnull
{
	/// <summary>
	/// Gets entity identity.
	/// </summary>
	TKey Id { get; }
}

/// <summary>
/// Represents entity with integer identity field.
/// </summary>
public interface IKeyEntity : IKeyEntity<int>;

public static class KeyEntityExtensions
{
	/// <summary>
	/// Returns system representation "{ToString} &lt;ID:{Id}&gt;".
	/// </summary>
	public static string ToSystem<TKey>(this IKeyEntity<TKey> entity)
		where TKey : notnull
		=> $"{entity} <{entity.Id}>";
}