using FormatRawEffectSentence.LocalSign;
using FormatRawEffectSentence.Model;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.SerializeUtilities.Interface;
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
        var motion = SimpleTypeTool.GetEnumValue<Motions>(reader.GetAttribute(nameof(Source.Motion)));
        var typePair = reader.GetAttribute(LocalNameType);
        var valuePair = reader.GetAttribute(LocalNameValue);
        var (valueType, triggerType) = SimpleTypeTool.ReadPair(typePair, (Types.None, Types.None),
            SimpleTypeTool.GetEnumValue<Types>,
            SimpleTypeTool.GetEnumValue<Types>);
        var (value, triggers) = SimpleTypeTool.ReadPair(valuePair, ("", Array.Empty<string>()),
            str => str, SimpleTypeTool.ReadArrayString);
        Source = new(motion, valueType, value, triggerType, triggers);

        Source.SubSentences.ReadXmlCollection(reader, LocalRootName, new EffectSentenceXmlSerialization());
    }

    public void WriteXml(XmlWriter writer)
    {
        if (Source is null)
            return;
        writer.WriteAttributeString(nameof(Source.Motion), Source.Motion.ToString());
        writer.WriteAttributeString(LocalNameType,
            SimpleTypeTool.WritePair(Source.Type.ToString(), Source.TriggerType.ToString()));
        writer.WriteAttributeString(LocalNameValue,
            SimpleTypeTool.WritePair(Source.Value, SimpleTypeTool.WriteArrayString(Source.Triggers)));

        Source.SubSentences.WriteXmlCollection(writer, new EffectSentenceXmlSerialization());
    }
}