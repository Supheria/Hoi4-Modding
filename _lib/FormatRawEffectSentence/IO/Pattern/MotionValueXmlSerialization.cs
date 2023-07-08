using FormatRawEffectSentence.LocalSign;
using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SerializeUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot(nameof(MotionValue))]
public class MotionValueXmlSerialization : Serialization<MotionValue>, IXmlSerialization<MotionValue>
{
    public MotionValueXmlSerialization() : base(nameof(MotionValue))
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        var valueType = SimpleTypeTool.GetEnumValue<Types>(reader.GetAttribute(nameof(Source.Type)));
        Source = new(valueType);
        Source.PartIndexOrder.ReadXmlCollection(reader, LocalRootName, new MotionValuePartIndexOrderXmlSerialization());
    }

    public void WriteXml(XmlWriter writer)
    {
        if (Source is null)
            return;
        writer.WriteAttributeString(nameof(Source.Type), Source.Type.ToString());
        Source.PartIndexOrder.WriteXmlCollection(writer, nameof(Source.PartIndexOrder),
            new MotionValuePartIndexOrderXmlSerialization());
    }
}