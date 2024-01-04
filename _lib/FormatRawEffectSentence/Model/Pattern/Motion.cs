using FormatRawEffectSentence.LocalSign;
using LocalUtilities.StringUtilities;

namespace FormatRawEffectSentence.Model.Pattern;

public class Motion
{
    internal Dictionary<string, Motions> ConditionMap { get; } = new() { [""] = Motions.None };

    /// <summary>
    /// index of part of motion in the pattern, -1 means to none
    /// </summary>
    internal int PartIndex { get; }

    internal string Pattern { get; }

    internal Motions this[string condition]
    {
        get => ConditionMap.TryGetValue(condition, out var motion) ? motion : Motions.None;
        set => ConditionMap[condition] = value;
    }

    public Motion(int partIndex, string pattern)
    {
        PartIndex = partIndex;
        Pattern = pattern is "" ? pattern : RegexPatternTool.ExcludeBlankInExclusiveOrUnlimitedCollection(pattern);
    }

    public Motion(int partIndex, string pattern, Dictionary<string, Motions> conditionMap) : this(partIndex, pattern)
    {
        foreach (var (condition, motion) in conditionMap)
            ConditionMap[condition] = motion;
    }

    public Motion(Motions motion, string pattern) : this(-1, pattern)
    {
        ConditionMap[""] = motion;
    }

    public Motion() : this(-1, "")
    {
    }
}