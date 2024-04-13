#define DEBUG

using LocalUtilities.FileUtilities;

namespace FocusTree.Model.Lattice;

/// <summary>
/// 栅格
/// </summary>
public static class LatticeGrid
{
    public static GridData GridData { get; set; } = new GridDataXmlSerialization().LoadFromXml(out _);
    /// <summary>
    /// 绘制无限制栅格，并调用绘制委托列表
    /// </summary>
    /// <param name="image"></param>
    public static void DrawLatticeGrid(this Image image)
    {
        var g = Graphics.FromImage(image);
        DrawLatticeCells(g);
        //
        // draw guide line
        //
        g.DrawLine(GridData.GuidePen, new(GridData.OriginX, GridData.DrawRect.Top), new(GridData.OriginX, GridData.DrawRect.Bottom));
        g.DrawLine(GridData.GuidePen, new(GridData.DrawRect.Left, GridData.OriginY), new(GridData.DrawRect.Right, GridData.OriginY));
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
        var cellRect = cell.CellRealRect();
        var xMany = GridData.DrawRect.X - GridData.OriginX;
        var yMany = GridData.DrawRect.Y - GridData.OriginY;
        var colOffset = xMany / cellRect.Width - (xMany < 0 ? 1 : 0);
        var rowOffset = yMany / cellRect.Height - (yMany < 0 ? 1 : 0);
        cell.LatticedPoint.Col = colOffset;
        cell.LatticedPoint.Row = rowOffset;
        var colNum = GridData.DrawRect.Width / cellRect.Width + 2;
        var rowNum = GridData.DrawRect.Height / cellRect.Height + 2;
        for (var i = 0; i < colNum; i++)
        {
            cell.LatticedPoint.Row = rowOffset;
            for (var j = 0; j < rowNum; j++)
            {
                cellRect = cell.CellRealRect();
                //
                // draw cell
                //
                if (CrossLineWithin(
                    new(cellRect.Left, cellRect.Bottom),
                    new(cellRect.Right, cellRect.Bottom),
                    GridData.CellPen.Width,
                    out var p1, out var p2
                    ))
                    g.DrawLine(GridData.CellPen, p1, p2);
                if (CrossLineWithin(
                    new(cellRect.Right, cellRect.Top),
                    new(cellRect.Right, cellRect.Bottom),
                    GridData.CellPen.Width,
                    out p1, out p2
                    ))
                    g.DrawLine(GridData.CellPen, p1, p2);
                var saveAll = RectWithin(cell.NodeRealRect(), out var saveRect);
                //
                // draw node
                //
                if (saveRect is not null)
                {
                    if (saveAll)
                        g.DrawRectangle(GridData.NodePenLine, saveRect.Value);
                    else
                        g.DrawRectangle(GridData.NodePenDash, saveRect.Value);
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
        if (left < GridData.DrawRect.Left)
        {
            saveAll = false;
            if (right <= GridData.DrawRect.Left)
                return false;
            left = GridData.DrawRect.Left;
        }
        if (right > GridData.DrawRect.Right)
        {
            saveAll = false;
            if (left >= GridData.DrawRect.Right)
                return false;
            right = GridData.DrawRect.Right;
        }
        if (top < GridData.DrawRect.Top)
        {
            saveAll = false;
            if (bottom <= GridData.DrawRect.Top)
                return false;
            top = GridData.DrawRect.Top;
        }
        if (bottom > GridData.DrawRect.Bottom)
        {
            saveAll = false;
            if (top >= GridData.DrawRect.Bottom)
                return false;
            bottom = GridData.DrawRect.Bottom;
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
        if (Math.Abs(p1.Y - p2.Y) < GridData.FloatComparisonTolerance)
        {
            if (!CrossLineWithin(
                p1.Y,
                GridData.DrawRect.Top, GridData.DrawRect.Bottom,
                (p1.X + halfLineWidth, p2.X + halfLineWidth),
                GridData.DrawRect.Left, GridData.DrawRect.Right,
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
                GridData.DrawRect.Left, GridData.DrawRect.Right,
                (p1.Y + halfLineWidth, p2.Y + halfLineWidth),
                GridData.DrawRect.Top, GridData.DrawRect.Bottom,
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
        if (Math.Abs(ends.Item1 - ends.Item2) < GridData.FloatComparisonTolerance || theSame < theSameLimitMin || theSame > theSameLimitMax) { return false; }
        endMin = Math.Min(ends.Item1, ends.Item2);
        endMax = Math.Max(ends.Item1, ends.Item2);
        if (endMin >= endLimitMax) { return false; }
        if (endMax <= endLimitMin) { return false; }
        if (endMin < endLimitMin) { endMin = endLimitMin; }
        if (endMax > endLimitMax) { endMax = endLimitMax; }
        return true;
    }
}
