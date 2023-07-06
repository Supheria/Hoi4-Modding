using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.Interface;
using LocalUtilities.XmlUtilities;

namespace FormatRawEffectSentence.IO.Pattern;

[XmlRoot("RawPatterns")]
public class RawPatternArraySerialization : IXmlSerialization<RawPattern[]>
{
    public RawPattern[] Source { get; set; }

    public string LocalName { get; } = "RawPatterns";

    public RawPatternArraySerialization(RawPattern[] source) => Source = source;

    public RawPatternArraySerialization() : this(Array.Empty<RawPattern>())
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader) =>
        Source.ReadXmlCollection(reader, LocalName, new RawPatternSerialization());

    public void WriteXml(XmlWriter writer) =>
        Source.WriteXmlCollection(writer, new RawPatternSerialization());
}