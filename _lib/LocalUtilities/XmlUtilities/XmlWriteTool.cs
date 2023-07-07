using LocalUtilities.Interface;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LocalUtilities.XmlUtilities;

public static class XmlWriteTool
{
    public static string WritePair(string item1, string item2) => $"({item1}){XmlGeneralMark.ArraySplitter}({item2})";

    public static string WriteArrayString(string[] elements)
    {
        var sb = new StringBuilder();
        foreach (var e in elements)
        {
            sb.Append(e + XmlGeneralMark.ArraySplitter);
        }
        var str = sb.ToString().Trim();
        if (!str.EndsWith(XmlGeneralMark.ArraySplitter))
            return str;
        if (str.Length >= 1)
            str = str[..^1];
        return str;
    }

    public static void WriteXmlCollection<T>(this ICollection<T> collection, XmlWriter writer, string collectionName,
        IXmlSerialization<T> itemSerialization)
    {
        if (collectionName is "")
            WriteXmlCollection(collection, writer, itemSerialization);
        else if (collection.Count is 0)
            return;
        writer.WriteStartElement(collectionName);
        WriteXmlCollection(collection, writer, itemSerialization);
        writer.WriteEndElement();
    }

    public static void WriteXmlCollection<T>(this ICollection<T> collection, XmlWriter writer,
        IXmlSerialization<T> itemSerialization)
    {
        foreach (var item in collection)
        {
            itemSerialization.Source = item;
            itemSerialization.Serialize(writer);
        }
    }

    public static void Serialize<T>(this ISerialization<T> serialization, XmlWriter writer)
    {
        XmlSerializer serializer = new(serialization.GetType());
        serializer.Serialize(writer, serialization);
    }
}