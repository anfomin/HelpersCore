namespace HelpersCore;

/// <summary>
/// Provides additional path methods.
/// </summary>
public static class PathHelper
{
	/// <summary>
	/// Combine paths and then resolves full path. Supports user-relative paths starting with ~.
	/// </summary>
	public static string GetFullPath(params ReadOnlySpan<string> paths)
	{
		string combined = Path.Combine(paths);
		return combined.StartsWith("~/") || combined.StartsWith(@"~\")
			? Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), combined[2..]))
			: Path.GetFullPath(combined);
	}

	/// <summary>
	/// Splits file path into file path and extension.
	/// </summary>
	public static (string Name, string Extension) SplitExtension(string path)
	{
		string ext = Path.GetExtension(path);
		return (path[..^ext.Length], ext);
	}

	/// <summary>
	/// Returns <see cref="FileInfo"/> with changed extension.
	/// </summary>
	/// <param name="file">File to change extension of.</param>
	/// <param name="extension">New file extension.</param>
	public static FileInfo ChangeExtension(this FileInfo file, string? extension)
		=> new(Path.ChangeExtension(file.FullName, extension));
}