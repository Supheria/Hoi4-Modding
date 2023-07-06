using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using FocusTree.Data.Focus;
using LocalUtilities.Interface;
using LocalUtilities.XmlUtilities;

namespace FocusTree.IO;

[XmlRoot("NationalFocus")]
public class FocusGraphSerialization : IXmlSerialization<FocusGraph>
{
    public FocusGraph Source { get; set; }

    public string LocalName { get; } = "NationalFocus";

    public XmlSchema? GetSchema() => null;

    public FocusGraphSerialization(FocusGraph? source) => Source = source ?? new();

    public FocusGraphSerialization() : this(new())
    {
    }

    public void ReadXml(XmlReader reader)
    {
        var name = reader.GetAttribute(nameof(Source.Name)) ?? "";
        var focusNodes = new List<FocusNode>();
        focusNodes.ReadXmlCollection(reader, LocalName, new FocusNodeSerialization());
        Source = new(name, focusNodes.ToArray());
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString(nameof(Source.Name), Source.Name);
        Source.FocusNodes.WriteXmlCollection(writer, new FocusNodeSerialization());
    }
}