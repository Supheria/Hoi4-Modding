using LocalUtilities.SerializeUtilities;
using System.Xml;
using System.Xml.Serialization;

namespace FocusTree.IO.Xml;

public class RawEffectXmlSerialization() : XmlSerialization<string>("")
{
    public override string LocalName => "RawEffect";

    public override void ReadXml(XmlReader reader)
    {
        reader.Read();
        Source = reader.Value;
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteValue(Source);
    }
}