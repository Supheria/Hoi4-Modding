using LocalUtilities.FileUtilities;
using LocalUtilities.Interface;
using LocalUtilities.MathBundle;
namespace FocusTree.Model.Lattice
{
    /// <summary>
    /// 格元
    /// </summary>
    public class LatticeCell
    {
        public static CellData CellData { get; set; } = new CellDataXmlSerialization().LoadFromXml(out _);
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
        /// 格元的部分
        /// </summary>
        public enum Parts
        {
            /// <summary>
            /// 离开格元
            /// </summary>
            Leave,
            /// <summary>
            /// 节点左侧区域
            /// </summary>
            Left,
            /// <summary>
            /// 节点上方区域
            /// </summary>
            Top,
            /// <summary>
            /// 节点右侧区域
            /// </summary>
            Right,
            /// <summary>
            /// 节点下方区域
            /// </summary>
            Bottom,
            /// <summary>
            /// 节点左上方区域
            /// </summary>
            LeftTop,
            /// <summary>
            /// 节点上右方区域
            /// </summary>
            TopRight,
            /// <summary>
            /// 节点左下方区域
            /// </summary>
            LeftBottom,
            /// <summary>
            /// 节点下右方区域
            /// </summary>
            BottomRight,
            /// <summary>
            /// 节点区域
            /// </summary>
            Node
        }
        /// <summary>
        /// 格元各个部分的真实坐标矩形
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public Rectangle CellPartsRealRect(Parts part)
        {
            var cellRect = CellRealRect();
            var nodePadding = NodePadding();
            var nodeRect = NodeRealRect();
            switch (part)
            {
                case Parts.Node:
                    return nodeRect;
                case Parts.Left:
                    return new(cellRect.Left, nodeRect.Top, nodePadding.Width, nodeRect.Height);
                case Parts.Top:
                    return new(nodeRect.Left, cellRect.Top, nodeRect.Width, nodePadding.Height);
                case Parts.Right:
                    return new(nodeRect.Right, nodeRect.Top, nodePadding.Width, nodeRect.Height);
                case Parts.Bottom:
                    return new(nodeRect.Left, nodeRect.Bottom, nodeRect.Width, nodePadding.Height);
                case Parts.LeftTop:
                    return new(cellRect.Left, cellRect.Top, nodePadding.Width, nodePadding.Height);
                case Parts.TopRight:
                    return new(nodeRect.Right, cellRect.Top, nodePadding.Width, nodePadding.Height);
                case Parts.BottomRight:
                    return new(nodeRect.Right, nodeRect.Bottom, nodePadding.Width, nodePadding.Height);
                case Parts.LeftBottom:
                    return new(cellRect.Left, nodeRect.Bottom, nodePadding.Width, nodePadding.Height);
                default:
                    return Rectangle.Empty;
            }
        }
        /// <summary>
        /// 获取坐标在格元上所处的部分
        /// </summary>
        /// <param name="point">坐标</param>
        /// <returns></returns>
        public Parts PointOnCellPart(Point point)
        {
            if (CellPartsRealRect(Parts.Node).Contains(point))
                return Parts.Node;
            if (CellPartsRealRect(Parts.Left).Contains(point))
                return Parts.Left;
            if (CellPartsRealRect(Parts.Top).Contains(point))
                return Parts.Top;
            if (CellPartsRealRect(Parts.Right).Contains(point))
                return Parts.Right;
            if (CellPartsRealRect(Parts.Bottom).Contains(point))
                return Parts.Bottom;
            if (CellPartsRealRect(Parts.LeftTop).Contains(point))
                return Parts.LeftTop;
            if (CellPartsRealRect(Parts.TopRight).Contains(point))
                return Parts.TopRight;
            if (CellPartsRealRect(Parts.BottomRight).Contains(point))
                return Parts.BottomRight;
            if (CellPartsRealRect(Parts.LeftBottom).Contains(point))
                return Parts.LeftBottom;
            return Parts.Leave;
        }
    }
}