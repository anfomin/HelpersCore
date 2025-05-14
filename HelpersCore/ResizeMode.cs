namespace HelpersCore;

/// <summary>
/// Represents different image resize modes.
/// </summary>
public enum ResizeMode
{
	/// <summary>
	/// Resize to fit area.
	/// </summary>
	Fit,

	/// <summary>
	/// Resize to fill entire area centering image.
	/// </summary>
	Fill,

	/// <summary>
	/// Resize to fit area and add background padding on sides.
	/// </summary>
	Pad
}