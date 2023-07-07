using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.GeneralSerialization;
using LocalUtilities.Interface;
using LocalUtilities.XmlUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot(nameof(RawPattern))]
public class RawPatternXmlSerialization : Serialization<RawPattern>, IXmlSerialization<RawPattern>
{
    public RawPatternXmlSerialization() : base(nameof(RawPattern))
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        Source = new();
        do
        {
            if (reader.Name == LocalRootName && reader.NodeType is XmlNodeType.EndElement)
                break;
            if (reader.NodeType is not XmlNodeType.Element)
                continue;
            if (reader.Name == new MotionTriggerXmlSerialization().LocalRootName)
            {
                Source.Trigger = new MotionTriggerXmlSerialization().Deserialize(reader) ?? new();
            }
            else if (reader.Name == new MotionXmlSerialization().LocalRootName)
            {
                Source.Motion = new MotionXmlSerialization().Deserialize(reader) ?? new();
            }
            else if (reader.Name == new MotionValueXmlSerialization().LocalRootName)
            {
                Source.Value = new MotionValueXmlSerialization().Deserialize(reader) ?? new();
            }
        } while (reader.Read());
    }

    public void WriteXml(XmlWriter writer)
    {
        if (Source is null)
            return;
        writer.WriteComment($"{(Source.IsComplex ? "complex" : "single")}: {Source.Title}");
        foreach (var sample in Source.Samples)
            writer.WriteComment($"{sample}");

        new MotionTriggerXmlSerialization { Source = Source.Trigger }.Serialize(writer);
        new MotionXmlSerialization { Source = Source.Motion }.Serialize(writer);
        new MotionValueXmlSerialization { Source = Source.Value }.Serialize(writer);
    }
}