using FormatRawEffectSentence.InternalSign;
using LocalUtilities.XmlUtilities;
using System.Xml;
using FormatRawEffectSentence.Model;
using System;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;
using LocalUtilities.Interface;

namespace FormatRawEffectSentence.IO;

[XmlRoot("Effect")]
public class EffectSentenceSerialization : IXmlSerialization<EffectSentence>
{
    public string LocalName { get; } = "Effect";

    private const string LocalNameSub = "Sub";
    private const string LocalNameType = "Type";
    private const string LocalNameValue = "Value";

    public EffectSentence Source { get; set; } = new();

    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        var motion = XmlReadTool.GetEnumValue<Motions>(reader.GetAttribute(nameof(Source.Motion))) as Motions? ?? Motions.None;
        var typePair = reader.GetAttribute(LocalNameType);
        var valuePair = reader.GetAttribute(LocalNameValue);
        var (valueType, triggerType) = XmlReadTool.ReadPair(typePair, (Types.None, Types.None),
            str => XmlReadTool.GetEnumValue<Types>(str) as Types? ?? Types.None,
            str => XmlReadTool.GetEnumValue<Types>(str) as Types? ?? Types.None);
        var (value, triggers) = XmlReadTool.ReadPair(valuePair, ("", Array.Empty<string>()), 
            str => str, XmlReadTool.ReadArrayString);
        Source = new(motion, valueType, value, triggerType, triggers);

        Source.SubSentences.ReadXmlCollection(reader, LocalNameSub, new EffectSentenceSerialization());
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString(nameof(Source.Motion), Source.Motion.ToString());
        writer.WriteAttributeString(LocalNameType,
            XmlWriteTool.WritePair(Source.Type.ToString(), Source.TriggerType.ToString()));
        writer.WriteAttributeString(LocalNameValue,
            XmlWriteTool.WritePair(Source.Value, XmlWriteTool.WriteArrayString(Source.Triggers)));

        Source.SubSentences.WriteXmlCollection(writer, LocalNameSub, new EffectSentenceSerialization());
    }

    public override string ToString()
    {
        var sb = new StringBuilder().AppendLine(SentenceToString());
        foreach (var sub in Source.SubSentences)
            sb.AppendLine(new EffectSentenceSerialization { Source = sub }.ToString(1));
        return sb.ToString();
    }

    private string ToString(int tabTime)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < tabTime; i++)
            sb.Append('\t');
        sb.Append(SentenceToString());
        foreach (var sub in Source.SubSentences)
            sb.Append($"\n{new EffectSentenceSerialization { Source = sub }.ToString(tabTime + 1)}");
        return sb.ToString();
    }

    private string SentenceToString() =>
        $"{nameof(Source.Motion)}=\"{Source.Motion}\", {LocalNameType}=\"{TypePairToString()}\", {LocalNameValue}=\"{ValuePairToString()}\"";

    private string TypePairToString() => $"({Source.Type}),({Source.TriggerType})";

    private string ValuePairToString() => $"({Source.Value}),({XmlWriteTool.WriteArrayString(Source.Triggers)})";
}