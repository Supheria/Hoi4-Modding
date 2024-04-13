using FormatRawEffectSentence.LocalSign;
using LocalUtilities.Serializations;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

public class MotionConditionXmlSerialization()
    : KeyValuePairXmlSerialization<string, Motions>(
        "Condition", "Pattern", "Motion",
        key => key ?? "", value => value.ToEnum<Motions>(),
        key => key, value => value.ToString()
        )
{

}