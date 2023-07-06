using System.Xml.Serialization;
using LocalUtilities.Interface;

namespace LocalUtilities.XmlUtilities;

public static class XmlIO
{
    /// <summary>
    /// 将 FGraph 序列化成 xml
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="path">保存路径</param>
    public static void SaveToXml<T>(this T? obj, string path) where T : IXmlSerializable
    {
        if (obj is null)
            return;
        var file = File.Create(path);
        var writer = new XmlSerializer(typeof(T));
        writer.Serialize(file, obj);
        file.Close();
    }

    /// <summary>
    /// 从 xml 文件中反序列化 FGraph
    /// </summary>
    /// <param name="serialization"></param>
    /// <param name="path">xml文件路径</param>
    /// <returns>FGraph</returns>
    public static IXmlSerialization<T> LoadFromXml<T>(this IXmlSerialization<T> serialization, string path)
    {
        if (!File.Exists(path))
            return serialization;
        var file = File.OpenRead(path);
        try
        {
            var reader = new XmlSerializer(serialization.GetType());
            var obj = reader.Deserialize(file);
            serialization = obj as IXmlSerialization<T> ?? serialization;
            file.Close();
        }
        catch
        {
            file.Close();
        }
        return serialization;
    }
}