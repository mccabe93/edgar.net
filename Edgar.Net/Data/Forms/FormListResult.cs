using System.Xml.Serialization;

namespace Edgar.Net.Data.Forms
{
    [XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
    public class FormListResult
    {
        public FormListResult()
        {
            Entries = new List<FormListEntry>();
        }
        [XmlElement("title")]
        public string Title { get; set; }
        [XmlElement("id")]
        public string Id { get; set; }
        [XmlElement("author")]
        public FormListAuthor Author { get; set; }
        [XmlElement("updated")]
        public DateTime Updated { get; set; }
        [XmlElement("entry")]
        public List<FormListEntry> Entries { get; set; }
    }

    [XmlRoot("entry")]
    public class FormListEntry
    {
        [XmlElement("title")]
        public string Title { get; set; }
        [XmlElement("link")]
        public FormLink FileLink { get; set; }
        [XmlElement("summary")]
        public string Summary { get; set; }
    }

    public class FormLink
    {
        [XmlAttribute("rel")]
        public string RelativePath { get; set; }
        [XmlAttribute("type")]
        public string DataType { get; set; }
        [XmlAttribute("href")]
        public string Url { get; set; }
    }

    public class FormListAuthor
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("email")]
        public string Email { get; set; }
    }
}
