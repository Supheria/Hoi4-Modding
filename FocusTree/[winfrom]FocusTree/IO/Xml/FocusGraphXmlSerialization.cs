using FocusTree.Model.Focus;
using LocalUtilities.SerializeUtilities;
using System.Xml;
using System.Xml.Serialization;

namespace FocusTree.IO.Xml;

[XmlRoot("NationalFocuses")]
public class FocusGraphXmlSerialization : XmlSerialization<FocusGraph>
{
    public FocusGraphXmlSerialization() : base("NationalFocuses")
    {
    }

    public override void ReadXml(XmlReader reader)
    {
        var name = reader.GetAttribute(nameof(Source.Name)) ?? "";
        var focusNodes = new List<FocusNode>();
        focusNodes.ReadXmlCollection(reader, LocalRootName, new FocusNodeXmlSerialization());
        Source = new(name, focusNodes.ToArray());
    }

    public override void WriteXml(XmlWriter writer)
    {
        if (Source is null)
            return;
        writer.WriteAttributeString(nameof(Source.Name), Source.Name);
        Source.FocusNodes.WriteXmlCollection(writer, new FocusNodeXmlSerialization());
    }
}