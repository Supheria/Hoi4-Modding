using LocalUtilities.Interface;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;
using System;
using System.Resources;

namespace LocalUtilities.GeneralSerialization;

[XmlRoot("Item")]
public class ValueXmlSerialization<T> : IXmlSerialization<T?>
{
    public T? Source { get; set; }

    public string LocalName { get; set; } = "Item";

    public Func<string?, T>? ReadValue { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <param name="readValue">nullable, null use for WriteXml</param>
    public ValueXmlSerialization(T? defaultValue, Func<string?, T>? readValue = null)
    {
        Source = defaultValue;
        ReadValue = readValue;
    }

    public ValueXmlSerialization()
    {
        Source = default;
        ReadValue = null;
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.Read();
        Source = ReadValue is null ? Source : ReadValue(reader.Value);
    }

    public void WriteXml(XmlWriter writer)
    {
        if (Source is not null)
            writer.WriteValue(Source);
    }
}