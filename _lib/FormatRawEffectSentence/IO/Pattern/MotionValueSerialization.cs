using FormatRawEffectSentence.LocalSign;
using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

public class MotionValueSerialization : SsSerialization<MotionValue>
{
    public override string LocalName => nameof(MotionValue);

    public MotionValueSerialization()
    {
        OnSerialize += Serialize;
        OnDeserialize += Deserialize;
    }

    private void Serialize()
    {
        WriteTag(nameof(Source.Type), Source.Type.ToString());
        WriteTag(nameof(Source.PartIndexOrder), Source.PartIndexOrder.ToArrayString());
    }

    private void Deserialize()
    {
        var type = ReadTag(nameof(Source.Type), s => s.ToEnum(Source.Type));
        var array = ReadTag(nameof(Source.PartIndexOrder), s => s.ToCollection(s => s.ToInt(null)));
        array?.ForEach(x => Source.PartIndexOrder.Add(x));
    }
}