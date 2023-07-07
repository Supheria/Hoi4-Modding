using FormatRawEffectSentence.InternalSign;
using FormatRawEffectSentence.Model;
using LocalUtilities.Interface;
using LocalUtilities.XmlUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO;

[XmlRoot(nameof(EffectSentence))]
public class EffectSentenceXmlSerialization : EffectSentenceStringSerialization, IXmlSerialization<EffectSentence>
{
    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        var motion = XmlReadTool.GetEnumValue<Motions>(reader.GetAttribute(nameof(Source.Motion)));
        var typePair = reader.GetAttribute(LocalNameType);
        var valuePair = reader.GetAttribute(LocalNameValue);
        var (valueType, triggerType) = XmlReadTool.ReadPair(typePair, (Types.None, Types.None),
            XmlReadTool.GetEnumValue<Types>,
            XmlReadTool.GetEnumValue<Types>);
        var (value, triggers) = XmlReadTool.ReadPair(valuePair, ("", Array.Empty<string>()),
            str => str, XmlReadTool.ReadArrayString);
        Source = new(motion, valueType, value, triggerType, triggers);

        Source.SubSentences.ReadXmlCollection(reader, LocalRootName, new EffectSentenceXmlSerialization());
    }

    public void WriteXml(XmlWriter writer)
    {
        if (Source is null)
            return;
        writer.WriteAttributeString(nameof(Source.Motion), Source.Motion.ToString());
        writer.WriteAttributeString(LocalNameType,
            XmlWriteTool.WritePair(Source.Type.ToString(), Source.TriggerType.ToString()));
        writer.WriteAttributeString(LocalNameValue,
            XmlWriteTool.WritePair(Source.Value, XmlWriteTool.WriteArrayString(Source.Triggers)));

        Source.SubSentences.WriteXmlCollection(writer, new EffectSentenceXmlSerialization());
    }
}