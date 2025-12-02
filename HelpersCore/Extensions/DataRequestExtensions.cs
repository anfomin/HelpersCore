namespace HelpersCore;

/// <summary>
/// Provides extension methods for <see cref="DataRequest{T}"/>.
/// </summary>
public static class DataRequestExtensions
{
	extension<T>(IQueryable<T> query)
	{
		/// <summary>
		/// Applies skip and take according to the <paramref name="request"/>.
		/// </summary>
		public IQueryable<T> Page(DataRequest<T> request)
		{
			if (request.StartIndex > 0)
				query = query.Skip(request.StartIndex);
			return query.Take(request.Count);
		}
	}

	extension<T>(IEnumerable<T> query)
	{
		/// <summary>
		/// Applies skip and take according to the <paramref name="request"/>.
		/// </summary>
		public IEnumerable<T> Page(DataRequest<T> request)
		{
			if (request.StartIndex > 0)
				query = query.Skip(request.StartIndex);
			return query.Take(request.Count);
		}
	}
}