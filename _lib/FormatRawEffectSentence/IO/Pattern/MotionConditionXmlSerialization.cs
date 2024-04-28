using FormatRawEffectSentence.LocalSign;
using LocalUtilities.Serializations;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

public class MotionConditionXmlSerialization : KeyValuePairXmlSerialization<string, Motions>
{
    public override string LocalName => "Condition";

    protected override string KeyName => "Pattern";

    protected override string ValueName => "Motion";

    protected override Func<string?, string> ReadKey => key => key ?? "";

    protected override Func<string?, Motions> ReadValue => value => value.ToEnum<Motions>();

    protected override Func<string, string> WriteKey => key => key;

    protected override Func<Motions, string> WriteValue => value => value.ToString();
}