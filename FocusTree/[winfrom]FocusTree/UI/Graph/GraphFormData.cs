using FocusTree.Model.WinFormGdiUtilities;
using LocalUtilities.UIUtilities;

namespace FocusTree.UI.Graph;

public class GraphFormData : FormData
{
    public override Size MinimumSize { get; set; } = new();

    public override Size Size { get; set; } = Background.Size;

    public GraphFormData()
    {

    }
}
