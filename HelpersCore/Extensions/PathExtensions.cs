namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="Path"/> and <see cref="FileInfo"/>.
/// </summary>
public static class PathExtensions
{
	extension(Path)
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
	}

	extension(FileInfo file)
	{
		/// <summary>
		/// Returns <see cref="FileInfo"/> with changed extension.
		/// </summary>
		/// <param name="extension">New file extension.</param>
		public FileInfo ChangeExtension(string? extension)
			=> new(Path.ChangeExtension(file.FullName, extension));
	}
}