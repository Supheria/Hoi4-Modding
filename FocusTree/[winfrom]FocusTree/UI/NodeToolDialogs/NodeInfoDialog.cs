using FocusTree.UI.Graph;
using LocalUtilities.FileUtilities;
using LocalUtilities.Interface;
using LocalUtilities.StringUtilities;
using System.Drawing.Text;

namespace FocusTree.UI.NodeToolDialogs
{
    public partial class NodeInfoDialog : ToolDialog, IHistoryRecordable
    {
        private bool _doFontScale = false;
        private bool _doTextEditCheck = false;

        #region ==== 初始化和更新 ====

        public NodeInfoDialog() { }

        internal NodeInfoDialog(GraphDisplay display)
        {
            Display = display;
            InitializeComponent();

            FormClosing += NodeInfoDialog_FormClosing;
            VisibleChanged += InfoDialog_VisibleChanged;
            Resize += InfoDialog_Resize;

            var font = new Font("仿宋", 20, FontStyle.Regular, GraphicsUnit.Pixel);
            var richBoxes = textBoxList.SkipWhile(x => x.Name is "FocusName" or "Duration").ToList();
            richBoxes.ForEach(x => x.Font = font);
            richBoxes.ForEach(x => x.KeyDown += RichTextBox_KeyDown);
            richBoxes.ForEach(x => x.MouseWheel += RichTextBox_MouseWheel);
            textBoxList.ForEach(x => x.KeyUp += TextBox_KeyUp);
            textBoxList.ForEach(x => x.TextChanged += Text_TextChanged);
            textBoxList.ForEach(x => x.LostFocus += Text_LostFocus);
            ButtonEvent.Click += ButtonEvent_Click;

            DrawClient();
        }

        private void NodeInfoDialog_FormClosing(object? sender, FormClosingEventArgs e)
        {
            this.ClearCache();
        }

        private void InfoDialog_VisibleChanged(object sender, EventArgs e)
        {
            if (Display.SelectedNode is null)
                return;
            var focus = Display.SelectedNode;
            Text = $"id: {focus.Id}";
            FocusName.Text = focus.Name;
            Duration.Text = $"{focus.Duration}";
            Descript.Text = focus.Description;
            Effects.Text = string.Join('\n', focus.RawEffects);

            AllowDrop = !GraphBox.ReadOnly;
            FocusName.ReadOnly = GraphBox.ReadOnly;
            Duration.ReadOnly = GraphBox.ReadOnly;
            ButtonEvent.Text = GraphBox.ReadOnly ? "开始" : "保存";
            Requires.ReadOnly = GraphBox.ReadOnly;
            Descript.ReadOnly = GraphBox.ReadOnly;
            Effects.ReadOnly = GraphBox.ReadOnly;

            this.NewHistory();
        }

        #endregion

        #region ==== 控件事件 ====

        private void Text_LostFocus(object sender, EventArgs e)
        {
            if (_doTextEditCheck == false)
            {
                return;
            }
            this.EnqueueHistory();
            if (this.IsEdit())
            {
                ButtonEvent.BackColor = Color.Yellow;
            }
            else
            {
                ButtonEvent.BackColor = BackColor;
            }
        }

        private void Text_TextChanged(object sender, EventArgs e)
        {
            _doTextEditCheck = true;
        }

        private void ButtonEvent_Click(object sender, EventArgs e)
        {
            if (GraphBox.ReadOnly)
            {
                EventReadOnly();
            }
            else
            {
                EventEdit();
            }
        }
        /// <summary>
        /// 作为展示对话框
        /// </summary>
        private void EventReadOnly()
        {

        }
        /// <summary>
        /// 作为可编辑对话框
        /// </summary>
        private void EventEdit()
        {

        }

        #endregion

        #region ==== 绘图和事件 ====

        private void InfoDialog_Resize(object sender, EventArgs e)
        {
            ScaleFontSize(Descript.ZoomFactor * ControlResize.GetRatio(this).Y);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            _doFontScale = false;
        }
        private void RichTextBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!_doFontScale)
                return;
            var richBox = sender as RichTextBox;
            ScaleFontSize(richBox.ZoomFactor + e.Delta * 0.00001f);
        }
        private void ScaleFontSize(float zoomFactor)
        {
            var richBoxes = textBoxList.SkipWhile(x => x.Name is "FocusName" or "Duration").ToList().Cast<RichTextBox>().ToList();
            var textBox = richBoxes.FirstOrDefault();
            zoomFactor = zoomFactor < Height * 0.0015f ? Height * 0.0015f :
                zoomFactor > Height * 0.002f ? Height * 0.002f : zoomFactor;
            richBoxes.ForEach(x => x.ZoomFactor = zoomFactor);
        }
        private void RichTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                _doFontScale = true;
            }
        }
        protected override void DrawClient()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                return;
            }
            SuspendLayout();
            const int padding = 12;
            var fontSize = Height * 0.03f;
            //
            // FocusName
            //
            FocusName.Left = ClientRectangle.Left + padding;
            FocusName.Top = ClientRectangle.Top + padding;
            FocusName.Width = (int)((ClientRectangle.Width - padding * 2.5) * 0.7 - padding);
            FocusName.Font = new Font(Font.FontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            //
            // FocusIcon
            //
            FocusIcon.Left = ClientRectangle.Left + padding;
            FocusIcon.Top = FocusName.Bottom + padding;
            FocusIcon.Width = (int)(MathF.Min(ClientRectangle.Width * 0.382f, ClientRectangle.Height * 0.3f));
            FocusIcon.Height = (int)(MathF.Min(ClientRectangle.Width * 0.382f, ClientRectangle.Height * 0.3f));
            //
            // Duration
            //
            Duration.Left = FocusIcon.Right + padding;
            Duration.Top = FocusName.Bottom + padding;
            Duration.Width = (int)((FocusName.Width * 1.05f - FocusIcon.Right) * 0.6f);
            //Duration.Height = textFont.Height;
            Duration.Font = new Font(Font.FontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            //
            // DurationUnit
            //
            DurationUnit.Left = (int)(Duration.Right);
            DurationUnit.Top = (int)(FocusName.Bottom + padding * 1.22f);
            DurationUnit.Width = (int)(FocusName.Width - FocusIcon.Width - Duration.Width);
            DurationUnit.Height = Duration.Height;
            //
            // ButtonEvent
            //
            ButtonEvent.Left = (int)(FocusName.Right + padding * 2.5f);
            ButtonEvent.Top = ClientRectangle.Top + padding;
            ButtonEvent.Width = (int)(ClientRectangle.Right - FocusName.Right - padding * 4.5f);
            ButtonEvent.Height = (int)(Duration.Bottom - ClientRectangle.Top - padding);
            ButtonEvent.Font = new Font("黑体", fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            //
            // Requires
            //
            Requires.Left = FocusIcon.Right + padding;
            Requires.Top = (int)(Duration.Bottom + padding * 1.5f);
            Requires.Width = (int)(ClientRectangle.Right - FocusIcon.Right - padding * 2.5f);
            Requires.Height = FocusIcon.Bottom - Duration.Bottom - padding * 2;

            //
            // Description
            //
            Descript.Left = ClientRectangle.Left + padding;
            Descript.Top = FocusIcon.Bottom + padding;
            Descript.Width = (int)(ClientRectangle.Width - padding * 2.5f);
            Descript.Height = (int)(ClientRectangle.Height * 0.15f);
            //
            //EffectsTitle
            //
            EffectsTitle.Left = ClientRectangle.Left + padding;
            EffectsTitle.Top = (int)(Descript.Bottom + padding * 1.5f);
            EffectsTitle.Width = (int)(ClientRectangle.Width - padding * 2.5);
            EffectsTitle.Height = (int)(ClientRectangle.Height * 0.05f);
            //
            // Effects
            //
            Effects.Left = ClientRectangle.Left + padding;
            Effects.Top = (int)(EffectsTitle.Bottom + padding * 0.5f);
            Effects.Width = (int)(ClientRectangle.Width - padding * 2.5f);
            Effects.Height = (int)(ClientRectangle.Bottom - EffectsTitle.Bottom - padding * 2.5f);
            //
            // draw picture box image
            //
            FocusIcon.Image?.Dispose();
            DurationUnit.Image?.Dispose();
            EffectsTitle.Image?.Dispose();
            //FocusIcon.Image = Image.FromFile("C:\\Non_E\\documents\\GitHub\\FocusTree\\FocusTree\\FocusTree\\Resources\\FocusTree.ico");
            DurationUnit.Image = new Bitmap(DurationUnit.Width, DurationUnit.Height);
            EffectsTitle.Image = new Bitmap(EffectsTitle.Width, EffectsTitle.Height);
            var g1 = Graphics.FromImage(DurationUnit.Image);
            var g2 = Graphics.FromImage(EffectsTitle.Image);
            g1.Clear(BackColor);
            g2.Clear(BackColor);
            var brush = new SolidBrush(Color.Black);
            var stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Near,
                FormatFlags = (StringFormatFlags)0,
                HotkeyPrefix = HotkeyPrefix.None,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.None
            };
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Center;
            g1.DrawString(
                "日",
                new Font("仿宋", fontSize, FontStyle.Bold, GraphicsUnit.Pixel),
                brush,
                new RectangleF(0, 0, DurationUnit.Width, DurationUnit.Height),
                stringFormat
                );
            stringFormat.Alignment = StringAlignment.Center;
            g2.DrawString(
                "==== 效果 ====",
                new Font("仿宋", fontSize, FontStyle.Regular, GraphicsUnit.Pixel),
                brush,
                new RectangleF(0, 0, EffectsTitle.Width, EffectsTitle.Height),
                stringFormat
                );
            g1.Flush(); g1.Dispose();
            g2.Flush(); g2.Dispose();
            ResumeLayout();
        }

        #endregion

        #region ==== InitializeComponent ====

        private void InitializeComponent()
        {
            var backColor = Color.White;
            var foreColor = Color.DarkBlue;
            textBoxList = new()
            {
                FocusName,
                Duration,
                Requires,
                Descript,
                Effects,
            };
            //
            // FocusName
            //
            FocusName.Name = "FocusName";
            FocusName.TextAlign = HorizontalAlignment.Center;
            FocusName.BorderStyle = BorderStyle.FixedSingle;
            FocusName.BackColor = backColor;
            FocusName.ForeColor = foreColor;
            //
            // FocusIcon
            //

            //
            // Duration
            //
            Duration.Name = "Duration";
            Duration.TextAlign = HorizontalAlignment.Center;
            Duration.BorderStyle = BorderStyle.FixedSingle;
            Duration.BackColor = backColor;
            Duration.ForeColor = foreColor;
            //
            // DurationUnit
            //

            //
            // ButtonEvent
            //
            ButtonEvent.TextAlign = ContentAlignment.MiddleCenter;
            ButtonEvent.FlatStyle = FlatStyle.Flat;
            //
            // Requires
            //
            Requires.Name = "Requires";
            Requires.Multiline = true;
            Requires.WordWrap = false;
            Requires.ScrollBars = RichTextBoxScrollBars.Both;
            Requires.BorderStyle = BorderStyle.FixedSingle;
            Requires.BackColor = backColor;
            Requires.ForeColor = foreColor;
            //
            // Description
            //
            Descript.Name = "Descript";
            Descript.Multiline = true;
            Descript.ScrollBars = RichTextBoxScrollBars.Vertical;
            Descript.BorderStyle = BorderStyle.FixedSingle;
            Descript.BackColor = backColor;
            Descript.ForeColor = foreColor;
            //
            //EffectsTitle
            //

            //
            // Effects
            //
            Effects.Name = "Effects";
            Effects.Multiline = true;
            Effects.WordWrap = false;
            Effects.ScrollBars = RichTextBoxScrollBars.Both;
            Effects.BorderStyle = BorderStyle.FixedSingle;
            Effects.BackColor = backColor;
            Effects.ForeColor = foreColor;
            //
            // main
            //
            Controls.AddRange(new Control[]
            {
                FocusName,
                FocusIcon,
                Duration,
                DurationUnit,
                ButtonEvent,
                Requires,
                Descript,
                EffectsTitle,
                Effects
            });
            TopMost = true;
            MinimumSize = Size = new((int)(500 * SizeRatio), 500);
            BackColor = backColor;
            Location = new(
                (Screen.GetBounds(this).Width / 2) - (this.Width / 2),
                (Screen.GetBounds(this).Height / 2) - (this.Height / 2)
                );
            //MinimizeBox = MaximizeBox = false;
            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }

        public string FocusNameText
        {
            get => FocusName.Text;
            set => FocusName.Text = value;
        }

        public string DurationText
        {
            get => Duration.Text;
            set => Duration.Text = value;
        }

        public string EffectsText
        {
            get => Effects.Text;
            set => Effects.Text = value;
        }

        public string DescriptText
        {
            get => Descript.Text;
            set => Descript.Text = value;
        }

        System.Windows.Forms.TextBox FocusName = new();
        System.Windows.Forms.PictureBox FocusIcon = new();
        System.Windows.Forms.TextBox Duration = new();
        System.Windows.Forms.PictureBox DurationUnit = new();
        System.Windows.Forms.Button ButtonEvent = new();
        System.Windows.Forms.RichTextBox Requires = new();
        System.Windows.Forms.RichTextBox Descript = new();
        System.Windows.Forms.PictureBox EffectsTitle = new();
        System.Windows.Forms.RichTextBox Effects = new();
        List<System.Windows.Forms.TextBoxBase> textBoxList = new();

        #endregion

        #region ==== 历史工具 ====

        public int HistoryIndex { get; set; } = 0;
        public int CurrentHistoryLength { get; set; } = 0;
        public string[] History { get; set; } = new string[50];
        public int LatestIndex { get; set; } = 0;

        public string HashCachePath => this.GetCacheFilePath("hash test");

        public string FileManageDirName => "NODE_NFO_DLG";

        public string ToHashString()
        {
            this.SaveToXml(HashCachePath, new NodeInfoDialogSerialization());
            using var data = new FileStream(HashCachePath, FileMode.Open);
            var hashString = data.ToMd5HashString();
            if (!File.Exists(hashString))
                this.SaveToXml(this.GetCacheFilePath(hashString), new NodeInfoDialogSerialization());
            return hashString;
        }

        public string ToHashString(string filePath)
        {
            new NodeInfoDialogSerialization().LoadFromXml(filePath)
                ?.SaveToXml(HashCachePath, new NodeInfoDialogSerialization());
            using var data = new FileStream(HashCachePath, FileMode.Open);
            return data.ToMd5HashString();
        }

        public void FromHashString(string data)
        {
            var nodeInfoDlg = new NodeInfoDialogSerialization().LoadFromXml(this.GetCacheFilePath(data));
            if (nodeInfoDlg is null)
                return;
            FocusNameText = nodeInfoDlg.FocusNameText;
            DurationText = nodeInfoDlg.DurationText;
            DescriptText = nodeInfoDlg.DescriptText;
            EffectsText = nodeInfoDlg.EffectsText;
        }

        #endregion
    }
}
