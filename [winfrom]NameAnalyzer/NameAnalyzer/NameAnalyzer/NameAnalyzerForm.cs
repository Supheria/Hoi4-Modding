using LocalUtilities.UIUtilities;

namespace NameAnalyzer;

public class NameAnalyzerForm : ResizeableForm<NameAnalyzerFormData>
{
    public NameAnalyzerForm() : base(new NameAnalyzerFormData(), new NameAnalyzerFormDataXmlSerialization() { IniFileName = "shit" })
    {
        OnDrawingClient += DrawClient;
    }

    private void DrawClient()
    {
        var labelFont = new Font("ºÚÌå", Height * 0.03f, FontStyle.Regular, GraphicsUnit.Pixel);
        var contentFont = new Font("ºÚÌå", Height * 0.05f, FontStyle.Regular, GraphicsUnit.Pixel);
        var widthHalf = ClientRectangle.Width / 2;
        //
        // SelectedLevelLabel
        //
        SelectedLevelLabel.Left = Left + Padding;
        SelectedLevelLabel.Top = Top + Padding;
        SelectedLevelLabel.Font = labelFont;
        //
        // SelectedLevel
        //
        SelectedLevel.Left = SelectedLevelLabel.Left;
        SelectedLevel.Top = SelectedLevelLabel.Bottom + Padding;
        SelectedLevel.Width = widthHalf / 2;
        SelectedLevel.Font = contentFont;
        //
        // OnlyShowWarning
        //
        OnlyShowWarning.Left = SelectedLevel.Right + Padding;
        OnlyShowWarning.Top = SelectedLevel.Top;
        OnlyShowWarning.Height = OnlyShowWarning.Width = SelectedLevel.Height;
    }

    protected override void InitializeComponent()
    {
        //
        // this
        //
        Controls.AddRange([
            SelectedLevelLabel,
            SelectedLevel,
            OnlyShowWarning,
            OpenWarningLog,
            ]);
        //
        // SelectedLevelLabel
        //
        SelectedLevelLabel.Text = "²ã¼¶";
        SelectedLevelLabel.AutoSize = true;
        //
        // SelectedLevel
        //

        //
        // OnlyShowWarning
        //
        OnlyShowWarning.Tag = false;
    }

    Label SelectedLevelLabel { get; } = new();
    NumericUpDown SelectedLevel { get; } = new();
    Button OnlyShowWarning { get; } = new();
    Button OpenWarningLog { get; } = new();
}
