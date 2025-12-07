using System.Xml.Serialization;

namespace Edgar.Net.Data.Forms;

/// <summary>
/// Represents the result of a form list query from EDGAR.
/// </summary>
[XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
public class FormListResult
{
    [XmlElement("title")]
    public string? Title { get; set; }

    [XmlElement("id")]
    public string? Id { get; set; }

    [XmlElement("author")]
    public FormListAuthor? Author { get; set; }

    [XmlElement("updated")]
    public DateTime Updated { get; set; }

    [XmlElement("entry")]
    public List<FormListEntry> Entries { get; set; } = [];
}

/// <summary>
/// Represents a single form entry in the results.
/// </summary>
[XmlRoot("entry")]
public class FormListEntry
{
    [XmlElement("title")]
    public string? Title { get; set; }

    [XmlElement("link")]
    public FormLink FileLink { get; set; } = new();

    [XmlElement("summary")]
    public string? Summary { get; set; }
}

/// <summary>
/// Represents a link to a form filing.
/// </summary>
public class FormLink
{
    [XmlAttribute("rel")]
    public string? RelativePath { get; set; }

    [XmlAttribute("type")]
    public string? DataType { get; set; }

    [XmlAttribute("href")]
    public string Url { get; set; } = string.Empty;
}

/// <summary>
/// Represents author information for a form list.
/// </summary>
public class FormListAuthor
{
    [XmlElement("name")]
    public string? Name { get; set; }

    [XmlElement("email")]
    public string? Email { get; set; }
}
