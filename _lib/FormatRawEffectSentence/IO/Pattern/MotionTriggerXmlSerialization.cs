using FormatRawEffectSentence.LocalSign;
using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

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
        var type = reader.GetAttribute(nameof(Source.Type)).ToEnum<Types>();
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