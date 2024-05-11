using FormatRawEffectSentence.LocalSign;
using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

public class MotionTriggerSerialization : SsSerialization<MotionTrigger>
{
    public override string LocalName => nameof(MotionTrigger);

    public MotionTriggerSerialization()
    {
        OnSerialize += Serialize;
        OnDeserialize += Deserialize;
    }

    private void Serialize()
    {
        WriteTag(nameof(Source.Type), Source.Type.ToString());
        WriteTag(nameof(Source.Pattern), Source.Pattern);
    }

    private void Deserialize()
    {
        var type = ReadTag(nameof(Source.Type), s => s.ToEnum(Source.Type));
        var pattern = ReadTag(nameof(Source.Pattern), s => s ?? Source.Pattern);
        Source = new(type, pattern);
    }
}