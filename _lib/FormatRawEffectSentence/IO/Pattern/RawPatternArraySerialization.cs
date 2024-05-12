using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SimpleScript.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

public class RawPatternArraySerialization : SsSerialization<List<RawPattern>>
{
    public override string LocalName => "RawPatterns";

    protected override void Serialize()
    {
        Serialize(Source, new RawPatternSerialization());
    }

    protected override void Deserialize()
    {
        Deserialize(new RawPatternSerialization(), Source);
    }
}