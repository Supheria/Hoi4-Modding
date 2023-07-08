using FormatRawEffectSentence.IO.Pattern;
using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.ManageUtilities;
using System.Text;

namespace FormatRawEffectSentence.Data;

internal class RawPatternArrayUtilities
{
    internal const string NumericValue = @"[\d]+\.?[\d]*%?";
    internal const string CollectZnChar = @"[\u4e00-\u9fa5]";
    internal const string CollectZnMark = @"[？《》]";

    internal static string AddClausesString => new StringBuilder().AppendJoin('|', AddClauses).ToString();
    internal static readonly string[] AddClauses = new[]
    {
        "增加",
        "获得",
        "添加",
    };

    internal static string SubClausesString => new StringBuilder().AppendJoin('|', SubClauses).ToString();
    internal static readonly string[] SubClauses = new[]
    {
        "移除",
    };

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

    internal static RawPattern[] LoadRawPatternArray(string filePath)
    {
#if DEBUG
        return //new RawPatternArrayXmlSerialization().LoadFromXml(filePath) ?? new LocalRawPatternArray().Patterns;
            new LocalRawPatternArray().Patterns;
#else
         return new RawPatternArraySerialization().LoadFromXml(filePath).Source ?? LocalRawPatternArray.Patterns;
#endif
    }

    internal static void SaveRawPatternArray(string filePath, ref RawPattern[] patterns)
    {
        patterns.SaveToXml(filePath, new RawPatternArrayXmlSerialization());
    }
}