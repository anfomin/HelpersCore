using System.Drawing;
using SkiaSharp;

namespace HelpersCore;

public static partial class SKExtensions
{
	extension(SKImageInfo info)
	{
		/// <summary>
		/// Returns <see cref="Size"/> for given <see cref="SKImageInfo"/>.
		/// </summary>
		public Size Size => new(info.Width, info.Height);
	}
}