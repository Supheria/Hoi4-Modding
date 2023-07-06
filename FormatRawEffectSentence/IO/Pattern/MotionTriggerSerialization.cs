using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FormatRawEffectSentence.InternalSign;
using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.Interface;
using LocalUtilities.XmlUtilities;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot(nameof(MotionTrigger))]
public class MotionTriggerSerialization : IXmlSerialization<MotionTrigger>
{
    public MotionTrigger Source { get; set; }

    public string LocalName { get; } = nameof(MotionTrigger);

    public MotionTriggerSerialization(MotionTrigger source) => Source = source;

    public MotionTriggerSerialization() : this(new())
    {
    }
    
    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        var type = (Types?)XmlReadTool.GetEnumValue<Types>(reader.GetAttribute(nameof(Source.Type))) ?? Types.None;
        var pattern = reader.GetAttribute(nameof(Source.Pattern)) ?? "";
        Source = new(type, pattern);
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString(nameof(Source.Type), Source.Type.ToString());
        writer.WriteAttributeString(nameof(Source.Pattern), Source.Pattern);
    }
}