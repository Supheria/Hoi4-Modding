using LocalUtilities.UIUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NameAnalyzer;

public class NameAnalyzerFormData : FormData
{
    public override Size MinimumSize { get; set; } = new(100, 100);
}
