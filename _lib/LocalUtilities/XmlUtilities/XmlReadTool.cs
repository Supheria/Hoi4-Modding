using LocalUtilities.Interface;
using LocalUtilities.RegexUtilities;
using System.Xml;
using System.Xml.Serialization;

namespace LocalUtilities.XmlUtilities;

public static class XmlReadTool
{
    public static (T1, T2) ReadPair<T1, T2>(string? pair, (T1, T2) defaultTuple, Func<string, T1> toItem1,
        Func<string, T2> toItem2)
    {
        if (pair is null)
            return defaultTuple;
        return RegexMatchTool.GetMatchIgnoreAllBlacks(pair, @$"\((.*)\){XmlGeneralMark.ArraySplitter}\((.*)\)",
            out var match)
            ? (toItem1(match.Groups[1].Value), toItem2(match.Groups[2].Value))
            : defaultTuple;
    }

    public static string[] ReadArrayString(string? str) => str is null
        ? Array.Empty<string>()
        : str.Split(XmlGeneralMark.ArraySplitter).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

    public static void ReadXmlCollection<T>(this ICollection<T> collection, XmlReader reader, string collectionName,
        IXmlSerialization<T> itemSerialization)
    {
        // 子节点探针
        if (reader.ReadToDescendant(itemSerialization.LocalRootName) is false)
            return;
        do
        {
            if (reader.Name == collectionName && reader.NodeType is XmlNodeType.EndElement)
                return;
            if (reader.Name != itemSerialization.LocalRootName || reader.NodeType is not XmlNodeType.Element)
                continue;
            var item = itemSerialization.Deserialize(reader);
            if (item is null)
                return;
            collection.Add(item);
        } while (reader.Read());
        throw new($"读取 {itemSerialization.LocalRootName} 时未能找到结束标签");
    }

    public static void ReadXmlCollection<TKey, TValue>(this IDictionary<TKey, TValue> collection, XmlReader reader,
        string collectionName, IXmlSerialization<KeyValuePair<TKey, TValue>> itemSerialization)
    {
        // 子节点探针
        if (reader.ReadToDescendant(itemSerialization.LocalRootName) is false)
            return;
        do
        {
            if (reader.Name == collectionName && reader.NodeType is XmlNodeType.EndElement)
                return;
            if (reader.Name != itemSerialization.LocalRootName || reader.NodeType is not XmlNodeType.Element)
                continue;
            var item = itemSerialization.Deserialize(reader);
            if (item.Key is not null)
                collection[item.Key] = item.Value;
        } while (reader.Read());
        throw new($"读取 {itemSerialization.LocalRootName} 时未能找到结束标签");
    }

    public static T? Deserialize<T>(this IXmlSerialization<T> serialization, XmlReader reader)
    {
        XmlSerializer serializer = new(serialization.GetType());
        var o = serializer.Deserialize(reader);
        serialization = o as IXmlSerialization<T> ?? serialization;
        return serialization.Source;
    }

    public static T? GetEnumValue<T>(string? name) where T : Enum
    {
        try
        {
            return name is null ? default : (T)Enum.Parse(typeof(T), name);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns>int.Parse fail will return null</returns>
    public static int? GetIntValue(string? name)
    {
        try
        {
            return name is null ? null : int.Parse(name);
        }
        catch
        {
            return null;
        }
    }

    public static bool? GetBoolValue(string? name)
    {
        try
        {
            return name is null ? null : bool.Parse(name);
        }
        catch
        {
            return null;
        }
    }
}