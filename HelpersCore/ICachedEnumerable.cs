namespace HelpersCore;

/// <summary>
/// An enumerable that caches its items as they are enumerated.
/// </summary>
/// <typeparam name="T">The type of objects to enumerate.</typeparam>
public interface ICachedEnumerable<out T> : IEnumerable<T>;