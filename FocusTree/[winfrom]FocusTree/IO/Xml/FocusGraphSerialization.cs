using FocusTree.Model.Focus;
using LocalUtilities.Serializations;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.SimpleScript.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace FocusTree.IO.Xml;

public class FocusGraphSerialization : RosterSerialization<FocusGraph, int, FocusNode>
{
    public override string LocalName => "NationalFocus";

    public FocusGraphSerialization() : base(new(), new FocusNodeSerialization())
    {
        OnSerialize += Serialize;
        OnDeserialize += Deserialize;
    }

    private void Serialize()
    {
        WriteTag(nameof(Source.Name), Source.Name);
    }

    private void Deserialize()
    {
        var name = ReadTag(nameof(Source.Name), s => s ?? Source.Name);
        Source = new(name);
    }
}