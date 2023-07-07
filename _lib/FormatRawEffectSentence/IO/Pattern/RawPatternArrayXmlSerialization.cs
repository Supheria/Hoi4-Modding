using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.Interface;
using LocalUtilities.XmlUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot("RawPatterns")]
public class RawPatternArrayXmlSerialization : Serialization<RawPattern[]>, IXmlSerialization<RawPattern[]>
{
    public RawPatternArrayXmlSerialization() : base("RawPatterns")
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        var patterns = new List<RawPattern>();
        patterns.ReadXmlCollection(reader, LocalRootName, new RawPatternXmlSerialization());
        Source = patterns.Count is 0 ? null : patterns.ToArray();
    }

    public void WriteXml(XmlWriter writer) =>
        Source?.WriteXmlCollection(writer, new RawPatternXmlSerialization());
}