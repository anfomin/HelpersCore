using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace HelpersCore;

public static class ServicesExtensions
{
	extension(IServiceCollection services)
	{
		/// <summary>
		/// Adds an <see cref="HtmlRenderer"/> for rendering components to HTML strings.
		/// </summary>
		public IServiceCollection AddHtmlRenderer()
		{
			services.AddScoped<HtmlRenderer>();
			return services;
		}
	}
}