
#define test_format

using FocusTree.Model.Focus;
using FocusTree.Model.Lattice;
using FormatRawEffectSentence;
using FormatRawEffectSentence.IO;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.SerializeUtilities.Interface;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FocusTree.IO.Xml;

[XmlRoot(nameof(FocusNode))]
public class FocusNodeXmlSerialization : Serialization<FocusNode>, IXmlSerialization<FocusNode>
{
    public FocusNodeXmlSerialization() : base(nameof(FocusNode))
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        var latticedPoint = SimpleTypeTool.ReadArrayString(reader.GetAttribute(nameof(Source.LatticedPoint)));
        Source = new()
        {
            Id = SimpleTypeTool.GetIntValue(reader.GetAttribute(nameof(Source.Id))) ?? 0,
            Name = reader.GetAttribute(nameof(Source.Name)) ?? "",
            BeginWithStar = SimpleTypeTool.GetBoolValue(reader.GetAttribute(nameof(Source.BeginWithStar))) ?? false,
            Duration = SimpleTypeTool.GetIntValue(reader.GetAttribute(nameof(Source.Duration))) ?? 0,
            Description = reader.GetAttribute(nameof(Source.Description)) ?? "",
            Ps = reader.GetAttribute(nameof(Source.Ps)) ?? "",
            LatticedPoint = latticedPoint.Length > 1
                ? new(SimpleTypeTool.GetIntValue(latticedPoint[0]) ?? 0, SimpleTypeTool.GetIntValue(latticedPoint[1]) ?? 0)
                : new LatticedPoint(),
        };

        while (reader.Read())
        {
            if (reader.Name == LocalRootName && reader.NodeType is XmlNodeType.EndElement)
                break;
            if (reader.NodeType is not XmlNodeType.Element)
                continue;
            switch (reader.Name)
            {
                case nameof(Source.RawEffects):
                    Source.RawEffects.ReadXmlCollection(reader, nameof(Source.RawEffects),
                        new RawEffectXmlSerialization());
                    break;
                case nameof(Source.Effects):
                    Source.Effects.ReadXmlCollection(reader, nameof(Source.Effects), new EffectSentenceXmlSerialization());
                    break;
                case nameof(Source.Requires):
                    Source.Requires.ReadXmlCollection(reader, nameof(Source.Requires), new RequireXmlSerialization());
                    break;
            }
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        if (Source is null)
            return;
        writer.WriteAttributeString(nameof(Source.Id), Source.Id.ToString());
        writer.WriteAttributeString(nameof(Source.Name), Source.Name);
        writer.WriteAttributeString(nameof(Source.BeginWithStar), Source.BeginWithStar.ToString());
        writer.WriteAttributeString(nameof(Source.Duration), Source.Duration.ToString());
        writer.WriteAttributeString(nameof(Source.Description), Source.Description);
        writer.WriteAttributeString(nameof(Source.Ps), Source.Ps);
        writer.WriteAttributeString(nameof(Source.LatticedPoint),
            SimpleTypeTool.WriteArrayString(new[]
                { Source.LatticedPoint.Col.ToString(), Source.LatticedPoint.Row.ToString() }));

        Source.RawEffects.WriteXmlCollection(writer, nameof(Source.RawEffects), new RawEffectXmlSerialization());
        FormatRawEffects();
        Source.Effects.WriteXmlCollection(writer, nameof(Source.Effects), new EffectSentenceXmlSerialization());
        Source.Requires.WriteXmlCollection(writer, nameof(Source.Requires), new RequireXmlSerialization());
    }

    [Obsolete("临时使用，作为转换语句格式的过渡")]
    private void FormatRawEffects()
    {
#if test_format
        Source!.Effects.Clear();
        foreach (var raw in Source!.RawEffects)
        {
            Program.TestInfo.Total++;
            if (!RawEffectSentenceFormatter.Format(raw, out var formattedList))
            {
                Program.TestInfo.Error++;
                Program.TestInfo.Good = Program.TestInfo.Total - Program.TestInfo.Error;
                Program.TestInfo.Append($"[{Source.Id}] {raw}");
                continue;
            }
            foreach (var formatted in formattedList)
            {
                Source!.Effects.Add(formatted);
            }
        }
#endif
    }
}