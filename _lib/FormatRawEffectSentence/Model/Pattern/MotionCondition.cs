using FormatRawEffectSentence.LocalSign;
using LocalUtilities.TypeBundle;

namespace FormatRawEffectSentence.Model.Pattern;

public class MotionCondition : SerializableDictionary<string, Motions>
{
    public override string LocalName { get; set; } = "Condition";

    protected override Func<string?, string> ReadKey => key => key ?? "";

    protected override Func<string?, Motions> ReadValue => value => value.ToEnum(Motions.None);

    protected override Func<string, string> WriteKey => key => key;

    protected override Func<Motions, string> WriteValue => value => value.ToString();
}
