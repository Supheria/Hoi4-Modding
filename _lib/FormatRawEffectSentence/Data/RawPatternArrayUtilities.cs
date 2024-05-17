using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeToolKit.Text;
using System.Text;

namespace FormatRawEffectSentence.Data;

internal class RawPatternArrayUtilities
{
    internal static string NumericValue => @"[\d]+\.?[\d]*%?";

    internal static string ZnCharCollection => @"[\u4e00-\u9fa5]";

    internal static string ZnMarkCollection => @"[？《》]";

    internal static string AddSubString => CombineStringArrays(AddWords, SubWords);

    internal static string[] AddWords => new[]
    {
        "增加",
        "获得",
        "添加",
        "升高",
    };

    internal static string[] SubWords => new[]
    {
        "移除",
        "降低",
    };

    internal static string AbleUnableString => CombineStringArrays(AbleWords, UnableWords);

    internal static string[] AbleWords => new[]
    {
        "可以",
        "是",
    };

    internal static string[] UnableWords => new[]
    {
        "不能",
        "不可以",
        "否",
    };

    private static string CombineStringArrays(params string[][] stringArrays)
    {
        return new StringBuilder()
            .AppendJoin('|', stringArrays, (sb, value) =>
            {
                sb.AppendJoin('|', value);
            })
            .ToString();
    }

    internal static Dictionary<TValueToKey, TKeyToValue> ReverseDictionary<TKeyToValue, TValueToKey>(
        Dictionary<TKeyToValue, TValueToKey[]> rawDictionary)
        where TKeyToValue : notnull where TValueToKey : notnull
    {
        var reverse = new Dictionary<TValueToKey, TKeyToValue>();
        foreach (var (rawKey, rawValues) in rawDictionary)
            foreach (var rawValue in rawValues)
                reverse[rawValue] = rawKey;
        return reverse;
    }



    internal static List<RawPattern> LoadRawPatternArray(string filePath)
    {
#if DEBUG
        return //new RawPatternArrayXmlSerialization().LoadFromXml(filePath) ?? new LocalRawPatternArray().Patterns;
            new LocalRawPatternArray().Patterns;
#else
        var collection = new SerializableList<RawPattern>("RawPatterns").LoadFromSimpleScript(filePath).List;
        return collection.Count is 0 ? LocalRawPatternArray.Patterns : collection.ToList();
#endif
    }

    internal static void SaveRawPatternArray(string filePath, ref List<RawPattern> patterns)
    {
        patterns.SaveToSimpleScript(true, filePath);
    }
}