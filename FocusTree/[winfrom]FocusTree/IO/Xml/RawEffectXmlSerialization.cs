using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LocalUtilities.SerializeUtilities.Interface;

namespace FocusTree.IO.Xml;

[XmlRoot("RawEffect")]
public class RawEffectXmlSerialization : Serialization<string>, IXmlSerialization<string>
{
    public RawEffectXmlSerialization() : base("RawEffect")
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.Read();
        Source = reader.Value;
    }

    public void WriteXml(XmlWriter writer) => writer.WriteValue(Source);
}