using System.Xml.Linq;

namespace HelpersCore;

public static class XmlExtensions
{
	/// <summary>
	/// Returns the child elements of this <see cref="XContainer"/> that match <paramref name="localName"/>.
	/// </summary>
	/// <param name="localName">Child local name to match.</param>
	public static IEnumerable<XElement> ElementsLocal(this XElement element, string localName)
	{
		var name = element.GetDefaultNamespace() + localName;
		return element.Elements(name);
	}

	/// <summary>
	/// Returns the child element with matching <paramref name="localName"/>.
	/// </summary>
	/// <param name="localName">Child local name to match.</param>
	public static XElement? ElementLocal(this XElement element, string localName)
	{
		var name = element.GetDefaultNamespace() + localName;
		return element.Element(name);
	}
}