using System.Text;
using LocalUtilities.StringUtilities;

namespace Parser.Data.TokenTypes;

public class TagArray : Token
{
    public List<List<KeyValuePair<Word, List<Word>>>> Value { get; }

    public TagArray(Token? from, Word name, uint level)
        : base(from, name, level)
    {
        Value = new();
    }

    public void Append(Word value)
    {
        Value.LastOrDefault()?.LastOrDefault().Value.Add(value);
    }

    public void AppendTag(Word value)
    {
        Value.LastOrDefault()?.Add(new(value, new()));
    }

    public void AppendNew(Word value)
    {
        Value.Add(new() { new(value, new()) });
    }

    public override string ValueToString()
    {
        return new StringBuilder()
            .AppendJoin('\0', Value, (sb, value) => StringBuilderTool.AppendJoin(sb
                    .Append('('), ' ', value, (sb2, pair) => sb2
                    .Append(pair.Key)
                    .Append('[')
                    .AppendJoin(' ', pair.Value)
                    .Append(']'))
                .Append(')')
                .Append('\n'))
            .ToString();
    }
}