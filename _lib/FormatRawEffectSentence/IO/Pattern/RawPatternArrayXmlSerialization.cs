using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SerializeUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot("RawPatterns")]
public class RawPatternArrayXmlSerialization : XmlSerialization<RawPattern[]>
{
    public RawPatternArrayXmlSerialization() : base("RawPatterns")
    {
    }

    public override void ReadXml(XmlReader reader)
    {
        var patterns = new List<RawPattern>();
        patterns.ReadXmlCollection(reader, LocalRootName, new RawPatternXmlSerialization());
        Source = patterns.Count is 0 ? null : patterns.ToArray();
    }

    public override void WriteXml(XmlWriter writer) =>
        Source?.WriteXmlCollection(writer, new RawPatternXmlSerialization());
}