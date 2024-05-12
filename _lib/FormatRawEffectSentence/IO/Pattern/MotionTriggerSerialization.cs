using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.StringUtilities;

namespace FormatRawEffectSentence.IO.Pattern;

public class MotionTriggerSerialization : SsSerialization<MotionTrigger>
{
    public override string LocalName => nameof(MotionTrigger);

    protected override void Serialize()
    {
        WriteTag(nameof(Source.Type), Source.Type.ToString());
        WriteTag(nameof(Source.Pattern), Source.Pattern);
    }

    protected override void Deserialize()
    {
        var type = ReadTag(nameof(Source.Type), s => s.ToEnum(Source.Type));
        var pattern = ReadTag(nameof(Source.Pattern), s => s ?? Source.Pattern);
        Source = new(type, pattern);
    }
}