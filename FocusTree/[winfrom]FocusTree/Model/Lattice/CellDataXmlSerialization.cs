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

public class CellDataXmlSerialization() : XmlSerialization<CellData>(new())
{
    public override string LocalName => nameof(CellData);

    public override void ReadXml(XmlReader reader)
    {
        Source.EdgeLengthMin = reader.GetAttribute(nameof(Source.EdgeLengthMin)).ToInt() ?? Source.EdgeLengthMin;
        Source.EdgeLengthMax = reader.GetAttribute(nameof(Source.EdgeLengthMax)).ToInt() ?? Source.EdgeLengthMax;
        Source.EdgeLength = reader.GetAttribute(nameof(Source.EdgeLength)).ToInt() ?? Source.EdgeLength;
        Source.NodePaddingFactorMin = reader.GetAttribute(nameof(Source.NodePaddingFactorMin)).ToFloat() ?? Source.NodePaddingFactorMin;
        Source.NodePaddingFactorMax = reader.GetAttribute(nameof(Source.NodePaddingFactorMax)).ToFloat() ?? Source.NodePaddingFactorMax;
        Source.NodePaddingWidthFactor = reader.GetAttribute(nameof(Source.NodePaddingWidthFactor)).ToFloat() ?? Source.NodePaddingWidthFactor;
        Source.NodePaddingHeightFactor = reader.GetAttribute(nameof(Source.NodePaddingHeightFactor)).ToFloat() ?? Source.NodePaddingHeightFactor;
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString(nameof(Source.EdgeLengthMin), Source.EdgeLengthMin.ToString());
        writer.WriteAttributeString(nameof(Source.EdgeLengthMax), Source.EdgeLengthMax.ToString());
        writer.WriteAttributeString(nameof(Source.EdgeLength), Source.EdgeLength.ToString());
        writer.WriteAttributeString(nameof(Source.NodePaddingFactorMin), Source.NodePaddingFactorMin.ToString());
        writer.WriteAttributeString(nameof(Source.NodePaddingFactorMax), Source.NodePaddingFactorMax.ToString());
        writer.WriteAttributeString(nameof(Source.NodePaddingWidthFactor), Source.NodePaddingWidthFactor.ToString());
        writer.WriteAttributeString(nameof(Source.NodePaddingHeightFactor), Source.NodePaddingHeightFactor.ToString());
    }
}
