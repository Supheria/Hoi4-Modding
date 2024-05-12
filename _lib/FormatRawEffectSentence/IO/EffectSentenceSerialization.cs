using FormatRawEffectSentence.Model;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.StringUtilities;

namespace FormatRawEffectSentence.IO;

public class EffectSentenceSerialization : SsSerialization<EffectSentence>
{

    public override string LocalName => "Effect";

    protected override void Serialize()
    {
        WriteTag(nameof(Source.Motion), Source.Motion.ToString());
        WriteTag(nameof(Source.ValueType), Source.ValueType.ToString());
        WriteTag(nameof(Source.Value), Source.Value);
        WriteTag(nameof(Source.TriggerType), Source.TriggerType.ToString());
        WriteTag(nameof(Source.Triggers), Source.Triggers.ToArrayString());
        Serialize(Source.SubSentences, new EffectSentenceSerialization());
    }

    protected override void Deserialize()
    {
        var motion = ReadTag(nameof(Source.Motion), s => s.ToEnum(Source.Motion));
        var valueType = ReadTag(nameof(Source.ValueType), s => s.ToEnum(Source.ValueType));
        var value = ReadTag(nameof(Source.Value), s => s ?? Source.Value);
        var triggerType = ReadTag(nameof(Source.TriggerType), s => s.ToEnum(Source.TriggerType));
        var triggers = ReadTag(nameof(Source.Triggers), s => s ?? Source.Triggers.ToArrayString()).ToArray();
        Source = new(motion, valueType, value, triggerType, triggers);
    }
}