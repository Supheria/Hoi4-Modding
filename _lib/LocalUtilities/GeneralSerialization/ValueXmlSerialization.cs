using LocalUtilities.Interface;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace LocalUtilities.GeneralSerialization;

[XmlRoot("Item")]
public class ValueXmlSerialization<T> : Serialization<T>, IXmlSerialization<T>
{
    public Func<string?, T>? ReadValue { get; }

    public ValueXmlSerialization(Func<string?, T>? readValue = null) : base("Item") => ReadValue = readValue;

    public ValueXmlSerialization() : this(null)
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.Read();
        Source = ReadValue is null ? default : ReadValue(reader.Value);
    }

    public void WriteXml(XmlWriter writer)
    {
        if (Source is not null)
            writer.WriteValue(Source);
    }
}