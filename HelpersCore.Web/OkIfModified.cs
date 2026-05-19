using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;

namespace HelpersCore;

/// <summary>
/// An <see cref="IResult"/> that on execution will write an object to the response with
/// Ok (200) status code. If the request has an If-Modified-Since header and the value has
/// not been modified since the specified date, the response will be Not Modified (304).
/// </summary>
public sealed class OkIfModified<TValue>(TValue? value, DateTimeOffset lastModified)
	: IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<TValue>
{
	public TValue? Value { get; } = value;

	/// <summary>
	/// Gets object last modification timestamp.
	/// </summary>
	public DateTimeOffset LastModified { get; } = lastModified;

	public int StatusCode => StatusCodes.Status200OK;

	/// <summary>
	/// Gets or sets JSON options used for serialization.
	/// </summary>
	public JsonSerializerOptions? JsonOptions { get; set; }

	int? IStatusCodeHttpResult.StatusCode => StatusCode;

	object? IValueHttpResult.Value => Value;

	public Task ExecuteAsync(HttpContext httpContext)
	{
		var response = httpContext.Response;
		response.GetTypedHeaders().LastModified = LastModified;
		if (httpContext.Request.GetTypedHeaders().IfModifiedSince is { } ifModifiedSince && LastModified.ToUnixTimeSeconds() <= ifModifiedSince.ToUnixTimeSeconds())
		{
			response.StatusCode = StatusCodes.Status304NotModified;
			return Task.CompletedTask;
		}

		response.StatusCode = StatusCode;
		if (Value is null)
			return Task.CompletedTask;
		return response.WriteAsJsonAsync<object>(Value, JsonOptions, httpContext.RequestAborted);
	}

	public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
	{
		builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status200OK, typeof(TValue), [MediaTypeNames.Application.Json]));
		builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status304NotModified));
	}
}