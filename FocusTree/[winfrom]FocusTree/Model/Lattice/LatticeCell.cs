using LocalUtilities.MathBundle;
namespace FocusTree.Model.Lattice
{
    /// <summary>
    /// 格元
    /// </summary>
    public class LatticeCell
    {
        public static CellData CellData { get; set; } = new CellDataSerialization().LoadFromFile(out _);
        /// <summary>
        /// 节点与格元的间隔
        /// </summary>
        public Size NodePadding() => new(
            (int)(CellData.EdgeLength * CellData.NodePaddingWidthFactor),
            (int)(CellData.EdgeLength * CellData.NodePaddingHeightFactor
            ));

        public Rectangle CellRealRect()
        {
            return new(
            CellData.EdgeLength * LatticedPoint.Col + LatticeGrid.GridData.OriginX,
            CellData.EdgeLength * LatticedPoint.Row + LatticeGrid.GridData.OriginY,
            CellData.EdgeLength, CellData.EdgeLength
            );
        }
        /// <summary>
        /// 节点真实坐标矩形
        /// </summary>
        public Rectangle NodeRealRect()
        {
            var cellRect = CellRealRect();
            var nodePadding = NodePadding();
            return new(
                cellRect.Left + nodePadding.Width, cellRect.Top + nodePadding.Height,
                cellRect.Width - nodePadding.Width * 2, cellRect.Height - nodePadding.Height * 2);
        }
        /// <summary>
        /// 格元栅格化坐标
        /// </summary>
        public LatticedPoint LatticedPoint { get; set; }

        public LatticeCell() : this(new LatticedPoint())
        {

        }

        public LatticeCell(LatticedPoint latticedPoint)
        {
            LatticedPoint = latticedPoint;
        }
        /// <summary>
        /// 使用真实坐标创建格元
        /// </summary>
        /// <param name="realPoint"></param>
        public LatticeCell(Point realPoint)
        {
            var widthDiff = realPoint.X - LatticeGrid.GridData.OriginX;
            var heightDiff = realPoint.Y - LatticeGrid.GridData.OriginY;
            var col = widthDiff / CellData.EdgeLength;
            var raw = heightDiff / CellData.EdgeLength;
            if (widthDiff < 0) { col--; }
            if (heightDiff < 0) { raw--; }
            LatticedPoint = new(col, raw);
        }

        /// <summary>
        /// 格元各个部分的真实坐标矩形
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public Rectangle CellPartsRealRect(Direction part)
        {
            var cellRect = CellRealRect();
            var nodePadding = NodePadding();
            var nodeRect = NodeRealRect();
            return part switch
            {
                Direction.Center => nodeRect,
                Direction.Left => new(cellRect.Left, nodeRect.Top, nodePadding.Width, nodeRect.Height),
                Direction.Top => new(nodeRect.Left, cellRect.Top, nodeRect.Width, nodePadding.Height),
                Direction.Right => new(nodeRect.Right, nodeRect.Top, nodePadding.Width, nodeRect.Height),
                Direction.Bottom => new(nodeRect.Left, nodeRect.Bottom, nodeRect.Width, nodePadding.Height),
                Direction.LeftTop => new(cellRect.Left, cellRect.Top, nodePadding.Width, nodePadding.Height),
                Direction.TopRight => new(nodeRect.Right, cellRect.Top, nodePadding.Width, nodePadding.Height),
                Direction.BottomRight => new(nodeRect.Right, nodeRect.Bottom, nodePadding.Width, nodePadding.Height),
                Direction.LeftBottom => new(cellRect.Left, nodeRect.Bottom, nodePadding.Width, nodePadding.Height),
                _ => Rectangle.Empty,
            };
        }
        /// <summary>
        /// 获取坐标在格元上所处的部分
        /// </summary>
        /// <param name="point">坐标</param>
        /// <returns></returns>
        public Direction PointOnCellPart(Point point)
        {
            if (CellPartsRealRect(Direction.Center).Contains(point))
                return Direction.Center;
            if (CellPartsRealRect(Direction.Left).Contains(point))
                return Direction.Left;
            if (CellPartsRealRect(Direction.Top).Contains(point))
                return Direction.Top;
            if (CellPartsRealRect(Direction.Right).Contains(point))
                return Direction.Right;
            if (CellPartsRealRect(Direction.Bottom).Contains(point))
                return Direction.Bottom;
            if (CellPartsRealRect(Direction.LeftTop).Contains(point))
                return Direction.LeftTop;
            if (CellPartsRealRect(Direction.TopRight).Contains(point))
                return Direction.TopRight;
            if (CellPartsRealRect(Direction.BottomRight).Contains(point))
                return Direction.BottomRight;
            if (CellPartsRealRect(Direction.LeftBottom).Contains(point))
                return Direction.LeftBottom;
            return Direction.None;
        }
    }
}