using LocalUtilities.Serializations;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.StringUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FocusTree.Model.Lattice;

public class GridDataXmlSerialization() : XmlSerialization<GridData>(new())
{
    public override string LocalName => nameof(GridData);

    public override void ReadXml(XmlReader reader)
    {
        Source.FloatComparisonTolerance = reader.GetAttribute(nameof(Source.FloatComparisonTolerance)).ToFloat() ?? Source.FloatComparisonTolerance;
        Source.OriginX = reader.GetAttribute(nameof(Source.OriginX)).ToInt() ?? Source.OriginX;
        Source.OriginY = reader.GetAttribute(nameof(Source.OriginY)).ToInt() ?? Source.OriginX;
        while (reader.Read())
        {
            if (reader.Name == LocalName && reader.NodeType is XmlNodeType.EndElement)
                break;
            if (reader.NodeType is not XmlNodeType.Element)
                continue;
            if (reader.Name == nameof(Source.DrawRect))
                Source.DrawRect = new RectangleXmlSerialization(nameof(Source.DrawRect)).Deserialize(reader);
        }
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString(nameof(Source.FloatComparisonTolerance), Source.FloatComparisonTolerance.ToString());
        writer.WriteAttributeString(nameof(Source.OriginX), Source.OriginX.ToString());
        writer.WriteAttributeString(nameof(Source.OriginY), Source.OriginY.ToString());
        new RectangleXmlSerialization(nameof(Source.DrawRect)) { Source = Source.DrawRect }.Serialize(writer);
    }
}
