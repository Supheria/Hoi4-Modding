using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LocalUtilities.Interface;
using LocalUtilities.XmlUtilities;

namespace FocusTree.IO;

[XmlRoot("Require")]
public class RequireSerialization : IXmlSerialization<HashSet<int>>
{
    public HashSet<int> Source { get; set; }

    public string LocalName { get; } = "Require";

    public RequireSerialization(HashSet<int> source) => Source = source;

    public RequireSerialization() : this(new())
    {
    }

    public XmlSchema? GetSchema() => null;

    public void ReadXml(XmlReader reader)
    {
        reader.Read();
        Source = XmlReadTool.ReadArrayString(reader.Value).Select(x => XmlReadTool.GetIntValue(x) ?? 0)
            .Where(x => x is not 0).ToHashSet();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteValue(XmlWriteTool.WriteArrayString(Source.Select(x => x.ToString()).ToArray()));
    }
}