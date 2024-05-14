using FormatRawEffectSentence.LocalSign;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeBundle;

namespace FormatRawEffectSentence.Model.Pattern;

public class Motion : ISsSerializable
{
    internal MotionCondition Conditions { get; private set; } = new();

    /// <summary>
    /// index of part of motion in the pattern, -1 means to none
    /// </summary>
    internal int PartIndex { get; private set; }

    internal string Pattern { get; private set; }

    internal Motions this[string condition]
    {
        get => Conditions.Map.TryGetValue(condition, out var motion) ? motion.Count is not 0 ? motion[0] : Motions.None : Motions.None;
        set => Conditions.Map[condition][0] = value;
    }

    public Motion(int partIndex, string pattern)
    {
        PartIndex = partIndex;
        Pattern = pattern is "" ? pattern : RegexPatternTool.ExcludeBlankInExclusiveOrUnlimitedCollection(pattern);
    }

    public Motion(int partIndex, string pattern, Dictionary<string, Motions> conditionMap) : this(partIndex, pattern)
    {
        foreach (var (condition, motion) in conditionMap)
            Conditions.Map[condition][0] = motion;
    }

    public Motion(Motions motion, string pattern) : this(-1, pattern)
    {
        Conditions.Map[""][0] = motion;
    }

    public Motion() : this(-1, "")
    {
    }

    public string LocalName { get; set; } = nameof(Motion);

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(Pattern), Pattern);
        serializer.WriteTag(nameof(PartIndex), PartIndex.ToString());
        if (PartIndex is -1)
            serializer.WriteTag(LocalName, Conditions.Map[""][0].ToString());
        else
            serializer.Serialize(Conditions);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        Pattern = deserializer.ReadTag(nameof(Pattern), s => s ?? Pattern);
        PartIndex = deserializer.ReadTag(nameof(PartIndex), s => s.ToInt(PartIndex));
        if (PartIndex is -1)
            Conditions.Map[""][0] = deserializer.ReadTag(LocalName, s => s.ToEnum(Motions.None));
        else
            Conditions = deserializer.Deserialize(Conditions);
    }
}