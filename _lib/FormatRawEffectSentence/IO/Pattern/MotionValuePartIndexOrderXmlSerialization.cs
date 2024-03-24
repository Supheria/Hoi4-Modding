using LocalUtilities.SerializeUtilities;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot("PartIndex")]
public class MotionValuePartIndexOrderXmlSerialization : XmlSerialization<int>
{
    public MotionValuePartIndexOrderXmlSerialization() : base("PartIndex")
    {
    }

    public override void ReadXml(XmlReader reader)
    {
        reader.Read();
        var index = reader.Value.ToInt();
        if (index is not null)
            Source = (int)index;
    }

    public override void WriteXml(XmlWriter writer) => writer.WriteValue(Source);
}