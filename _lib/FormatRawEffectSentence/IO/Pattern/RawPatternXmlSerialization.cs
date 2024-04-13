using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SerializeUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

public class RawPatternXmlSerialization() : XmlSerialization<RawPattern>(new())
{
    public override string LocalName => nameof(RawPattern);

    public override void ReadXml(XmlReader reader)
    {
        while (reader.Read())
        {
            if (reader.Name == LocalName && reader.NodeType is XmlNodeType.EndElement)
                break;
            if (reader.NodeType is not XmlNodeType.Element)
                continue;
            if (reader.Name == new MotionTriggerXmlSerialization().LocalName)
                Source.Trigger = new MotionTriggerXmlSerialization().Deserialize(reader) ?? new();
            else if (reader.Name == new MotionXmlSerialization().LocalName)
                Source.Motion = new MotionXmlSerialization().Deserialize(reader) ?? new();
            else if (reader.Name == new MotionValueXmlSerialization().LocalName)
                Source.Value = new MotionValueXmlSerialization().Deserialize(reader) ?? new();
        }
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteComment($"{(Source.IsComplex ? "complex" : "single")}: {Source.Title}");
        foreach (var sample in Source.Samples)
            writer.WriteComment($"{sample}");

        new MotionTriggerXmlSerialization { Source = Source.Trigger }.Serialize(writer);
        new MotionXmlSerialization { Source = Source.Motion }.Serialize(writer);
        new MotionValueXmlSerialization { Source = Source.Value }.Serialize(writer);
    }
}