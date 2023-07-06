using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FormatRawEffectSentence.InternalSign;
using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.Interface;
using LocalUtilities.RegexUtilities;
using LocalUtilities.XmlUtilities;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot(nameof(RawPattern))]
public class RawPatternSerialization : IXmlSerialization<RawPattern>
{

    public RawPattern Source { get; set; }

    public string LocalName { get; } = nameof(RawPattern);

    public RawPatternSerialization(RawPattern source) => Source = source;

    public RawPatternSerialization() : this(new())
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        do
        {
            if (reader.Name == LocalName && reader.NodeType is XmlNodeType.EndElement)
                break;
            if (reader.NodeType is not XmlNodeType.Element)
                continue;
            if (reader.Name == new MotionTriggerSerialization().LocalName)
            {
                var serialization = new MotionTriggerSerialization();
                Source.Trigger = reader.Deserialize(ref serialization) ? serialization.Source : new();
            }
            else if (reader.Name == new MotionSerialization().LocalName)
            {
                var serialization = new MotionSerialization();
                Source.Motion = reader.Deserialize(ref serialization) ? serialization.Source : new();
            }
            else if (reader.Name == new MotionValueSerialization().LocalName)
            {
                var serialization = new MotionValueSerialization();
                Source.Value = reader.Deserialize(ref serialization) ? serialization.Source : new();
            }
        } while (reader.Read());
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteComment($"{(Source.IsComplex ? "complex" : "single")}: {Source.Title}");
        foreach (var sample in Source.Samples)
            writer.WriteComment($"{sample}");

        writer.Serialize(new MotionTriggerSerialization(Source.Trigger));
        writer.Serialize(new MotionSerialization(Source.Motion));
        writer.Serialize(new MotionValueSerialization(Source.Value));
        //writer.WriteAttributeString(nameof(Source.Trigger), Source.TriggerPattern);
        //writer.WriteAttributeString(nameof(Source.Motion), Source.MotionPattern);

        //writer.WriteElementString(nameof(Source.TriggerType), Source.TriggerType.ToString());

        //if (Source.MotionCondition.PartIndex is -1)
        //    writer.WriteElementString(nameof(Source.Motion), Source.MotionConditionMap[""].ToString());
        //else
        //{
        //    // <MotionConditionMap>
        //    writer.WriteStartElement(nameof(Source.MotionConditionPartIndex));
        //    writer.WriteAttributeString(LocalNamePartIndex, Source.MotionConditionPartIndex.ToString());
        //    Source.MotionConditionMap.WriteXmlCollection(writer, new MotionSerialization());
        //    // </MotionConditionMap>
        //    writer.WriteEndElement();
        //}

        //if (Source.Type is Types.None)
        //    return;
        //// <ValuePartIndexOrderArray>
        //writer.WriteStartElement(nameof(Source.ValuePartIndexOrder));
        //writer.WriteAttributeString(nameof(Source.Type), Source.Type.ToString());
        //XmlWriteTool.WriteCollection(writer, Source.ValuePartIndexOrderArray, "", LocalNameItem, (w, index) =>
        //{
        //    w.WriteAttributeString(LocalNamePartIndex, index.ToString());
        //});
        //// </ValuePartIndexOrderArray>
        //writer.WriteEndElement();
    }
}