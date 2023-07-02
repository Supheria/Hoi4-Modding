using System.Text;
using FocusTree.Data.Focus;
using FocusTree.IO;

namespace FocusTree.Utilities.test
{
    public partial class TestInfo : Form
    {
        private bool DoWrap
        {
            get => ToolStripMenuItemLook_Wrap.Checked;
            set => ToolStripMenuItemLook_Wrap.Checked = value;
        }

        HashSet<string> _infoText = new();
        public int Total = 0;
        public int Error = 0;
        public int Differ = 0;
        public int Good = 0;
        public TestInfo()
        {
            InitializeComponent();
            Info.Left = ClientRectangle.Left;
            Info.Top = ClientRectangle.Top;
            Info.Width = ClientRectangle.Width;
            Info.Height = ClientRectangle.Height;
            Info.Dock = DockStyle.Fill;
            Info.WordWrap = false;
            Info.ZoomFactor = 2f;
            Closing += (sender, args) =>
            {
                Hide();
                Renew();
                args.Cancel = true;
            };
            //TopMost = true;
            //testFormatter.Show();
        }

        public void Append(string text)
        {
            _infoText.Add(text);
            var sb = new StringBuilder().AppendLine($"错误 {Error}/{Total}, 差异 {Differ}/{Total}, 正确 {Good}/{Total}")
                .AppendLine();
            foreach (var info in _infoText)
                sb.AppendLine(info);
            Info.Text = sb.ToString();
        }

        public void Initialize()
        {
            _infoText = new();
            Total = 0;
            Error = 0;
            Differ = 0;
            Good = 0;
        }

        public void Renew()
        {
            _infoText = new();
            Total = 0;
            Error = 0;
            Differ = 0;
            Good = 0;
        }

        private void ToolStripMenuItemLook_Wrap_Click(object sender, EventArgs e)
        {
            if (DoWrap)
            {
                DoWrap = true;
                Info.WordWrap = true;
            }
            else
            {
                DoWrap = false;
                Info.WordWrap = false;
            }
        }
    }
}
