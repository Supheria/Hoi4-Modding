using FormatRawEffectSentence.LocalSign;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot("Condition")]
public class MotionConditionXmlSerialization : KeyValuePairXmlSerialization<string, Motions>
{
    public MotionConditionXmlSerialization() : 
        base("Condition", "Pattern", "Motion",
        key => key ?? "", value => value.ToEnum<Motions>(),
        key => key, value => value.ToString()
        )
    {
    }
}