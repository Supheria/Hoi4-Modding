using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FormatRawEffectSentence.InternalSign;
using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.GeneralSerialization;
using LocalUtilities.Interface;
using LocalUtilities.XmlUtilities;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot(nameof(MotionValue))]
public class MotionValueSerialization : IXmlSerialization<MotionValue>
{
    public MotionValue Source { get; set; }

    public string LocalName { get; } = nameof(MotionValue);

    public MotionValueSerialization(MotionValue source) => Source = source;

    public MotionValueSerialization() : this(new())
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        var valueType = (Types?)XmlReadTool.GetEnumValue<Types>(reader.GetAttribute(nameof(Source.Type))) ?? Types.None;
        Source = new(valueType);
        Source.PartIndexOrder.ReadXmlCollection(reader, nameof(Source.PartIndexOrder),
            new ValueXmlSerialization<int>(0, str => XmlReadTool.GetIntValue(str) ?? 0));
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString(nameof(Source.Type), Source.Type.ToString());
        Source.PartIndexOrder.WriteXmlCollection(writer, nameof(Source.PartIndexOrder),
            new ValueXmlSerialization<int>(0));
    }
}