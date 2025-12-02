namespace HelpersCore;

/// <summary>
/// A function that provides data items by request.
/// </summary>
/// <param name="request">Data items request.</param>
public delegate ValueTask<DataResult<T>> DataDelegate<T>(DataRequest<T> request);

/// <summary>
/// Represents a request to an <see cref="DataDelegate{T}"/>.
/// </summary>
/// <param name="StartIndex">The start index of the data segment requested.</param>
/// <param name="Count">The requested number of items to be provided. The actual number of provided items does not need to match this value.</param>
/// <param name="Sort">Optional sorting information.</param>
/// <param name="CancellationToken">The <see cref="CancellationToken"/> used to relay cancellation of the request.</param>
public readonly record struct DataRequest<T>(
	int StartIndex,
	int Count,
	DataSort? Sort,
	CancellationToken CancellationToken
) {
	public override string ToString()
		=> $"Request from {StartIndex}, count {Count}, sort {Sort}";
}

/// <summary>
/// Represents the result of a <see cref="DataDelegate{T}"/>.
/// </summary>
/// <param name="Items">Items for the specified <see cref="DataRequest{T}.StartIndex"/> and <see cref="DataRequest{T}.Count"/>.</param>
/// <param name="TotalItems">Total items count.</param>
public readonly record struct DataResult<T>(IEnumerable<T> Items, int TotalItems)
{
	/// <summary>
	/// Initializes a new <see cref="DataResult{T}"/> with no items and a total of <c>0</c>.
	/// </summary>
	public DataResult() : this([], 0) { }
}