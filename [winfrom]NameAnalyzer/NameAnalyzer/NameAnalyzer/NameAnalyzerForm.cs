using LocalUtilities.UIUtilities;

namespace NameAnalyzer;

public class NameAnalyzerForm() : 
    ResizeableForm<NameAnalyzerFormData>(new NameAnalyzerFormData(), new NameAnalyzerFormDataXmlSerialization() { IniFileName = "shit"})
{
    protected override void DrawClient()
    {
    }

    protected override void InitializeComponent()
    {
    }
}
