using LocalUtilities.SerializeUtilities;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace FocusTree.IO.Xml;

[XmlRoot("Require")]
public class RequireXmlSerialization : Serialization<HashSet<int>>, IXmlSerialization<HashSet<int>>
{
    public RequireXmlSerialization() : base("Require")
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.Read();
        Source = reader.Value.ToArray().Select(x => x.ToInt() ?? 0)
            .Where(x => x is not 0).ToHashSet();
    }

    public void WriteXml(XmlWriter writer)
    {
        if (Source is null)
            return;
        writer.WriteValue(Source.ToArrayString());
    }
}