using System.Text.RegularExpressions;
using System.Xml;

namespace XmlUtilities;

public class XmlReadTool
{
    public static void ReadPair<T1, T2>(string? pair, ref T1 item1, ref T2 item2, Func<string, T1> toItem1,
        Func<string, T2> toItem2)
    {
        if (pair is null)
            return;
        var match = Regex.Match(pair.Trim(), $"\\((.*)\\){XmlGeneralMark.ArraySplitter}\\((.*)\\)");
        if (!match.Success)
            return;
        item1 = toItem1(match.Groups[1].Value);
        item2 = toItem2(match.Groups[2].Value);
    }

    public static string[] ReadArrayString(string? str) => str is null
        ? Array.Empty<string>()
        : str.Split(XmlGeneralMark.ArraySplitter).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

    public static void ReadCollection<T>(XmlReader reader, List<T> collection, string collectionName,
        string itemName, Func<XmlReader, T> addItem)

    {
        // 子节点探针
        if (reader.ReadToDescendant(itemName) is false)
            return;
        do
        {
            if (reader.Name == collectionName && reader.NodeType is XmlNodeType.EndElement)
                return;
            if (reader.Name == itemName && reader.NodeType is XmlNodeType.Element)
                collection.Add(addItem(reader));
        } while (reader.Read());
        throw new($"读取 {collectionName} 时未能找到结束标签");
    }

    public static void ReadCollection<TKey, TValue>(XmlReader reader, Dictionary<TKey, TValue> collection, string collectionName,
        string itemName, Func<XmlReader, (TKey, TValue)> addItem) where TKey : notnull

    {
        // 子节点探针
        if (reader.ReadToDescendant(itemName) is false)
            return;
        do
        {
            if (reader.Name == collectionName && reader.NodeType is XmlNodeType.EndElement)
                return;
            if (reader.Name != itemName || reader.NodeType is not XmlNodeType.Element)
                continue;
            var (key, value) = addItem(reader);
            collection[key] = value;
        } while (reader.Read());
        throw new($"读取 {collectionName} 时未能找到结束标签");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns>Enum.Parse fail will return null</returns>
    public static object? GetEnumValue<T>(string? name)
    {
        try
        {
            return name is null ? null : Enum.Parse(typeof(T), name);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns>int.Parse fail will return -1</returns>
    public static int GetIntValue(string? name)
    {
        try
        {
            return name is null ? -1 : int.Parse(name);
        }
        catch
        {
            return -1;
        }
    }
}