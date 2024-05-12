using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.StringUtilities;

namespace FocusTree.Model.Lattice;

public class GridDataSerialization : SsSerialization<GridData>
{
    public override string LocalName => nameof(GridData);

    protected override void Serialize()
    {
        WriteTag(nameof(Source.FloatComparisonTolerance), Source.FloatComparisonTolerance.ToString());
        WriteTag(nameof(Source.OriginX), Source.OriginX.ToString());
        WriteTag(nameof(Source.OriginY), Source.OriginY.ToString());
        WriteTag(nameof(Source.DrawRect), Source.DrawRect.ToArrayString());
    }

    protected override void Deserialize()
    {
        Source.FloatComparisonTolerance = ReadTag(nameof(Source.FloatComparisonTolerance), s => s.ToFloat(Source.FloatComparisonTolerance));
        Source.OriginX = ReadTag(nameof(Source.OriginX), s => s.ToInt(Source.OriginX));
        Source.OriginY = ReadTag(nameof(Source.OriginY), s => s.ToInt(Source.OriginY));
        Source.DrawRect = ReadTag(nameof(Source.DrawRect), s => s.ToRectangle(Source.DrawRect));
    }
}
