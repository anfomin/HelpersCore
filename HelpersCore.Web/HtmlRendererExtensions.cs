using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace HelpersCore;

/// <summary>
/// Provides extension methods for <see cref="HtmlRenderer"/>.
/// </summary>
public static class HtmlRendererExtensions
{
	extension(HtmlRenderer htmlRenderer)
	{
		/// <summary>
		/// Creates an instance of the specified component and renders it to HTML using renderer dispatcher.
		/// </summary>
		/// <param name="componentType">The component type.</param>
		/// <param name="parameters">Parameters for the component.</param>
		/// <returns>HTML string.</returns>
		public Task<string> RenderComponentToHtmlAsync(Type componentType, Dictionary<string, object?>? parameters = null)
			=> htmlRenderer.Dispatcher.InvokeAsync(async () =>
			{
				var parameterView = parameters is null ? ParameterView.Empty : ParameterView.FromDictionary(parameters);
				var root = await htmlRenderer.RenderComponentAsync(componentType, parameterView);
				return root.ToHtmlString();
			});

		/// <summary>
		/// Creates an instance of the specified component and renders it to HTML using renderer dispatcher.
		/// </summary>
		/// <typeparam name="TComponent">The component type.</typeparam>
		/// <param name="parameters">Parameters for the component.</param>
		/// <returns>HTML string.</returns>
		public Task<string> RenderComponentToHtmlAsync<TComponent>(Dictionary<string, object?>? parameters = null)
			where TComponent : IComponent
			=> htmlRenderer.Dispatcher.InvokeAsync(async () =>
			{
				var parameterView = parameters is null ? ParameterView.Empty : ParameterView.FromDictionary(parameters);
				var root = await htmlRenderer.RenderComponentAsync<TComponent>(parameterView);
				return root.ToHtmlString();
			});

		/// <summary>
		/// Creates an instance of the specified component and renders it to HTML using renderer dispatcher.
		/// </summary>
		/// <param name="componentType">The component type.</param>
		/// <param name="output">The output destination.</param>
		/// <param name="parameters">Parameters for the component.</param>
		public Task RenderComponentToHtmlAsync(Type componentType, TextWriter output, Dictionary<string, object?>? parameters = null)
			=> htmlRenderer.Dispatcher.InvokeAsync(async () =>
			{
				var parameterView = parameters is null ? ParameterView.Empty : ParameterView.FromDictionary(parameters);
				var root = await htmlRenderer.RenderComponentAsync(componentType, parameterView);
				root.WriteHtmlTo(output);
			});

		/// <summary>
		/// Creates an instance of the specified component and renders it to HTML using renderer dispatcher.
		/// </summary>
		/// <typeparam name="TComponent">The component type.</typeparam>
		/// <param name="output">The output destination.</param>
		/// <param name="parameters">Parameters for the component.</param>
		public Task RenderComponentToHtmlAsync<TComponent>(TextWriter output, Dictionary<string, object?>? parameters = null)
			where TComponent : IComponent
			=> htmlRenderer.Dispatcher.InvokeAsync(async () =>
			{
				var parameterView = parameters is null ? ParameterView.Empty : ParameterView.FromDictionary(parameters);
				var root = await htmlRenderer.RenderComponentAsync<TComponent>(parameterView);
				root.WriteHtmlTo(output);
			});

		/// <summary>
		/// Creates a component builder for the specified component type.
		/// </summary>
		public ComponentBuilder<TComponent> Build<TComponent>()
			where TComponent : IComponent
			=> new(htmlRenderer);
	}
}