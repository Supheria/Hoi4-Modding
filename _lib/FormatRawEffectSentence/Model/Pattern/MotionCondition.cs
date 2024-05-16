using FormatRawEffectSentence.LocalSign;
using LocalUtilities.TypeBundle;

namespace FormatRawEffectSentence.Model.Pattern;

public class MotionCondition : SerializableTagValues<string, Motions>
{
    public override string LocalName { get; set; } = "Condition";

    protected override Func<string, string> WriteTag => tag => tag;

    protected override Func<Motions, List<string>> WriteValue => value => [value.ToString()];

    protected override Func<string, string> ReadTag => tag => tag ?? "";

    protected override Func<List<string>, Motions> ReadValue => values => values[0].ToEnum(Motions.None);
}
