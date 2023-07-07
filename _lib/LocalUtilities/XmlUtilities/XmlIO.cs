using LocalUtilities.Interface;
using System.Xml.Serialization;

namespace LocalUtilities.XmlUtilities;

public static class XmlIO
{
    public static void SaveToXml<T>(this T obj, string path, IXmlSerialization<T> serialization)
    {
        serialization.Source = obj;
        var file = File.Create(path);
        var writer = new XmlSerializer(serialization.GetType());
        writer.Serialize(file, serialization);
        file.Close();
    }

    public static T? LoadFromXml<T>(this IXmlSerialization<T> serialization, string path)
    {
        if (!File.Exists(path))
            return serialization.Source;
        var file = File.OpenRead(path);
        try
        {
            var reader = new XmlSerializer(serialization.GetType());
            var o = reader.Deserialize(file);
            serialization = o as IXmlSerialization<T> ?? serialization;
            file.Close();
        }
        catch
        {
            file.Close();
        }
        return serialization.Source;
    }
}