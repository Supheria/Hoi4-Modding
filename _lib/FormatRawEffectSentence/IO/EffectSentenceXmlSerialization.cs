using FormatRawEffectSentence.LocalSign;
using FormatRawEffectSentence.Model;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO;

public class EffectSentenceXmlSerialization : SsSerialization<EffectSentence>
{
    protected static string LocalNameType => "Type";

    protected static string LocalNameValue => "Value";

    const string SubSentence = "Sub";

    public override string LocalName { get; }

    public EffectSentenceXmlSerialization(string localName) : base((new()))
    {
        LocalName = localName;
        OnSerialize += EffectSentence_Serialize;
        OnDeserialize += EffectSentence_Deserialize;
    }

    private void EffectSentence_Serialize()
    {
        WriteTag(nameof(Source.Motion), Source.Motion.ToString());
        WriteTag(nameof(Source.ValueType), Source.ValueType.ToString());
        WriteTag(nameof(Source.Value), Source.Value.ToArrayString());
        WriteTag(nameof(Source.TriggerType), Source.TriggerType.ToString());
        WriteTag(nameof(Source.Triggers), Source.Triggers.ToArrayString());
        Serialize(Source.SubSentences, new EffectSentenceXmlSerialization(SubSentence));
    }

    private void EffectSentence_Deserialize()
    {
        var motion = ReadTag(nameof(Source.Motion), s => s.ToEnum(Source.Motion));
        var valueType = ReadTag(nameof(Source.ValueType), s => s.ToEnum(Source.ValueType));
        var value = ReadTag(nameof(Source.Value), s => s ?? Source.Value);
        var triggerType = ReadTag(nameof(Source.TriggerType), s=>s.ToEnum(Source.TriggerType));
        var triggers = ReadTag(nameof(Source.Triggers), s => s ?? Source.Triggers.ToArrayString()).ToArray();
        Source = new(motion, valueType, value, triggerType, triggers);
    }
}