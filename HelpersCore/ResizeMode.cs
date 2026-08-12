namespace HelpersCore;

/// <summary>
/// Represents different image resize modes.
/// </summary>
public enum ResizeMode
{
	/// <summary>
	/// Resize to fit area.
	/// Resulting width and height will be less than or equal to specified size.
	/// </summary>
	Fit,

	/// <summary>
	/// Resize to fill entire area.
	/// Resulting width or height can be greater than specified size.
	/// </summary>
	Fill,

	/// <summary>
	/// Resize to fit area and add background padding on sides.
	/// Resulting width and height will be equal to specified size.
	/// </summary>
	Pad
}