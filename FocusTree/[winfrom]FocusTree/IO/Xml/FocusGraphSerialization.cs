using FocusTree.Model.Focus;
using LocalUtilities.Serializations;
using LocalUtilities.SimpleScript.Serialization;

namespace FocusTree.IO.Xml;

public class FocusGraphSerialization : RosterSerialization<FocusGraph, int, FocusNode>
{
    public override string LocalName => "NationalFocus";

    protected override SsSerialization<FocusNode> ItemSerialization => new FocusNodeSerialization();

    protected override void SerializeRoster()
    {
        WriteTag(nameof(Source.Name), Source.Name);
    }

    protected override void DeserializeRoster()
    {
        var name = ReadTag(nameof(Source.Name), s => s ?? Source.Name);
        Source = new(name);
    }
}