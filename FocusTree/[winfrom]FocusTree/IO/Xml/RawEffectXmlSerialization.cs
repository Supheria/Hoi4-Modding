using LocalUtilities.SerializeUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FocusTree.IO.Xml;

[XmlRoot("RawEffect")]
public class RawEffectXmlSerialization : XmlSerialization<string>
{
    public RawEffectXmlSerialization() : base("RawEffect")
    {
    }

    public override void ReadXml(XmlReader reader)
    {
        reader.Read();
        Source = reader.Value;
    }

    public override void WriteXml(XmlWriter writer) => writer.WriteValue(Source);
}