
#define test_format

using FocusTree.Model.Focus;
using FormatRawEffectSentence;
using FormatRawEffectSentence.IO;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.StringUtilities;

namespace FocusTree.IO.Xml;

public class FocusNodeSerialization : SsSerialization<FocusNode>
{
    public override string LocalName => nameof(FocusNode);

    RawEffectsSerialization RawEffectsSerialization { get; } = new();

    EffectSentenceSerialization EffectSentenceSerialization { get; } = new();

    RequiresSerialization RequiresSerialization { get; } = new();

    protected override void Serialize()
    {
        WriteTag(nameof(Source.Signature), Source.Signature.ToString());
        WriteTag(nameof(Source.Name), Source.Name);
        WriteTag(nameof(Source.BeginWithStar), Source.BeginWithStar.ToString());
        WriteTag(nameof(Source.Duration), Source.Duration.ToString());
        WriteTag(nameof(Source.Description), Source.Description);
        WriteTag(nameof(Source.Ps), Source.Ps);
        WriteTag(nameof(Source.LatticedPoint), Source.LatticedPoint.ToString());
        Serialize(Source.RawEffects, RawEffectsSerialization);
        FormatRawEffects();
        Serialize(Source.Effects, EffectSentenceSerialization);
        Serialize(Source.Requires, RequiresSerialization);
    }

    protected override void Deserialize()
    {
        Source.SetSignature = ReadTag(nameof(Source.Signature), s => s.ToInt(Source.Signature));
        Source.Name = ReadTag(nameof(Source.Name), s => s ?? Source.Name);
        Source.BeginWithStar = ReadTag(nameof(Source.BeginWithStar), s => s.ToBool(Source.BeginWithStar));
        Source.Duration = ReadTag(nameof(Source.Duration), s => s.ToInt(Source.Duration));
        Source.Description = ReadTag(nameof(Source.Description), s => s ?? Source.Description);
        Source.Ps = ReadTag(nameof(Source.Ps), s => s ?? Source.Ps);
        Source.LatticedPoint = ReadTag(nameof(Source.LatticedPoint), s => s.ToLatticedPoint(Source.LatticedPoint));
        Source.RawEffects = Deserialize(Source.RawEffects, RawEffectsSerialization);
        Deserialize(EffectSentenceSerialization, Source.Effects);
        Source.Requires = Deserialize(Source.Requires, RequiresSerialization);
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