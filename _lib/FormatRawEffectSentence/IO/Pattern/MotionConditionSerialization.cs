using FormatRawEffectSentence.LocalSign;
using LocalUtilities.Serializations;
using LocalUtilities.StringUtilities;

namespace FormatRawEffectSentence.IO.Pattern;

public class MotionConditionSerialization : KeyValuePairsSerialization<string, Motions>
{
    public override string LocalName => "Condition";

    protected override Func<string?, string> ReadKey => key => key ?? "";

    protected override Func<string?, Motions> ReadValue => value => value.ToEnum(Motions.None);

    protected override Func<string, string> WriteKey => key => key;

    protected override Func<Motions, string> WriteValue => value => value.ToString();
}