using System.Xml;
using System.Xml.Serialization;

namespace LocalUtilities.SerializeUtilities;

public static class SerializeTool
{
    public static void WriteXmlCollection<T>(this ICollection<T> collection, XmlWriter writer, string collectionName,
        IXmlSerialization<T> itemSerialization)
    {
        if (collectionName is "")
            collection.WriteXmlCollection(writer, itemSerialization);
        else if (collection.Count is 0)
            return;
        writer.WriteStartElement(collectionName);
        collection.WriteXmlCollection(writer, itemSerialization);
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