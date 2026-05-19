using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace HelpersCore;

/// <summary>
/// Builder for creating and rendering non-interactive components with specified parameters.
/// </summary>
/// <typeparam name="TComponent">The component type.</typeparam>
public class ComponentBuilder<TComponent>(HtmlRenderer htmlRenderer)
	where TComponent : IComponent
{
	readonly HtmlRenderer _htmlRenderer = htmlRenderer;
	readonly Dictionary<string, object?> _parameters = new();

	/// <summary>
	/// Sets a parameter for the component.
	/// </summary>
	/// <param name="propertyExpression">Expression to select the component property.</param>
	/// <param name="value">Property value.</param>
	public ComponentBuilder<TComponent> SetParameter<TParam>(Expression<Func<TComponent, TParam>> propertyExpression, TParam value)
	{
		if (propertyExpression.Body is not MemberExpression memberExpr)
			throw new ArgumentException("Expression body must be MemberExpression", nameof(propertyExpression));
		string propertyName = memberExpr.Member.Name;
		_parameters[propertyName] = value;
		return this;
	}

	/// <summary>
	/// Creates an instance of the specified component and renders it to HTML.
	/// </summary>
	public Task<string> RenderToHtmlAsync()
		=> _htmlRenderer.RenderComponentToHtmlAsync<TComponent>(_parameters);

	/// <summary>
	/// Creates an instance of the specified component and renders it to HTML.
	/// </summary>
	/// <param name="output">The output destination.</param>
	public Task RenderToHtmlAsync(TextWriter output)
		=> _htmlRenderer.RenderComponentToHtmlAsync<TComponent>(output, _parameters);
}