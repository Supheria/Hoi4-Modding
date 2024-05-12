using LocalUtilities.StringUtilities;
using System.Text;

namespace FormatRawEffectSentence.IO;

public class EffectSentenceStringSerialization() : EffectSentenceSerialization()
{
    protected static string LocalNameType => "Type";

    protected static string LocalNameValue => "Value";

    public override string ToString()
    {
        if (Source is null)
            return "";
        var sb = new StringBuilder().AppendLine(SentenceToString());
        foreach (var sub in Source.SubSentences)
            sb.AppendLine(new EffectSentenceStringSerialization { Source = sub }.ToString(1));
        return sb.ToString();
    }

    private string ToString(int tabTime)
    {
        if (Source is null)
            return "";
        var sb = new StringBuilder();
        for (var i = 0; i < tabTime; i++)
            sb.Append('\t');
        sb.Append(SentenceToString());
        foreach (var sub in Source.SubSentences)
            sb.Append($"\n{new EffectSentenceStringSerialization { Source = sub }.ToString(tabTime + 1)}");
        return sb.ToString();
    }

    private string SentenceToString() => Source is null
        ? ""
        : $"{nameof(Source.Motion)}=\"{Source.Motion}\", {LocalNameType}=\"{TypePairToString()}\", {LocalNameValue}=\"{ValuePairToString()}\"";

    private string TypePairToString() => Source is null ? "" : $"({Source.ValueType}),({Source.TriggerType})";

    private string ValuePairToString() =>
        Source is null ? "" : $"({Source.Value}),({Source.Triggers.ToArrayString()})";
}