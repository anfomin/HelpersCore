using System.Text;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace HelpersCore;

/// <summary>
/// Represents sitemap serializable to XML.
/// </summary>
[Serializable]
[XmlRoot("urlset", Namespace = Namespace, IsNullable = false)]
public class Sitemap : List<SitemapUrl>
{
	/// <summary>
	/// Sitemap XML namespace.
	/// </summary>
	public const string Namespace = "https://www.sitemaps.org/schemas/sitemap/0.9";

	/// <summary>
	/// Serializes sitemap to the <paramref name="output"/> in XML format.
	/// </summary>
	/// <param name="output">Output stream to serialize to.</param>
	/// <param name="encoding">Serialization encoding. UTF-8 will be used for <c>null</c>.</param>
	public void Serialize(Stream output, Encoding? encoding = null)
	{
		var serializer = new XmlSerializer(typeof(Sitemap));
		var ns = new XmlSerializerNamespaces();
		ns.Add(string.Empty, Namespace);

		var settings = new XmlWriterSettings { Encoding = encoding ?? new UTF8Encoding(false) };
		using var writer = XmlWriter.Create(output, settings);
		serializer.Serialize(writer, this, ns);
	}

	/// <summary>
	/// Serializes sitemap to XML string.
	/// </summary>
	public string ToXml()
	{
		var encoding = new UTF8Encoding(false);
		using var ms = new MemoryStream();
		Serialize(ms, encoding);
		return encoding.GetString(ms.ToArray());
	}
}

/// <summary>
/// Represents sitemap URL entry.
/// </summary>
[Serializable]
[XmlRoot("url"), XmlType("url")]
public record SitemapUrl()
{
	/// <summary>
	/// Absolute URL location.
	/// </summary>
	[XmlElement("loc")]
	public string? Location { get; init; }

	/// <summary>
	/// DateTime of last modification.
	/// </summary>
	[XmlIgnore, JsonIgnore]
	public DateTime? LastMod { get; init; }

	/// <summary>
	/// Date of last modification.
	/// </summary>
	[XmlIgnore, JsonIgnore]
	public DateOnly? LastModDate { get; init; }

	/// <summary>
	/// Date of last modification as string.
	/// </summary>
	[XmlElement("lastmod")]
	public string? LastModString
	{
		get => LastMod?.ToString("o") ?? LastModDate?.ToString("o");
		init
		{
			LastMod = null;
			LastModDate = value == null ? null : DateOnly.Parse(value);
		}
	}

	/// <summary>
	/// Approximate change frequency.
	/// </summary>
	[XmlElement("changefreq")]
	public SitemapFrequency? ChangeFrequency { get; init; }

	/// <summary>
	/// Page priority from 0.0 to 1.0.
	/// </summary>
	[XmlElement("priority")]
	public double? Priority { get; init; }

	/// <summary>
	/// Initializes sitemap URL with specified location.
	/// </summary>
	/// <param name="location">Absolute URL location.</param>
	public SitemapUrl(string? location) : this()
		=> Location = location;

	/// <summary>
	/// Returns whether to serialize <see cref="ChangeFrequency"/> property.
	/// </summary>
	public bool ShouldSerializeChangeFrequency() => ChangeFrequency.HasValue;

	/// <summary>
	/// Returns whether to serialize <see cref="Priority"/> property.
	/// </summary>
	public bool ShouldSerializePriority() => Priority.HasValue;
}

/// <summary>
/// Sitemap page approximate change frequency.
/// </summary>
[Serializable]
public enum SitemapFrequency
{
	/// <summary>
	/// Page is changed every time.
	/// </summary>
	[XmlEnum(Name = "always")]
	Always,

	/// <summary>
	/// Page is changed hourly.
	/// </summary>
	[XmlEnum(Name = "hourly")]
	Hourly,

	/// <summary>
	/// Page is changed daily.
	/// </summary>
	[XmlEnum(Name = "daily")]
	Daily,

	/// <summary>
	/// Page is changed weekly.
	/// </summary>
	[XmlEnum(Name = "weekly")]
	Weekly,

	/// <summary>
	/// Page is changed monthly.
	/// </summary>
	[XmlEnum(Name = "monthly")]
	Monthly,

	/// <summary>
	/// Page is changed yearly.
	/// </summary>
	[XmlEnum(Name = "yearly")]
	Yearly,

	/// <summary>
	/// Page is never changed.
	/// </summary>
	[XmlEnum(Name = "never")]
	Never
}