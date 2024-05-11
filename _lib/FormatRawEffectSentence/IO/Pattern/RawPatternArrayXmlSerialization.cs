using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.SimpleScript.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

public class RawPatternArrayXmlSerialization : SsSerialization<List<RawPattern>>
{
    public override string LocalName => "RawPatterns";

    public RawPatternArrayXmlSerialization()
    {
        OnSerialize += Serialize;
        OnDeserialize += Deserialize;
    }

    private void Serialize()
    {
        foreach (var item in Source)
            Serialize(item, new RawPatternXmlSerialization());
    }

    private void Deserialize()
    {
        var serialization = new RawPatternXmlSerialization();
        Deserialize(serialization.LocalName, token =>
        {
            if (serialization.Deserialize(token))
                Source.Add(serialization.Source);
        });
    }
}