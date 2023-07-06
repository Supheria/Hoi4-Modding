using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using LocalUtilities.Interface;
using LocalUtilities.RegexUtilities;

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

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="reader"></param>
    /// <param name="collectionName"></param>
    /// <param name="itemSerialization"></param>
    public static void ReadXmlCollection<T>(this ICollection<T> collection, XmlReader reader, string collectionName, IXmlSerialization<T> itemSerialization)
    {
        // 子节点探针
        if (reader.ReadToDescendant(itemSerialization.LocalName) is false)
            return;
        do
        {
            if (reader.Name == collectionName && reader.NodeType is XmlNodeType.EndElement)
                return;
            if (reader.Name != itemSerialization.LocalName || reader.NodeType is not XmlNodeType.Element) 
                continue;
            if (reader.Deserialize(ref itemSerialization))
                collection.Add(itemSerialization.Source);
        } while (reader.Read());
        throw new($"读取 {itemSerialization.LocalName} 时未能找到结束标签");
    }

    public static bool Deserialize<T>(this XmlReader reader, ref T obj) where T : IXmlSerializable
    {
        XmlSerializer serializer = new(obj.GetType());
        if (serializer.Deserialize(reader) is not T o) 
            return false;
        obj = o;
        return true;
    }

    //public static void ReadCollection<TKey, TValue>(XmlReader reader, Dictionary<TKey, TValue> collection, string collectionName,
    //    string itemName, Func<XmlReader, (TKey, TValue)> addItem) where TKey : notnull

    //{
    //    // 子节点探针
    //    if (reader.ReadToDescendant(itemName) is false)
    //        return;
    //    do
    //    {
    //        if (reader.Name == collectionName && reader.NodeType is XmlNodeType.EndElement)
    //            return;
    //        if (reader.Name != itemName || reader.NodeType is not XmlNodeType.Element)
    //            continue;
    //        var (key, value) = addItem(reader);
    //        collection[key] = value;
    //    } while (reader.Read());
    //    throw new($"读取 {collectionName} 时未能找到结束标签");
    //}

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