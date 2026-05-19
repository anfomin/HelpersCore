using Microsoft.AspNetCore.Http;

namespace HelpersCore;

/// <summary>
/// Provides extension methods for <see cref="TypedResults"/>.
/// </summary>
public static class ResultsExtensions
{
	extension(TypedResults)
	{
		/// <summary>
		/// Creates a new <see cref="HelpersCore.OkIfModified{TValue}"/> with the specified <paramref name="value"/> and <paramref name="lastModified"/>.
		/// </summary>
		/// <param name="value">Object result.</param>
		/// <param name="lastModified">Object last modification timestamp.</param>
		public static OkIfModified<TValue> OkIfModified<TValue>(TValue? value, DateTime lastModified)
			=> new(value, lastModified);

		/// <summary>
		/// Creates a new <see cref="HelpersCore.OkIfModified{TValue}"/> with the specified <paramref name="value"/> and its timestamp.
		/// </summary>
		/// <param name="value">Object result.</param>
		public static OkIfModified<TValue> OkIfModified<TValue>(TValue value)
			where TValue : ITimestamped
			=> new(value, value.Timestamp);

		/// <summary>
		/// Creates a new <see cref="GatewayTimeout"/>.
		/// </summary>
		public static GatewayTimeout GatewayTimeout()
			=> new();
	}
}