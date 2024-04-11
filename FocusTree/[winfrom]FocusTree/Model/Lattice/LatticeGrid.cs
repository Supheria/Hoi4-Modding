#define DEBUG


namespace FocusTree.Model.Lattice
{
    /// <summary>
    /// 栅格
    /// </summary>
    internal static class LatticeGrid
    {
        private const float FloatComparisonTolerance = 0.1f;
        /// <summary>
        /// 栅格绘图区域矩形
        /// </summary>
        public static Rectangle DrawRect { get; set; }
        /// <summary>
        /// 栅格坐标系原点X坐标
        /// </summary>
        public static int OriginX { get; set; }
        /// <summary>
        /// 栅格坐标系原点Y坐标
        /// </summary>
        public static int OriginY { get; set; }
        /// <summary>
        /// 格元边框绘制用笔
        /// </summary>
        public static Pen CellPen { get; } = new(Color.FromArgb(200, Color.AliceBlue), 1.5f);
        /// <summary>
        /// 节点边框绘制用笔（直线）
        /// </summary>
        public static Pen NodePenLine { get; } = new(Color.FromArgb(150, Color.Orange), 2f);
        /// <summary>
        /// 节点边框绘制用笔（虚线）
        /// </summary>
        public static Pen NodePenDash { get; } = new(Color.FromArgb(150, Color.Orange), 2f)
        {
            DashStyle = System.Drawing.Drawing2D.DashStyle.Custom,
            DashPattern = new float[] { 1.25f, 1.25f },
        };
        /// <summary>
        /// 坐标辅助线绘制用笔
        /// </summary>
        public static Pen GuidePen { get; } = new(Color.FromArgb(200, Color.Red), 1.75f);

        /// <summary>
        /// 绘制无限制栅格，并调用绘制委托列表
        /// </summary>
        /// <param name="image"></param>
        public static void Draw(Image image)
        {
            var g = Graphics.FromImage(image);
            DrawLatticeCells(g);
            //
            // draw guide line
            //
            g.DrawLine(GuidePen, new(OriginX, DrawRect.Top), new(OriginX, DrawRect.Bottom));
            g.DrawLine(GuidePen, new(DrawRect.Left, OriginY), new(DrawRect.Right, OriginY));
            g.Flush(); g.Dispose();
            //Program.testInfo.InfoText = $"{new Point(ColNumber, RowNumber)}";
            //Program.testInfo.InfoText = $"{Drawing.MethodNumber()}\n" +
            //    $"1. {Drawing.MethodNumber(0)}, 2. {Drawing.MethodNumber(1)}, 3. {Drawing.MethodNumber(2)}";
        }

        /// <summary>
        /// 绘制循环格元（格元左上角坐标与栅格坐标系中心偏移量近似投射在一个格元大小范围内）
        /// </summary>
        /// <param name="g"></param>
        private static void DrawLatticeCells(Graphics g)
        {
            var cell = new LatticeCell();
            var cellRect = cell.CellRealRect;
            var xMany = DrawRect.X - OriginX;
            var yMany = DrawRect.Y - OriginY;
            var colOffset = xMany / cellRect.Width - (xMany < 0 ? 1 : 0);
            var rowOffset = yMany / cellRect.Height - (yMany < 0 ? 1 : 0);
            cell.LatticedPoint.Col = colOffset;
            cell.LatticedPoint.Row = rowOffset;
            var colNum = DrawRect.Width / cellRect.Width + 2;
            var rowNum = DrawRect.Height / cellRect.Height + 2;
            for (var i = 0; i < colNum; i++)
            {
                cell.LatticedPoint.Row = rowOffset;
                for (var j = 0; j < rowNum; j++)
                {
                    cellRect = cell.CellRealRect;
                    //
                    // draw cell
                    //
                    if (CrossLineWithin(
                        new(cellRect.Left, cellRect.Bottom),
                        new(cellRect.Right, cellRect.Bottom),
                        CellPen.Width,
                        out var p1, out var p2
                        ))
                        g.DrawLine(CellPen, p1, p2);
                    if (CrossLineWithin(
                        new(cellRect.Right, cellRect.Top),
                        new(cellRect.Right, cellRect.Bottom),
                        CellPen.Width,
                        out p1, out p2
                        ))
                        g.DrawLine(CellPen, p1, p2);
                    var saveAll = RectWithin(cell.NodeRealRect, out var saveRect);
                    //
                    // draw node
                    //
                    if (saveRect is not null)
                    {
                        if (saveAll)
                            g.DrawRectangle(NodePenLine, saveRect.Value);
                        else
                            g.DrawRectangle(NodePenDash, saveRect.Value);
                    }
                    cell.LatticedPoint.Row++;
                }
                cell.LatticedPoint.Col++;
            }
        }

        /// <summary>
        /// 获取给定的矩形在栅格绘图区域内的矩形
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="saveRect"></param>
        /// <returns>完全在绘图区域内返回true，否则返回false</returns>
        public static bool RectWithin(Rectangle rect, out Rectangle? saveRect)
        {
            saveRect = null;
            bool saveAll = true;
            var left = rect.Left;
            var right = rect.Right;
            var top = rect.Top;
            var bottom = rect.Bottom;
            if (left < DrawRect.Left)
            {
                saveAll = false;
                if (right <= DrawRect.Left)
                    return false;
                left = DrawRect.Left;
            }
            if (right > DrawRect.Right)
            {
                saveAll = false;
                if (left >= DrawRect.Right)
                    return false;
                right = DrawRect.Right;
            }
            if (top < DrawRect.Top)
            {
                saveAll = false;
                if (bottom <= DrawRect.Top)
                    return false;
                top = DrawRect.Top;
            }
            if (bottom > DrawRect.Bottom)
            {
                saveAll = false;
                if (top >= DrawRect.Bottom)
                    return false;
                bottom = DrawRect.Bottom;
            }
            saveRect = new(left, top, right - left, bottom - top);
            return saveAll;
        }

        /// <summary>
        /// 获取给定横纵直线在栅格绘图区域内的矩形
        /// </summary>
        /// <param name="p1">直线的端点</param>
        /// <param name="p2">直线的另一端点</param>
        /// <param name="lineWidth">直线的宽度</param>
        /// <param name="endMin"></param>
        /// <param name="endMax"></param>
        /// <returns></returns>
        public static bool CrossLineWithin(PointF p1, PointF p2, float lineWidth, out PointF endMin, out PointF endMax)
        {
            endMin = endMax = PointF.Empty;
            var halfLineWidth = (lineWidth / 2);
            if (Math.Abs(p1.Y - p2.Y) < FloatComparisonTolerance)
            {
                if (!CrossLineWithin(
                    p1.Y, 
                    DrawRect.Top, DrawRect.Bottom, 
                    (p1.X + halfLineWidth, p2.X + halfLineWidth), 
                    DrawRect.Left, DrawRect.Right, 
                    out var xMin, out var xMax
                    ))
                    return false;
                endMin = new(xMin, p1.Y);
                endMax = new(xMax, p1.Y);
            }
            else
            {
                if (!CrossLineWithin(
                    p1.X, 
                    DrawRect.Left, DrawRect.Right, 
                    (p1.Y + halfLineWidth, p2.Y + halfLineWidth), 
                    DrawRect.Top, DrawRect.Bottom, 
                    out var yMin, out var yMax
                    ))
                    return false;
                endMin = new(p1.X, yMin);
                endMax = new(p1.X, yMax);
            }
            return true;
        }
        private static bool CrossLineWithin(float theSame, float theSameLimitMin, float theSameLimitMax, (float, float) ends, float endLimitMin, float endLimitMax, out float endMin, out float endMax)
        {
            endMin = endMax = 0;
            if (Math.Abs(ends.Item1 - ends.Item2) < FloatComparisonTolerance || theSame < theSameLimitMin || theSame > theSameLimitMax) { return false; }
            endMin = Math.Min(ends.Item1, ends.Item2);
            endMax = Math.Max(ends.Item1, ends.Item2);
            if (endMin >= endLimitMax) { return false; }
            if (endMax <= endLimitMin) { return false; }
            if (endMin < endLimitMin) { endMin = endLimitMin; }
            if (endMax > endLimitMax) { endMax = endLimitMax; }
            return true;
        }
    }
}
