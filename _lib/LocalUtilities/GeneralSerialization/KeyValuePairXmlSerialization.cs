using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LocalUtilities.Interface;

namespace LocalUtilities.GeneralSerialization;

[XmlRoot("Item")]
public class KeyValuePairXmlSerialization<TKey, TValue> : IXmlSerialization<KeyValuePair<TKey, TValue>>
    where TKey : notnull where TValue : notnull
{

    public KeyValuePair<TKey, TValue> Source { get; set; }

    public string LocalName { get; set; } = "Item";

    public TKey DefaultKey { get; }

    public TValue DefaultValue { get; }

    public string KeyName { get; }

    public string ValueName { get; }

    public Func<string?, TKey>? ReadKey { get; }

    public Func<string?, TValue>? ReadValue { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="defaultKey"></param>
    /// <param name="defaultValue"></param>
    /// <param name="keyName"></param>
    /// <param name="valueName"></param>
    /// <param name="readKey">nullable, null use for WriteXml</param>
    /// <param name="readValue">nullable, null use for WriteXml</param>
    public KeyValuePairXmlSerialization(TKey defaultKey, TValue defaultValue, string keyName, string valueName, Func<string?, TKey>? readKey = null, Func<string?, TValue>? readValue = null)
    {
        DefaultKey = defaultKey;
        DefaultValue = defaultValue;
        KeyName = keyName;
        ValueName = valueName;
        ReadKey = readKey;
        ReadValue = readValue;
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        var key = ReadKey is null ? DefaultKey : ReadKey(reader.GetAttribute(KeyName));
        var value = ReadValue is null ? DefaultValue : ReadValue(reader.GetAttribute(ValueName));
        Source = new(key, value);
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString(KeyName, Source.Key.ToString());
        writer.WriteAttributeString(ValueName, Source.Value.ToString());
    }
}