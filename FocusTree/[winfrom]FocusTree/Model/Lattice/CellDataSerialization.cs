using LocalUtilities.Serializations;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.StringUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace FocusTree.Model.Lattice;

public class CellDataSerialization : SsSerialization<CellData>
{
    public override string LocalName => nameof(CellData);

    public CellDataSerialization()
    {
        OnSerialize += Serialize;
        OnDeserialize += Deserialize;
    }

    private void Serialize()
    {
        WriteTag(nameof(Source.EdgeLengthMin), Source.EdgeLengthMin.ToString());
        WriteTag(nameof(Source.EdgeLengthMax), Source.EdgeLengthMax.ToString());
        WriteTag(nameof(Source.EdgeLength), Source.EdgeLength.ToString());
        WriteTag(nameof(Source.NodePaddingFactorMin), Source.NodePaddingFactorMin.ToString());
        WriteTag(nameof(Source.NodePaddingFactorMax), Source.NodePaddingFactorMax.ToString());
        WriteTag(nameof(Source.NodePaddingWidthFactor), Source.NodePaddingWidthFactor.ToString());
        WriteTag(nameof(Source.NodePaddingHeightFactor), Source.NodePaddingHeightFactor.ToString());
    }

    private void Deserialize()
    {
        Source.EdgeLengthMin = ReadTag(nameof(Source.EdgeLengthMin), s => s.ToInt(Source.EdgeLengthMin));
        Source.EdgeLengthMax = ReadTag(nameof(Source.EdgeLengthMax), s => s.ToInt(Source.EdgeLengthMax));
        Source.EdgeLength = ReadTag(nameof(Source.EdgeLength), s => s.ToInt(Source.EdgeLength));
        Source.NodePaddingFactorMin = ReadTag(nameof(Source.NodePaddingFactorMin), s => s.ToFloat(Source.NodePaddingFactorMin));
        Source.NodePaddingFactorMax = ReadTag(nameof(Source.NodePaddingFactorMax), s => s.ToFloat(Source.NodePaddingFactorMax));
        Source.NodePaddingWidthFactor = ReadTag(nameof(Source.NodePaddingWidthFactor), s => s.ToFloat(Source.NodePaddingWidthFactor));
        Source.NodePaddingHeightFactor = ReadTag(nameof(Source.NodePaddingHeightFactor), s => s.ToFloat(Source.NodePaddingHeightFactor));
    }
}
