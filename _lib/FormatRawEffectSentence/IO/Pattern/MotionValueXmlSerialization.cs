using FormatRawEffectSentence.LocalSign;
using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

public class MotionValueXmlSerialization() : XmlSerialization<MotionValue>(new())
{
    public override string LocalName => nameof(MotionValue);

    public override void ReadXml(XmlReader reader)
    {
        var valueType = reader.GetAttribute(nameof(Source.Type)).ToEnum<Types>();
        Source = new(valueType);
        Source.PartIndexOrder.ReadXmlCollection(reader, LocalName, new MotionValuePartIndexOrderXmlSerialization());
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString(nameof(Source.Type), Source.Type.ToString());
        Source.PartIndexOrder.WriteXmlCollection(writer, nameof(Source.PartIndexOrder),
            new MotionValuePartIndexOrderXmlSerialization());
    }
}