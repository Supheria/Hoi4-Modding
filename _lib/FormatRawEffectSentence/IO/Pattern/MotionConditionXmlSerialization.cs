using LocalUtilities.SerializeUtilities;
using LocalUtilities.SerializeUtilities.Interface;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FormatRawEffectSentence.LocalSign;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot("Condition")]
public class MotionConditionXmlSerialization : Serialization<KeyValuePair<string, Motions>>, IXmlSerialization<KeyValuePair<string, Motions>>
{
    private const string LocalNamePartIndex = "Pattern";
    private const string LocalNameMotion = "Motion";

    public MotionConditionXmlSerialization() : base("Condition")
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        var condition = reader.GetAttribute(LocalNamePartIndex) ?? "";
        var motion = SimpleTypeTool.GetEnumValue<Motions>(reader.GetAttribute(LocalNameMotion));
        Source = new(condition, motion);

    }

    public void WriteXml(XmlWriter writer)
    {
        if (Source.Value is Motions.None)
            return;
        writer.WriteAttributeString(LocalNamePartIndex, Source.Key);
        writer.WriteAttributeString(LocalNameMotion, Source.Value.ToString());
    }
}