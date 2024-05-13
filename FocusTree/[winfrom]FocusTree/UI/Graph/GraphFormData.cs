using FocusTree.Model.WinFormGdiUtilities;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.UIUtilities;

namespace FocusTree.UI.Graph;

public class GraphFormData(string localName) : FormData(localName)
{
    public override Size MinimumSize { get; set; } = new();

    public override Size Size { get; set; } = Background.Size;

    protected override void SerializeFormData(SsSerializer serializer)
    {

    }

    protected override void DeserializeFormData(SsDeserializer deserializer)
    {

    }
}
