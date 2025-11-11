namespace HelpersCore;

public static partial class Extensions
{
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