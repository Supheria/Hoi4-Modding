using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.GeneralSerialization;
using LocalUtilities.Interface;
using LocalUtilities.XmlUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FormatRawEffectSentence.InternalSign;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot(nameof(Motion))]
public class MotionXmlSerialization : Serialization<Motion>, IXmlSerialization<Motion>
{
    private const string LocalNameMotion = "Motion";

    public MotionXmlSerialization() : base(nameof(Motion))
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        var partIndex = XmlReadTool.GetIntValue(reader.GetAttribute(nameof(Source.PartIndex))) ?? -1;
        var pattern = reader.GetAttribute(nameof(Source.Pattern)) ?? "";
        Source = new(partIndex, pattern);
        if (partIndex is -1)
            Source.ConditionMap[""] = XmlReadTool.GetEnumValue<Motions>(reader.GetAttribute(LocalNameMotion));
        else
            Source.ConditionMap.ReadXmlCollection(reader, LocalRootName, new MotionConditionXmlSerialization());
    }

    public void WriteXml(XmlWriter writer)
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