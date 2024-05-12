using LocalUtilities.SerializeUtilities;
using System.Xml;

namespace FocusTree.UI.NodeToolDialogs;

public class NodeInfoDialogSerialization() : XmlSerialization<NodeInfoDialog>(new())
{
    public override string LocalName => nameof(NodeInfoDialog);

    public override void ReadXml(XmlReader reader)
    {
        Source.FocusNameText = reader.GetAttribute(nameof(Source.FocusNameText)) ?? Source.FocusNameText;
        Source.DurationText = reader.GetAttribute(nameof(Source.DurationText)) ?? Source.DurationText;
        Source.DescriptText = reader.GetAttribute(nameof(Source.DescriptText)) ?? Source.DescriptText;
        Source.EffectsText = reader.GetAttribute(nameof(Source.EffectsText)) ?? Source.EffectsText;
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString(nameof(Source.FocusNameText), Source.FocusNameText);
        writer.WriteAttributeString(nameof(Source.DurationText), Source.DurationText);
        writer.WriteAttributeString(nameof(Source.DescriptText), Source.DescriptText);
        writer.WriteAttributeString(nameof(Source.EffectsText), Source.EffectsText);
    }
}
