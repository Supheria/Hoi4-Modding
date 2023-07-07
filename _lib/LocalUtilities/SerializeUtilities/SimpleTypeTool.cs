using System.Security.Cryptography;
using System.Text;
using LocalUtilities.RegexUtilities;

namespace LocalUtilities.SerializeUtilities;

public static class SimpleTypeTool
{
    public const char ElementSplitter = ',';

    public static (T1, T2) ReadPair<T1, T2>(string? pair, (T1, T2) defaultTuple, Func<string, T1> toItem1,
        Func<string, T2> toItem2)
    {
        if (pair is null)
            return defaultTuple;
        return RegexMatchTool.GetMatchIgnoreAllBlacks(pair, @$"\((.*)\){ElementSplitter}\((.*)\)",
            out var match)
            ? (toItem1(match.Groups[1].Value), toItem2(match.Groups[2].Value))
            : defaultTuple;
    }

    public static string WritePair(string item1, string item2) => $"({item1}){ElementSplitter}({item2})";

    public static string[] ReadArrayString(string? str) => str is null
        ? Array.Empty<string>()
        : str.Split(ElementSplitter).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

    public static string WriteArrayString(string[] elements) =>
        new StringBuilder().AppendJoin(ElementSplitter, elements).ToString();


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

    public static string ToMd5HashString(this string str)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(str));
        return hash.Aggregate(string.Empty, (current, b) => current + b);
    }

    public static string ToMd5HashString(this FileStream file)
    {
        var sha = MD5.Create();
        var hash = sha.ComputeHash(file);
        return hash.Aggregate(string.Empty, (current, b) => current + b);
    }
}