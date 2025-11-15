using NpgsqlTypes;

namespace HelpersCore;

/// <summary>
/// Represents bounding box for top left and bottom right points.
/// </summary>
/// <param name="TopLeft">Top left point.</param>
/// <param name="BottomRight">Bottom right point.</param>
public record struct NpgsqlBounds(NpgsqlPoint TopLeft, NpgsqlPoint BottomRight);