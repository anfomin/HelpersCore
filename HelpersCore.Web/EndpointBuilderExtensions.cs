using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HelpersCore;

/// <summary>
/// Provides extension methods for <see cref="IEndpointRouteBuilder"/>.
/// </summary>
public static class EndpointBuilderExtensions
{
	/// <param name="endpoints">The <see cref="IEndpointRouteBuilder" /> to add the route to.</param>
	extension(IEndpointRouteBuilder endpoints)
	{
		/// <summary>
		/// Adds a <see cref="RouteEndpoint" /> to the <see cref="IEndpointRouteBuilder" /> that matches HTTP GET or HEAD requests
		/// for the specified pattern.
		/// </summary>
		/// <param name="pattern">The route pattern.</param>
		/// <param name="requestDelegate">The delegate executed when the endpoint is matched.</param>
		/// <returns>A <see cref="RouteHandlerBuilder" /> that can be used to further customize the endpoint.</returns>
		public IEndpointConventionBuilder MapGetOrHead([StringSyntax("Route")]string pattern, RequestDelegate requestDelegate)
			=> endpoints.MapMethods(pattern, [HttpMethods.Get, HttpMethods.Head], requestDelegate);

		/// <summary>
		/// Adds a <see cref="RouteEndpoint" /> to the <see cref="IEndpointRouteBuilder" /> that matches HTTP GET or HEAD requests
		/// for the specified pattern.
		/// </summary>
		/// <param name="pattern">The route pattern.</param>
		/// <param name="handler">The delegate executed when the endpoint is matched.</param>
		/// <returns>A <see cref="RouteHandlerBuilder" /> that can be used to further customize the endpoint.</returns>
		public IEndpointConventionBuilder MapGetOrHead([StringSyntax("Route")]string pattern, Delegate handler)
			=> endpoints.MapMethods(pattern, [HttpMethods.Get, HttpMethods.Head], handler);
	}
}