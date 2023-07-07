using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.SerializeUtilities.Interface;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FormatRawEffectSentence.LocalSign;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot(nameof(MotionTrigger))]
public class MotionTriggerXmlSerialization : Serialization<MotionTrigger>, IXmlSerialization<MotionTrigger>
{
    public MotionTriggerXmlSerialization() : base(nameof(MotionTrigger))
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        var type = SimpleTypeTool.GetEnumValue<Types>(reader.GetAttribute(nameof(Source.Type)));
        var pattern = reader.GetAttribute(nameof(Source.Pattern)) ?? "";
        Source = new(type, pattern);
    }

    public void WriteXml(XmlWriter writer)
    {
        if (Source is null)
            return;
        writer.WriteAttributeString(nameof(Source.Type), Source.Type.ToString());
        writer.WriteAttributeString(nameof(Source.Pattern), Source.Pattern);
    }
}