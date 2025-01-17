﻿
#define test_format

using FocusTree.Model.Focus;
using FocusTree.Model.Lattice;
using FormatRawEffectSentence;
using FormatRawEffectSentence.IO;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Serialization;

namespace FocusTree.IO.Xml;

public class FocusNodeXmlSerialization() : XmlSerialization<FocusNode>(new())
{
    public override string LocalName => nameof(FocusNode);

    public override void ReadXml(XmlReader reader)
    {
        Source.SetSignature = reader.GetAttribute(nameof(Source.Signature)).ToInt() ?? 0;
        Source.Name = reader.GetAttribute(nameof(Source.Name)) ?? "";
        Source.BeginWithStar = reader.GetAttribute(nameof(Source.BeginWithStar)).ToBool() ?? false;
        Source.Duration = reader.GetAttribute(nameof(Source.Duration)).ToInt() ?? 0;
        Source.Description = reader.GetAttribute(nameof(Source.Description)) ?? "";
        Source.Ps = reader.GetAttribute(nameof(Source.Ps)) ?? "";
        var latticedPoint = reader.GetAttribute(nameof(Source.LatticedPoint)).ToArray();
        Source.LatticedPoint = latticedPoint.Length > 1
            ? new()
            {
                Col = latticedPoint[0].ToInt() ?? 0,
                Row = latticedPoint[1].ToInt() ?? 0
            }
            : new();

        while (reader.Read())
        {
            if (reader.Name == LocalName && reader.NodeType is XmlNodeType.EndElement)
                break;
            if (reader.NodeType is not XmlNodeType.Element)
                continue;
            switch (reader.Name)
            {
                case nameof(Source.RawEffects):
                    Source.RawEffects.ReadXmlCollection(reader, new RawEffectXmlSerialization(), nameof(Source.RawEffects));
                    break;
                case nameof(Source.Effects):
                    Source.Effects.ReadXmlCollection(reader, new EffectSentenceXmlSerialization(), nameof(Source.Effects));
                    break;
                case nameof(Source.Requires):
                    Source.Requires.ReadXmlCollection(reader, new RequireXmlSerialization(), nameof(Source.Requires));
                    break;
            }
        }
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString(nameof(Source.Signature), Source.Signature.ToString());
        writer.WriteAttributeString(nameof(Source.Name), Source.Name);
        writer.WriteAttributeString(nameof(Source.BeginWithStar), Source.BeginWithStar.ToString());
        writer.WriteAttributeString(nameof(Source.Duration), Source.Duration.ToString());
        writer.WriteAttributeString(nameof(Source.Description), Source.Description);
        writer.WriteAttributeString(nameof(Source.Ps), Source.Ps);
        writer.WriteAttributeString(nameof(Source.LatticedPoint),
            StringSimpleTypeConverter.ToArrayString(Source.LatticedPoint.Col, Source.LatticedPoint.Row));

        Source.RawEffects.WriteXmlCollection(writer, new RawEffectXmlSerialization(), nameof(Source.RawEffects));
        FormatRawEffects();
        Source.Effects.WriteXmlCollection(writer, new EffectSentenceXmlSerialization(), nameof(Source.Effects));
        Source.Requires.WriteXmlCollection(writer, new RequireXmlSerialization(), nameof(Source.Requires));
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
                Program.TestInfo.Append($"[{Source.Signature}] {raw}");
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