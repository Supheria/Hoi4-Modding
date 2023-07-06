using System.Text;
using System.Xml;

namespace XmlUtilities;

public class XmlWriteTool
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

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="writer"></param>
    /// <param name="collection"></param>
    /// <param name="collectionName">if is "", start element of name collectionName won't be written</param>
    /// <param name="itemName"></param>
    /// <param name="writeComment">nullable, write comment before start element of name itemName</param>
    /// <param name="writeItem"></param>
    public static void WriteCollection<T>(XmlWriter writer, ICollection<T> collection, string collectionName,
        string itemName, Action<XmlWriter, T> writeItem, Action<XmlWriter, T>? writeComment = null)

    {
        if (collectionName is not "")
            writer.WriteStartElement(collectionName);
        foreach (var item in collection)
        {
            writeComment?.Invoke(writer, item);
            writer.WriteStartElement(itemName);
            writeItem(writer, item);
            writer.WriteEndElement();
        }
        if (collectionName is not "")
            writer.WriteEndElement();
    }
}