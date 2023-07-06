using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FormatRawEffectSentence.InternalSign;
using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.GeneralSerialization;
using LocalUtilities.Interface;
using LocalUtilities.XmlUtilities;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot(nameof(Motion))]
public class MotionSerialization : IXmlSerialization<Motion>
{
    public Motion Source { get; set; }

    public string LocalName { get; } = nameof(Motion);
    
    private const string LocalNameCondition = "Condition";
    private const string LocalNameMotion = "Motion";

    public MotionSerialization(Motion source) => Source = source;

    public MotionSerialization() : this(new())
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        var partIndex = XmlReadTool.GetIntValue(reader.GetAttribute(nameof(Source.PartIndex))) ?? -1;
        var pattern = reader.GetAttribute(nameof(Source.PartIndex)) ?? "";
        Source = new(partIndex, pattern);
        Source.ConditionMap.ReadXmlCollection(reader, nameof(Source.ConditionMap),
            new KeyValuePairXmlSerialization<string, Motions>("", Motions.None, LocalNameCondition, LocalNameMotion,
                str => str ?? "", str => (Motions?)XmlReadTool.GetEnumValue<Motions>(str) ?? Motions.None));
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString(nameof(Source.PartIndex), Source.PartIndex.ToString());
        writer.WriteAttributeString(nameof(Source.Pattern), Source.Pattern);
        Source.ConditionMap.WriteXmlCollection(writer, nameof(Source.ConditionMap),
            new KeyValuePairXmlSerialization<string, Motions>("", Motions.None, LocalNameCondition, LocalNameMotion));
    }
}