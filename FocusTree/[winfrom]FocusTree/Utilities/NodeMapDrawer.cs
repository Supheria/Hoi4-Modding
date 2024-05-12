using FocusTree.Model.Focus;
using System.Numerics;

namespace FocusTree.Utilities
{
    /// <summary>
    /// 输出带国策信息的图片
    /// </summary>
    public class NodeMapDrawer : Form
    {
        private ProgressBar Progress;
        float Percent { get { return Progress.Value / (float)Progress.Maximum * 100; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count">进度条的最大值</param>
        private NodeMapDrawer(int count)
        {
            InitializeComponent();
            Visible = true;
            Progress.Minimum = 0;
            Progress.Maximum = count;
            Progress.Value = 0;
            Progress.Step = 1;
        }
        private void InitializeComponent()
        {
            Progress = new ProgressBar();
            SuspendLayout();
            // 
            // Progress
            // 
            Progress.Dock = DockStyle.Fill;
            Progress.ForeColor = SystemColors.ActiveCaption;
            Progress.Location = new Point(0, 0);
            Progress.Name = "Progress";
            Progress.Size = new Size(305, 33);
            Progress.Style = ProgressBarStyle.Continuous;
            Progress.TabIndex = 0;
            // 
            // NodeMapDrawer
            // 
            ClientSize = new Size(305, 33);
            Controls.Add(Progress);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "NodeMapDrawer";
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            ResumeLayout(false);
        }
        /// <summary>
        /// 进度条步进下一刻
        /// </summary>
        private void StepNext()
        {
            Progress.PerformStep();
        }

        #region ==== 输出图片 ====

        private static NodeMapDrawer? _drawer;
        public static void SaveImage(FocusGraph? graph, string toSavePath)
        {
            if (graph is null)
                return;
            var canvas = GetCanvas(graph);
            var g = Graphics.FromImage(canvas);
            g.Clear(Color.White);

            _drawer = new(graph.GetRosterList().Count + 1);
            foreach (var focus in graph.GetRosterList())
            {
                DrawNodeLinks(g, graph, focus);
            }
            foreach (var focus in graph.GetRosterList())
            {
                var drawingRect = NodeDrawingRect(focus);
                g.FillRectangle(
                    new SolidBrush(Color.FromArgb(60, Color.DimGray)),
                    drawingRect
                    );
                DrawNodeInfo(g, drawingRect, focus);
                _drawer.StepNext();
                _drawer.Text = $"{graph.Name}.jpg: {(int)_drawer.Percent}%";
            }
            g.Flush();
            g.Dispose();

            toSavePath = Path.ChangeExtension(toSavePath, ".jpg");
            canvas.Save(toSavePath);
            canvas.Dispose();
            _drawer.Close();
        }

        #endregion

        #region ==== 绘图信息 ====

        private static readonly SizeF NodeSize = new(600f, 666f);
        private static Vector2 ScalingUnit => new(NodeSize.Width + 10f, NodeSize.Height + 80f);
        private const float Border = 1000f;

        private static Image GetCanvas(FocusGraph graph)
        {
            var rect = graph.GetGraphLatticedRect();
            Point center = new(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            var size = new Size(
                (int)(center.X * ScalingUnit.X + Border * 2),
                (int)(center.Y * ScalingUnit.Y + Border * 2)
                );
            return new Bitmap(size.Width, size.Height);
        }
        private static RectangleF NodeDrawingRect(FocusNode focus)
        {
            var point = focus.LatticedPoint;
            return new(
                    point.Col * ScalingUnit.X + Border,
                    point.Row * ScalingUnit.Y + Border,
                    NodeSize.Width,
                    NodeSize.Height
                    );
        }

        #endregion

        #region ==== 绘图 ====
        private static void DrawNodeLinks(Graphics g, FocusGraph graph, FocusNode focus)
        {
            var drawingRect = NodeDrawingRect(focus);
            var requires = focus.Requires;
            foreach (var requireGroup in requires)
            {
                foreach (var require in requireGroup)
                {
                    var todrawingRect = NodeDrawingRect(graph[require]);

                    var startLoc = new Point((int)(drawingRect.X + drawingRect.Width / 2), (int)(drawingRect.Y + drawingRect.Height / 2)); // x -> 中间, y -> 下方
                    var endLoc = new Point((int)(todrawingRect.X + todrawingRect.Width / 2), (int)(todrawingRect.Y + todrawingRect.Height / 2)); // x -> 中间, y -> 上方

                    g.DrawLine(
                        new Pen(Color.FromArgb(120, Color.OrangeRed), 10),
                        startLoc, endLoc
                        );
                }
            }
        }
        private static void DrawNodeInfo(Graphics g, RectangleF drawingRect, FocusNode focus)
        {
            var name = focus.Name;
            var duration = $"{focus.Duration}日";
            var descript = focus.Description;
            var effects = string.Join("\n\n", focus.RawEffects);
            float padding = 10f;

            RectangleF nameRect = new(
                drawingRect.Left + padding,
                drawingRect.Top + padding,
                drawingRect.Width * 0.618f - padding,
                drawingRect.Height * 0.15f - padding
                );
            RectangleF durationRect = new(
                nameRect.Right + padding,
                drawingRect.Top + padding,
                drawingRect.Right - nameRect.Right - padding * 2f,
                drawingRect.Height * 0.15f - padding
                );
            RectangleF descriptRect = new(
                drawingRect.Left + padding,
                nameRect.Bottom + padding,
                drawingRect.Width - padding * 2f,
                drawingRect.Height * 0.2f
                );
            RectangleF effectsRect = new(
                drawingRect.Left + padding,
                descriptRect.Bottom + padding,
                drawingRect.Width - padding * 2f,
                drawingRect.Bottom - descriptRect.Bottom - padding * 2f
                );
            g.FillRectangles(
                new SolidBrush(Color.FromArgb(150, Color.DarkGreen)),
                new RectangleF[]{
                nameRect,
                durationRect
            });
            g.FillRectangles(
                new SolidBrush(Color.FromArgb(255, Color.White)),
                new RectangleF[]{
                descriptRect,
                effectsRect
            });

            StringFormat nodeFontFormat = new();
            nodeFontFormat.Alignment = StringAlignment.Center;
            nodeFontFormat.LineAlignment = StringAlignment.Center;
            g.DrawString(name,
                new("黑体", 35, FontStyle.Regular, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(255, Color.White)),
                nameRect, nodeFontFormat
                );
            g.DrawString(duration,
                new("黑体", 35, FontStyle.Regular, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(255, Color.White)),
                durationRect, nodeFontFormat
                );
            nodeFontFormat.Alignment = StringAlignment.Near;
            g.DrawString(descript,
                new("仿宋", 25, FontStyle.Bold, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(255, Color.Black)),
                descriptRect, nodeFontFormat
                );
            nodeFontFormat.LineAlignment = StringAlignment.Near;
            g.DrawString(effects,
                new("仿宋", 23, FontStyle.Bold, GraphicsUnit.Pixel),
                new SolidBrush(Color.FromArgb(255, Color.Black)),
                effectsRect, nodeFontFormat
                );
        }

        #endregion
    }
}
