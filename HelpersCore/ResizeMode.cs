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
	/// Resize to fill entire area, clipping and centering image.
	/// </summary>
	Fill,

	/// <summary>
	/// Resize to fit area and add transparent background padding on sides.
	/// Resulting width and height will be equal to maximum size.
	/// </summary>
	Pad
}