using FocusTree.Model.Focus;
using LocalUtilities.SerializeUtilities;
using System.Xml;
using System.Xml.Serialization;

namespace FocusTree.IO.Xml;

public class FocusGraphXmlSerialization() : XmlSerialization<FocusGraph>(new())
{
    public override string LocalName => "NationalFocuses";

    public override void ReadXml(XmlReader reader)
    {
        //Name
        var name = reader.GetAttribute(nameof(Source.Name)) ?? "";
        var focusNodes = new List<FocusNode>();
        focusNodes.ReadXmlCollection(reader, LocalName, new FocusNodeXmlSerialization());
        Source = new(name) {
            RosterList = focusNodes.ToArray()
        };
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString(nameof(Source.Name), Source.Name);
        Source.RosterList.WriteXmlCollection(writer, new FocusNodeXmlSerialization());
    }
}