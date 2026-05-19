using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;

namespace HelpersCore;

/// <summary>
/// An <see cref="Microsoft.AspNetCore.Http.IResult" /> that on execution will write
/// an object to the response with Gateway Timeout (504) status code.
/// </summary>
public class GatewayTimeout : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult
{
	public int? StatusCode => 504;

	public Task ExecuteAsync(HttpContext httpContext)
	{
		httpContext.Response.StatusCode = 504;
		return Task.CompletedTask;
	}

	public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
	{
		builder.Metadata.Add(new ProducesResponseTypeMetadata(504, typeof(void)));
	}
}