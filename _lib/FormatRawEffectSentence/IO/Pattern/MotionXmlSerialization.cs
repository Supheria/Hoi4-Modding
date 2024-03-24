using FormatRawEffectSentence.LocalSign;
using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot(nameof(Motion))]
public class MotionXmlSerialization : XmlSerialization<Motion>
{
    private const string LocalNameMotion = "Motion";

    public MotionXmlSerialization() : base(nameof(Motion))
    {
    }

    public override void ReadXml(XmlReader reader)
    {
        var partIndex = reader.GetAttribute(nameof(Source.PartIndex)).ToInt() ?? -1;
        var pattern = reader.GetAttribute(nameof(Source.Pattern)) ?? "";
        Source = new(partIndex, pattern);
        if (partIndex is -1)
            Source.ConditionMap[""] = reader.GetAttribute(LocalNameMotion).ToEnum<Motions>();
        else
            Source.ConditionMap.ReadXmlCollection(reader, LocalRootName, new MotionConditionXmlSerialization());
    }

    public override void WriteXml(XmlWriter writer)
    {
        if (Source is null)
            return;
        writer.WriteAttributeString(nameof(Source.Pattern), Source.Pattern);
        writer.WriteAttributeString(nameof(Source.PartIndex), Source.PartIndex.ToString());
        if (Source.PartIndex is -1)
            writer.WriteAttributeString(LocalNameMotion, Source.ConditionMap[""].ToString());
        else
            Source.ConditionMap.WriteXmlCollection(writer, new MotionConditionXmlSerialization());
    }
}