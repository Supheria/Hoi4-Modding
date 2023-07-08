using LocalUtilities.SerializeUtilities;
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
        Source = SimpleTypeTool.ReadArrayString(reader.Value).Select(x => SimpleTypeTool.GetIntValue(x) ?? 0)
            .Where(x => x is not 0).ToHashSet();
    }

    public void WriteXml(XmlWriter writer)
    {
        if (Source is null)
            return;
        writer.WriteValue(SimpleTypeTool.WriteArrayString(Source.Select(x => x.ToString()).ToArray()));
    }
}