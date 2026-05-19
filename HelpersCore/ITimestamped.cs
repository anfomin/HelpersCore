namespace HelpersCore;

/// <summary>
/// An interface that represents an object with a timestamp.
/// </summary>
public interface ITimestamped
{
	/// <summary>
	/// Gets object last modified timestamp.
	/// </summary>
	DateTime Timestamp { get; }
}