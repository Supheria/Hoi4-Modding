using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SerializeUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FormatRawEffectSentence.IO.Pattern;

public class RawPatternArrayXmlSerialization() : XmlSerialization<RawPattern[]>([])
{
    public override string LocalName => "RawPatterns";

    public override void ReadXml(XmlReader reader)
    {
        var patterns = new List<RawPattern>();
        patterns.ReadXmlCollection(reader, LocalName, new RawPatternXmlSerialization());
        Source = patterns.Count is 0 ? [] : patterns.ToArray();
    }

    public override void WriteXml(XmlWriter writer)
    {
        Source.WriteXmlCollection(writer, new RawPatternXmlSerialization());
    }
}