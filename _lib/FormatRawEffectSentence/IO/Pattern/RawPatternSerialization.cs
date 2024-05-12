using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SimpleScript.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

public class RawPatternSerialization : SsSerialization<RawPattern>
{
    public override string LocalName => nameof(RawPattern);

    protected override void Serialize()
    {
        //writer.WriteComment($"{(Source.IsComplex ? "complex" : "single")}: {Source.Title}");
        //foreach (var sample in Source.Samples)
        //    writer.WriteComment($"{sample}");
        Serialize(Source.Trigger, new MotionTriggerSerialization());
        Serialize(Source.Motion, new MotionSerialization());
        Serialize(Source.Value, new MotionValueSerialization());
    }

    protected override void Deserialize()
    {
        Source.Trigger = Deserialize(Source.Trigger, new MotionTriggerSerialization());
        Source.Motion = Deserialize(Source.Motion, new MotionSerialization());
        Source.Value = Deserialize(Source.Value, new MotionValueSerialization());
    }
}