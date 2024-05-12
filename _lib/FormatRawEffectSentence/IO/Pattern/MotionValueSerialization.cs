using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.StringUtilities;

namespace FormatRawEffectSentence.IO.Pattern;

public class MotionValueSerialization : SsSerialization<MotionValue>
{
    public override string LocalName => nameof(MotionValue);

    protected override void Serialize()
    {
        WriteTag(nameof(Source.Type), Source.Type.ToString());
        WriteTag(nameof(Source.PartIndexOrder), Source.PartIndexOrder.ToArrayString());
    }

    protected override void Deserialize()
    {
        var type = ReadTag(nameof(Source.Type), s => s.ToEnum(Source.Type));
        Source = new(type);
        var array = ReadTag(nameof(Source.PartIndexOrder), s => s.ToCollection(s => s.ToInt(null)));
        array?.ForEach(x => Source.PartIndexOrder.Add(x));
    }
}