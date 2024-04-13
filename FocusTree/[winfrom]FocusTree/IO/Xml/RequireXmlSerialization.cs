using LocalUtilities.SerializeUtilities;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Serialization;

namespace FocusTree.IO.Xml;

public class RequireXmlSerialization() : XmlSerialization<HashSet<int>>([])
{
    public override string LocalName => "Require";

    public override void ReadXml(XmlReader reader)
    {
        reader.Read();
        Source = reader.Value.ToArray().Select(x => x.ToInt() ?? 0)
            .Where(x => x is not 0).ToHashSet();
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteValue(Source.ToArrayString());
    }
}