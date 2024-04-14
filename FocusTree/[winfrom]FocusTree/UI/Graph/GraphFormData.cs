using LocalUtilities.UIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FocusTree.Model.WinFormGdiUtilities;

namespace FocusTree.UI.Graph;

public class GraphFormData : FormData
{
    public override Size MinimumSize { get;set; } = new();

    public override Size Size { get;set; } = Background.Size;

    public GraphFormData()
    {
        
    }
}
