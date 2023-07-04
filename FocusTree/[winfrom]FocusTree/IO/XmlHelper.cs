using FocusTree.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace FocusTree.IO;

public class XmlHelper
{
    public static readonly char Splitter = ',';
    public static void ReadPair<T1, T2>(string? pair, ref T1 item1, ref T2 item2, Func<string, T1> toItem1,
        Func<string, T2> toItem2)
    {
        if (pair is null)
            return;
        var match = Regex.Match(pair.Trim(), $"\\((.*)\\){Splitter}\\((.*)\\)");
        if (!match.Success)
            return;
        item1 = toItem1(match.Groups[1].Value);
        item2 = toItem2(match.Groups[2].Value);
    }

    public static string WritePair(string item1, string item2) => $"({item1}){Splitter}({item2})";

    public static string[] ReadArrayString(string? str) => str is null
        ? Array.Empty<string>()
        : str.Split(Splitter).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

    public static string WriteArrayString(string[] elements)
    {
        var sb = new StringBuilder();
        foreach (var e in elements)
        {
            sb.Append(e + Splitter);
        }
        var str = sb.ToString().Trim();
        if (!str.EndsWith(Splitter)) 
            return str;
        if (str.Length >= 1)
            str = str[..^1];
        return str;
    }

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

    public static void WriteCollection<T>(XmlWriter writer, ICollection<T> collection, string collectionName,
        string itemName, Action<XmlWriter, T> writeItem)

    {
        writer.WriteStartElement(collectionName);
        foreach (var item in collection)
        {
            writer.WriteStartElement(itemName);
            writeItem(writer, item);
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
    }

    /// <summary>
    /// Write items directly without collectionName starter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="writer"></param>
    /// <param name="collection"></param>
    /// <param name="itemName"></param>
    /// <param name="writeItem"></param>
    public static void WriteCollection<T>(XmlWriter writer, ICollection<T> collection, string itemName,
        Action<XmlWriter, T> writeItem)

    {
        foreach (var item in collection)
        {
            writer.WriteStartElement(itemName);
            writeItem(writer, item);
            writer.WriteEndElement();
        }
    }

    /// <summary>
    /// 通过枚举名获取对应枚举值
    /// </summary>
    /// <typeparam name="T">枚举类型</typeparam>
    /// <param name="name">枚举名</param>
    /// <returns>枚举名存在则以 object 返回枚举对象，否则返回null</returns>
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

    public static uint GetUintValue(string? name)
    {
        try
        {
            return name is null ? 0 : uint.Parse(name);
        }
        catch
        {
            return 0;
        }
    }
}