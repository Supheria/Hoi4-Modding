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

public class GridDataSerialization : SsSerialization<GridData>
{
    public override string LocalName => nameof(GridData);

    public GridDataSerialization()
    {
        OnSerialize += Serialize;
        OnDeserialize += Deserialize;
    }

    private void Serialize()
    {
        WriteTag(nameof(Source.FloatComparisonTolerance), Source.FloatComparisonTolerance.ToString());
        WriteTag(nameof(Source.OriginX), Source.OriginX.ToString());
        WriteTag(nameof(Source.OriginY), Source.OriginY.ToString());
        WriteTag(nameof(Source.DrawRect), Source.DrawRect.ToArrayString());
    }

    private void Deserialize()
    {
        Source.FloatComparisonTolerance = ReadTag(nameof(Source.FloatComparisonTolerance), s => s.ToFloat(Source.FloatComparisonTolerance));
        Source.OriginX = ReadTag(nameof(Source.OriginX), s => s.ToInt(Source.OriginX));
        Source.OriginY = ReadTag(nameof(Source.OriginY), s => s.ToInt(Source.OriginY));
        Source.DrawRect = ReadTag(nameof(Source.DrawRect), s => s.ToRectangle(Source.DrawRect));
    }
}
