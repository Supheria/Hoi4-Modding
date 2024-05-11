using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.SimpleScript.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

public class RawPatternXmlSerialization : SsSerialization<RawPattern>
{
    public override string LocalName => nameof(RawPattern);

    public RawPatternXmlSerialization()
    {
        OnSerialize += Serialize;
        OnDeserialize += Deserialize;
    }

    private void Serialize()
    {
        //writer.WriteComment($"{(Source.IsComplex ? "complex" : "single")}: {Source.Title}");
        //foreach (var sample in Source.Samples)
        //    writer.WriteComment($"{sample}");
        Serialize(Source.Trigger, new MotionTriggerSerialization());
        Serialize(Source.Motion, new MotionSerialization());
        Serialize(Source.Value, new MotionValueSerialization());
    }

    private void Deserialize()
    {
        Source.Trigger = Deserialize(Source.Trigger, new MotionTriggerSerialization());
        Source.Motion = Deserialize(Source.Motion, new MotionSerialization());
        Source.Value = Deserialize(Source.Value, new MotionValueSerialization());
    }
}