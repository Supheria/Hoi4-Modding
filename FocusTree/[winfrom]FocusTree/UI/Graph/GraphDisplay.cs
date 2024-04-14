//#define MOUSE_DRAG_FREE
using FocusTree.Model.Focus;
using FocusTree.Model.Lattice;
using FocusTree.Model.WinFormGdiUtilities;
using FocusTree.UI.Controls;
using FocusTree.UI.NodeToolDialogs;
using LocalUtilities.FileUtilities;

namespace FocusTree.UI.Graph
{
    public class GraphDisplay : PictureBox
    {
        #region ---- 关联控件 ----

        public new readonly GraphForm Parent;
        /// <summary>
        /// 工具对话框集（给父窗口下拉菜单“窗口”用）
        /// </summary>
        public readonly Dictionary<string, ToolDialog> ToolDialogs = new()
        {
            ["国策信息"] = new ToolDialog()
        };
        /// <summary>
        /// 国策信息对话框
        /// </summary>
        private NodeInfoDialog NodeInfo
        {
            get => (NodeInfoDialog)ToolDialogs["国策信息"];
            init => ToolDialogs["国策信息"] = value;
        }
        /// <summary>
        /// 节点信息浮标
        /// </summary>
        private readonly ToolTip _nodeInfoTip = new();

        #endregion

        #region ==== 事件指示器 ====

        /// <summary>
        /// 已选中的节点
        /// </summary>
        public FocusNode? SelectedNode
        {
            get => _selectedNode;
            private set
            {
                _selectedNode = value;
                _prevSelectNode = null;
            }
        }

        private FocusNode? _selectedNode;
        /// <summary>
        /// 预选中的节点
        /// </summary>
        private FocusNode? _prevSelectNode;
        /// <summary>
        /// 图像拖动指示器
        /// </summary>
        private bool _dragGraphFlag = false;
        /// <summary>
        /// 拖动节点指示器
        /// </summary>
        private bool _dragNodeFlag = false;

        #endregion

        #region ---- 绘图工具 ----

        /// <summary>
        /// 信息展示条区域
        /// </summary>
        private Rectangle InfoBrandRect => new(Left, Bottom - 100, Width, 75);

        /// <summary>
        /// 鼠标移动灵敏度（值越大越迟顿）
        /// </summary>
#if MOUSE_DRAG_FREE
        static int MouseMoveSensibility = 1;
#else
        private const int MouseMoveSensibility = 20;

#endif
        /// <summary>
        /// 拖动事件使用的鼠标参照坐标
        /// </summary>
        private Point _dragMouseFlagPoint = new(0, 0);
        /// <summary>
        /// 格元放置边界
        /// </summary>
        public Rectangle LatticeBound
        {
            get
            {
                var left = Left + 30;
                var right = left + Width - 60;
                var top = Top;
                var bottom = InfoBrandRect.Top - 30;
                var bkRight = Background.Size.Width;
                var bkBottom = Background.Size.Height;
                if (right > bkRight)
                {
                    right = bkRight;
                }
                if (bottom > bkBottom)
                {
                    bottom = bkBottom;
                }
                return new(left, top, right - left, bottom - top);
            }
        }
        /// <summary>
        /// 刷新时调用的绘图委托（两层）
        /// </summary>
        public DrawLayers OnRefresh = new(2);
        /// <summary>
        /// 是否绘制背景栅格
        /// </summary>
        public bool DrawBackLattice = false;

        #endregion

        #region ---- 初始化 ----

        public GraphDisplay(GraphForm mainForm)
        {
            base.Parent = Parent = mainForm;
            NodeInfo = new NodeInfoDialog(this);

            SizeChanged += GraphDisplay_SizeChanged;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;
            MouseDoubleClick += OnMouseDoubleClick;
        }

        #endregion

        #region ==== 绘图 ====

        /// <summary>
        /// 将节点绘制上载到栅格绘图委托（初始化节点列表时仅需上载第一次，除非节点列表或节点关系或节点位置信息发生变更才重新上载）
        /// </summary>
        private void UploadNodeMap()
        {
            OnRefresh.Clear();
            foreach (var focus in GraphBox.FocusList)
            {
                foreach (var require in focus.Requires.SelectMany(requires => requires.Select(GraphBox.GetFocus)))
                    OnRefresh += (1,
                        (image) => GraphDrawer.DrawRequireLine(image, focus.LatticedPoint, require.LatticedPoint));

                OnRefresh += (0, (image) => GraphDrawer.DrawFocusNodeNormal(image, focus));
            }
        }
        public void DrawNodeMapInfo()
        {
            DrawInfo($"节点数量：{GraphBox.NodeCount}，分支数量：{GraphBox.BranchCount}",
                new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                );
        }
        public void DrawInfo(string info)
        {
            DrawInfo(info,
                new SolidBrush(Color.FromArgb(160, Color.DarkGray)),
                new SolidBrush(Color.FromArgb(255, Color.WhiteSmoke))
                );
        }
        private void DrawInfo(string info, Brush backBrush, Brush frontBrush)
        {
            var g = Graphics.FromImage(Image);
            Rectangle infoRect = new(Bounds.Left, Bounds.Bottom - 100, Bounds.Width, 66);
            g.FillRectangle(backBrush, infoRect);
            g.DrawString(
                info,
                new Font(GraphDrawer.InfoFont, 25, FontStyle.Bold, GraphicsUnit.Pixel),
                frontBrush,
                infoRect,
                GraphDrawer.NodeFontFormat);
        }

        #endregion

        #region ---- 事件 ----

        private void GraphDisplay_SizeChanged(object sender, EventArgs e)
        {
            Image?.Dispose();
            Image = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            LatticeGrid.GridData.DrawRect = LatticeBound;
            Refresh();
        }

        //---- OnMouseDown ----//

        private void OnMouseDown(object sender, MouseEventArgs args)
        {
            if (CheckPrevSelect())
            {
                switch (args.Button)
                {
                    case MouseButtons.Left:
                        NodeLeftClicked();
                        break;
                    case MouseButtons.Right:
                        NodeRightClicked();
                        break;
                }
            }
            else
            {
                switch (args.Button)
                {
                    case MouseButtons.Left:
                        GraphLeftClicked(args.Location);
                        break;
                    case MouseButtons.Right:
                        OpenGraphContextMenu(args.Button);
                        Parent.UpdateText("打开图像选项");
                        break;
                    case MouseButtons.Middle:
                        OpenGraphContextMenu(args.Button);
                        Parent.UpdateText("打开备份选项");
                        break;
                }
            }
        }
        private void GraphLeftClicked(Point startPoint)
        {
            _dragGraphFlag = true;
            _dragMouseFlagPoint = startPoint;
            Invalidate();
            Parent.UpdateText("拖动图像");
        }

        private Bitmap? _lineMapCache;
        private Bitmap? _backgroundCache;
        private void NodeLeftClicked()
        {
            _dragNodeFlag = true;
            _lineMapCache = new(Image.Width, Image.Height);
            _backgroundCache = new(Image.Width, Image.Height);
            DrawBackLattice = true;
            Refresh();
            OnRefresh.Invoke(_lineMapCache, 1);
            Background.Redraw(_backgroundCache);
            _backgroundCache.DrawLatticeGrid();
            OnRefresh.Invoke(_backgroundCache, 0);
            var info = $"{_prevSelectNode?.Name}, {_prevSelectNode?.Duration}日\n{_prevSelectNode?.Description}";
            DrawInfo(info);
            Parent.UpdateText("选择节点");
        }
        private void NodeRightClicked()
        {
            CloseAllNodeToolDialogs();
            _nodeInfoTip.Hide(this);
            SelectedNode = _prevSelectNode;
            CameraLocateSelectedNode(false);
            _ = new NodeContextMenu(this, Cursor.Position);
            Parent.UpdateText("打开节点选项");
        }
        private void OpenGraphContextMenu(MouseButtons button)
        {
            SelectedNode = _prevSelectNode;
            _nodeInfoTip.Hide(this);
            _ = new GraphContextMenu(this, Cursor.Position, button);
        }

        //---- OnMouseDoubleClick ----//

        private void OnMouseDoubleClick(object sender, MouseEventArgs args)
        {
            if (CheckPrevSelect())
            {
                // node double clicked
            }
            else
            {
                if (args.Button == MouseButtons.Left)
                {
                    GraphLeftDoubleClicked();
                }
            }
        }
        private void GraphLeftDoubleClicked()
        {
            if (SelectedNode != null)
            {
                //var focus = SelectedNode.Value;
                SelectedNode = null;
            }
            if (GraphBox.ReadOnly && MessageBox.Show("[202303052340]是否恢复备份？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                GraphBox.Graph?.Backup(GraphBox.FilePath);
                GraphBox.Save();
                RefreshGraphBox();
                Parent.UpdateText("恢复备份");
            }
        }

        //---- OnMouseMove ----//

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            if (args.Button == MouseButtons.Left && _dragGraphFlag)
            {
                DragGraph(args.Location);
            }
            else if (args.Button == MouseButtons.Left && _dragNodeFlag)
            {
                DragNode(args.Location);
            }
            else if (args.Button == MouseButtons.None)
            {
                ShowNodeInfoTip(args.Location);
            }

            //Invalidate();
        }
        private void DragGraph(Point newPoint)
        {
            var diffInWidth = newPoint.X - _dragMouseFlagPoint.X;
            var diffInHeight = newPoint.Y - _dragMouseFlagPoint.Y;
            if (Math.Abs(diffInWidth) > MouseMoveSensibility || Math.Abs(diffInHeight) > MouseMoveSensibility)
            {
#if MOUSE_DRAG_FREE
                Lattice.OriginX += (newPoint.X - DragMouseFlagPoint.X) / MouseMoveSensibility;
                Lattice.OriginTop += (newPoint.Y - DragMouseFlagPoint.Y) / MouseMoveSensibility;
#else
                LatticeGrid.GridData.OriginX += (newPoint.X - _dragMouseFlagPoint.X) / MouseMoveSensibility * LatticeCell.CellData.EdgeLength;
                LatticeGrid.GridData.OriginY += (newPoint.Y - _dragMouseFlagPoint.Y) / MouseMoveSensibility * LatticeCell.CellData.EdgeLength;
#endif
                _dragMouseFlagPoint = newPoint;
                Refresh();
                DrawNodeMapInfo();
            }
        }
        /// <summary>
        /// 上次光标所处的节点部分
        /// </summary>
        LatticeCell.Parts LastCellPart = LatticeCell.Parts.Leave;
        LatticedPoint LatticedPointCursorOn;
        FocusNode FocusNodeToDrag = new();
        Rectangle LastPartRealRect;
        bool FirstDrag = true;
        private void DragNode(Point newPoint)
        {
            if (GraphBox.ReadOnly) { return; }
            _nodeInfoTip.Hide(this);
            var cell = new LatticeCell(newPoint);
            LatticedPointCursorOn = cell.LatticedPoint;
            var cellPart = cell.PointOnCellPart(newPoint);
            if (cellPart == LastCellPart) { return; }
            ImageDrawer.DrawImageOn(_backgroundCache, (Bitmap)Image, LastPartRealRect, true);
            ImageDrawer.DrawImageOn(_lineMapCache, (Bitmap)Image, LastPartRealRect, true);
            LastCellPart = cellPart;
            if (GraphBox.PointInAnyFocusNode(newPoint, out var focus))
            {
                if (FirstDrag)
                {
                    FocusNodeToDrag = focus;
                    FirstDrag = false;
                }
                GraphDrawer.DrawFocusNodeSelected((Bitmap)Image, focus);
                LastPartRealRect = cell.NodeRealRect();
            }
            else
            {
                GraphDrawer.DrawSelectedCellPart((Bitmap)Image, LatticedPointCursorOn, cellPart);
                LastPartRealRect = cell.CellPartsRealRect(cellPart);
                ImageDrawer.DrawImageOn(_lineMapCache, (Bitmap)Image, LastPartRealRect, true);
            }
            Parent.Text = $"CellSideLength {LatticeCell.CellData.EdgeLengthMin}, o: {LatticeGrid.GridData.OriginX}, {LatticeGrid.GridData.OriginY}, cursor: {newPoint}, cellPart: {LastCellPart}";
            Invalidate();
        }

        private void ShowNodeInfoTip(Point location)
        {
            if (!GraphBox.PointInAnyFocusNode(location, out var focus))
            {
                _nodeInfoTip.Hide(this);
                return;
            }
            _nodeInfoTip.BackColor = Color.FromArgb(0, Color.AliceBlue);
            _nodeInfoTip.Show($"{focus?.Name}\nID: {focus?.Signature}", this, location.X + 10, location.Y);
        }

        //---- OnMouseUp ----//

        private void OnMouseUp(object sender, MouseEventArgs args)
        {
            if (_dragGraphFlag)
            {
                _dragGraphFlag = false;
            }
            if (_dragNodeFlag)
            {
                _lineMapCache?.Dispose();
                _backgroundCache?.Dispose();
                FirstDrag = true;
                _dragNodeFlag = false;
                LastCellPart = LatticeCell.Parts.Leave;
                DrawBackLattice = false;
                FocusNodeToDrag.LatticedPoint = LatticedPointCursorOn;
                if (GraphBox.ContainLatticedPoint(LatticedPointCursorOn))
                {
                    Refresh();
                }
                else
                {
                    GraphBox.SetFocus(FocusNodeToDrag);
                    RefreshGraphBox();
                }
            }
        }

        //---- OnMouseWheel ----//

        private void OnMouseWheel(object sender, MouseEventArgs args)
        {
            var drWidth = LatticeGrid.GridData.DrawRect.Width;
            var drHeight = LatticeGrid.GridData.DrawRect.Height;
            var diffInWidth = args.Location.X - Width / 2;
            var diffInHeight = args.Location.Y - Height / 2;
            LatticeGrid.GridData.OriginX += diffInWidth / LatticeCell.CellData.EdgeLength * drWidth / 200;
            LatticeGrid.GridData.OriginY += diffInHeight / LatticeCell.CellData.EdgeLength * drHeight / 200;
            LatticeCell.CellData.EdgeLength += args.Delta / 100 * Math.Max(drWidth, drHeight) / 200;
            Refresh();
            Parent.UpdateText("打开节点选项");
        }

        //---- Public ----//

        /// <summary>
        /// 检查预选择节点
        /// </summary>
        private bool CheckPrevSelect()
        {
            var clickPos = PointToClient(Cursor.Position);
            return GraphBox.PointInAnyFocusNode(clickPos, out _prevSelectNode);
        }
        private void CloseAllNodeToolDialogs()
        {
            ToolDialogs.ToList().ForEach(x => x.Value.Close());
        }

        #endregion

        #region ==== 镜头操作 ====

        /// <summary>
        /// 缩放居中至全景
        /// </summary>
        public void CameraLocatePanorama()
        {
            if (GraphBox.Graph is null)
                return;
            var gRect = GraphBox.MetaRect;
            var gridData = LatticeGrid.GridData;
            var cellData = LatticeCell.CellData;
            //
            // 自适应大小
            //
            if (gridData.DrawRect.Width < (gRect.Width) * cellData.EdgeLengthMin)
            {
                cellData.EdgeLength = cellData.EdgeLengthMin;
                Parent.Width = (gRect.Width + 1) * cellData.EdgeLengthMin + Parent.Width - gridData.DrawRect.Width;
            }
            if (gridData.DrawRect.Height < (gRect.Height) * cellData.EdgeLengthMin)
            {
                cellData.EdgeLength = cellData.EdgeLengthMin;
                Parent.Height = (gRect.Height + 1) * cellData.EdgeLengthMin + Parent.Height - gridData.DrawRect.Height;
            }
            Background.DrawNew(Image);
            gridData.DrawRect = LatticeBound;
            //
            //
            //
            cellData.EdgeLength = Math.Min(gridData.DrawRect.Width / (gRect.Width + 1), gridData.DrawRect.Height / (gRect.Height + 1));
            gridData.OriginX = (gridData.DrawRect.Left + gridData.DrawRect.Width / 2) - (gRect.Left + gRect.Width * cellData.EdgeLength / 2);
            gridData.OriginY = (gridData.DrawRect.Top + gridData.DrawRect.Height / 2) - (gRect.Top + gRect.Height * cellData.EdgeLength / 2);
            Refresh();
        }
        /// <summary>
        /// 居中或可缩放至选中的国策节点
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="zoom">是否聚焦</param>
        public void CameraLocateSelectedNode(bool zoom)
        {
            if (SelectedNode is null || GraphBox.Graph is null)
                return;
            var gridData = LatticeGrid.GridData;
            var cellData = LatticeCell.CellData;
            if (zoom)
            {
                cellData.EdgeLength = cellData.EdgeLengthMax;
            }
            var cell = new LatticeCell(SelectedNode.LatticedPoint);
            var nodeRect = cell.NodeRealRect();
            var halfNodeWidth = nodeRect.Width / 2;
            var halfNodeHeight = nodeRect.Height / 2;
            gridData.OriginX += (gridData.DrawRect.Left + gridData.DrawRect.Width / 2) - (nodeRect.Left + halfNodeWidth);
            gridData.OriginY += (gridData.DrawRect.Top + gridData.DrawRect.Height / 2) - (nodeRect.Top + halfNodeHeight);
            gridData.DrawRect = LatticeBound;
            Refresh();
            Cursor.Position = PointToScreen(new Point(
                nodeRect.Left + halfNodeWidth,
                nodeRect.Top + halfNodeHeight
                ));
        }

        #endregion

        #region ==== 读写操作调用 ====

        /// <summary>
        /// 重置显示器
        /// </summary>
        public void ResetDisplay()
        {
            CloseAllNodeToolDialogs();
            SelectedNode = null;
            UploadNodeMap();
            CameraLocatePanorama();
        }
        /// <summary>
        /// 更改 GraphBox 后刷新显示
        /// </summary>
        public void RefreshGraphBox()
        {
            UploadNodeMap();
            Refresh();
        }
        /// <summary>
        /// 刷新显示（不重新上载绘图委托）
        /// </summary>
        public new void Refresh()
        {
            Background.Redraw(Image);
            DrawBackLattice = true;
            if (DrawBackLattice) { Image.DrawLatticeGrid(); }
            OnRefresh.Invoke((Bitmap)Image);
            Invalidate();
        }
        /// <summary>
        /// 弹出节点信息对话框
        /// </summary>
        public void ShowNodeInfo()
        {
            NodeInfo.Show();
        }

        #endregion
    }
}
