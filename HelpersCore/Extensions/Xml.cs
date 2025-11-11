using System.Xml.Linq;

namespace HelpersCore;

public static partial class Extensions
{
	extension(XElement element)
	{
		/// <summary>
		/// Returns the child elements of this <see cref="XContainer"/> that match <paramref name="localName"/>.
		/// </summary>
		/// <param name="localName">Child local name to match.</param>
		public IEnumerable<XElement> ElementsLocal(string localName)
		{
			var name = element.GetDefaultNamespace() + localName;
			return element.Elements(name);
		}

		/// <summary>
		/// Returns the child element with matching <paramref name="localName"/>.
		/// </summary>
		/// <param name="localName">Child local name to match.</param>
		public XElement? ElementLocal(string localName)
		{
			var name = element.GetDefaultNamespace() + localName;
			return element.Element(name);
		}
	}
}