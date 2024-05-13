using FormatRawEffectSentence.LocalSign;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeBundle;

namespace FormatRawEffectSentence.Model.Pattern;

public class MotionTrigger : ISsSerializable
{
    internal Types Type { get; private set; }

    internal string Pattern { get; private set; }

    public MotionTrigger(Types type, string pattern)
    {
        Type = type;
        Pattern = pattern is "" ? pattern : RegexPatternTool.ExcludeBlankInExclusiveOrUnlimitedCollection(pattern);
    }

    public MotionTrigger(Types type) : this(type, "")
    {
    }

    public MotionTrigger() : this(Types.None, "")
    {
    }

    public string LocalName { get; set; } = nameof(MotionTrigger);

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(Type), Type.ToString());
        serializer.WriteTag(nameof(Pattern), Pattern);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        Type = deserializer.ReadTag(nameof(Type), s => s.ToEnum(Type));
        Pattern = deserializer.ReadTag(nameof(Pattern), s => s ?? Pattern);
    }
}