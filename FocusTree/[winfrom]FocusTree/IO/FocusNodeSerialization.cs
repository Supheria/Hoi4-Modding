using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FocusTree.Data.Focus;
using FocusTree.Graph.Lattice;
using FormatRawEffectSentence;
using FormatRawEffectSentence.IO;
using LocalUtilities.GeneralSerialization;
using LocalUtilities.Interface;
using LocalUtilities.XmlUtilities;

namespace FocusTree.IO;

[XmlRoot(nameof(FocusNode))]
public class FocusNodeSerialization : IXmlSerialization<FocusNode>
{

    public FocusNode Source { get; set; }

    public string LocalName { get; } = nameof(FocusNode);

    public FocusNodeSerialization(FocusNode source) => Source = source;

    public FocusNodeSerialization() : this(new())
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        var latticedPoint = XmlReadTool.ReadArrayString(reader.GetAttribute(nameof(Source.LatticedPoint)));
        Source = new()
        {
            Id = XmlReadTool.GetIntValue(reader.GetAttribute(nameof(Source.Id))) ?? 0,
            Name = reader.GetAttribute(nameof(Source.Name)) ?? "",
            BeginWithStar = XmlReadTool.GetBoolValue(reader.GetAttribute(nameof(Source.BeginWithStar))) ?? false,
            Duration = XmlReadTool.GetIntValue(reader.GetAttribute(nameof(Source.Duration))) ?? 0,
            Description = reader.GetAttribute(nameof(Source.Description)) ?? "",
            Ps = reader.GetAttribute(nameof(Source.Ps)) ?? "",
            LatticedPoint = latticedPoint.Length > 1
                ? new(XmlReadTool.GetIntValue(latticedPoint[0]) ?? 0, XmlReadTool.GetIntValue(latticedPoint[1]) ?? 0)
                : new LatticedPoint(),
        };

        while (reader.Read())
        {
            if (reader.Name == LocalName && reader.NodeType is XmlNodeType.EndElement)
                break;
            if (reader.NodeType is not XmlNodeType.Element)
                continue;
            switch (reader.Name)
            {
                case nameof(Source.RawEffects):
                    Source.RawEffects.ReadXmlCollection(reader, nameof(Source.RawEffects),
                        new ValueXmlSerialization<string>("", str => str ?? ""));
                    break;
                case nameof(Source.Effects):
                    Source.Effects.ReadXmlCollection(reader, nameof(Source.Effects), new EffectSentenceSerialization());
                    break;
                case nameof(Source.Requires):
                    Source.Requires.ReadXmlCollection(reader, nameof(Source.Requires), new RequireSerialization());
                    break;
            }
        }
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString(nameof(Source.Id), Source.Id.ToString());
        writer.WriteAttributeString(nameof(Source.Name), Source.Name);
        writer.WriteAttributeString(nameof(Source.BeginWithStar), Source.BeginWithStar.ToString());
        writer.WriteAttributeString(nameof(Source.Duration), Source.Duration.ToString());
        writer.WriteAttributeString(nameof(Source.Description), Source.Description);
        writer.WriteAttributeString(nameof(Source.Ps), Source.Ps);
        writer.WriteAttributeString(nameof(Source.LatticedPoint),
            XmlWriteTool.WriteArrayString(new[]
                { Source.LatticedPoint.Col.ToString(), Source.LatticedPoint.Row.ToString() }));

        Source.RawEffects.WriteXmlCollection(writer, nameof(Source.RawEffects), new ValueXmlSerialization<string>(""));
        //FormatRawEffects(Source.RawEffects, Source.Id);
        Source.Effects.WriteXmlCollection(writer, nameof(Source.Effects), new EffectSentenceSerialization());
        Source.Requires.WriteXmlCollection(writer, nameof(Source.Requires), new RequireSerialization());
    }

    //[Obsolete("临时使用，作为转换语句格式的过渡")]
    //private void FormatRawEffects(List<string> rawEffects, int id)
    //{
    //    foreach (var raw in rawEffects)
    //    {
    //        Program.TestInfo.Total++;
    //        if (!RawEffectSentenceFormatter.Format(raw, out var formattedList))
    //        {
    //                Program.TestInfo.Error++;
    //                Program.TestInfo.Good = Program.TestInfo.Total - Program.TestInfo.Error;
    //                Program.TestInfo.Append($"[{id}] {raw}");
    //            continue;
    //        }
    //        foreach (var formatted in formattedList)
    //        {
    //            _effects.Add(formatted);
    //        }
    //    }
    //}
}