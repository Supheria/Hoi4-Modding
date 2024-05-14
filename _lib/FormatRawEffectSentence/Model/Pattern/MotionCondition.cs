using FormatRawEffectSentence.LocalSign;
using LocalUtilities.TypeBundle;

namespace FormatRawEffectSentence.Model.Pattern;

public class MotionCondition : SerializableTagValues<string, Motions>
{
    public override string LocalName { get; set; } = "Condition";

    public override string KeyName { get; set; } = "Index";

    protected override Func<string?, string> ReadKey => key => key ?? "";

    protected override Func<string?, Motions> ReadValue => value => value.ToEnum(Motions.None);

    protected override Func<string, string> WriteKey => key => key;

    protected override Func<Motions, string> WriteValue => value => value.ToString();
}
