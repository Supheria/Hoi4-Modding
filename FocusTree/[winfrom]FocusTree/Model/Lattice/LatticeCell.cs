using System.Xml;

namespace FocusTree.Model.Lattice
{
    /// <summary>
    /// 格元
    /// </summary>
    public class LatticeCell
    {
        #region ==== 设置宽高和间距 ====

        /// <summary>
        /// 格元边长（限制最小值和最大值）
        /// </summary>
        public static int Length
        {
            get => _sideLength;
            set => _sideLength = value < LengthMin ? LengthMin : value > LengthMax ? LengthMax : value;
        }

        private static int _sideLength = 30;
        /// <summary>
        /// 最小尺寸
        /// </summary>
        public static int LengthMin { get; set; } = 25;
        /// <summary>
        /// 最大尺寸
        /// </summary>
        public static int LengthMax { get; set; } = 125;

        public Size NodePadding => new(
            (int)(Length * NodePaddingZoomFactor.X), 
            (int)(Length * NodePaddingZoomFactor.Y));
        /// <summary>
        /// 节点空隙系数（0.3 < X < 0.7, 0.3 < Y < 0.7)
        /// </summary>
        public static PointF NodePaddingZoomFactor
        {
            get => _nodePaddingZoomFactor;
            set => _nodePaddingZoomFactor = new(value.X < 0.1f ? 0.1f : value.X > 0.7f ? 0.7f : value.X, value.Y < 0.1f ? 0.1f : value.Y > 0.7f ? 0.7f : value.Y);
        }

        private static PointF _nodePaddingZoomFactor = new(0.333f, 0.333f);

        #endregion

        #region ==== 坐标 ====

        /// <summary>
        /// 格元栅格化坐标
        /// </summary>
        public LatticedPoint LatticedPoint { get; set; } = new();

        public Rectangle CellRealRect => new(
            Length * LatticedPoint.Col + LatticeGrid.OriginX,
            Length * LatticedPoint.Row + LatticeGrid.OriginY, 
            Length, Length
            );
        /// <summary>
        /// 节点真实坐标矩形
        /// </summary>
        public Rectangle NodeRealRect
        {
            get
            {
                var cellRect = CellRealRect;
                var nodePadding = NodePadding;
                return new(
                    cellRect.Left + nodePadding.Width, cellRect.Top + nodePadding.Height,
                    Length - nodePadding.Width * 2, Length - nodePadding.Height * 2);
            }
        }

        #endregion

        #region ==== 构造函数 ====

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LatticeCell()
        {
        }

        /// <summary>
        /// 使用已有的栅格化坐标创建
        /// </summary>
        public LatticeCell(LatticedPoint point)
        {
            LatticedPoint = point;
        }

        #endregion

        #region ==== 格元区域 ====

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
        public Dictionary<Parts, Rectangle> CellPartsRealRect
        {
            get
            {
                var cellRect = CellRealRect;
                var nodePadding = NodePadding;
                var nodeRect = NodeRealRect;
                return new()
                {
                    [Parts.Leave] = Rectangle.Empty,
                    [Parts.Left] = new(cellRect.Left, nodeRect.Top, nodePadding.Width, nodeRect.Height),
                    [Parts.Top] = new(nodeRect.Left, cellRect.Top, nodeRect.Width, nodePadding.Height),
                    [Parts.Right] = new(nodeRect.Right, nodeRect.Top, nodePadding.Width, nodeRect.Height),
                    [Parts.Bottom] = new(nodeRect.Left, nodeRect.Bottom, nodeRect.Width, nodePadding.Height),
                    [Parts.LeftTop] = new(cellRect.Left, cellRect.Top, nodePadding.Width, nodePadding.Height),
                    [Parts.TopRight] = new(nodeRect.Right, cellRect.Top, nodePadding.Width, nodePadding.Height),
                    [Parts.LeftBottom] = new(cellRect.Left, nodeRect.Bottom, nodePadding.Width, nodePadding.Height),
                    [Parts.BottomRight] = new(nodeRect.Right, nodeRect.Bottom, nodePadding.Width, nodePadding.Height),
                    [Parts.Node] = nodeRect
                };
            }
        }
            

        /// <summary>
        /// 获取坐标在格元上所处的部分
        /// </summary>
        /// <param name="point">坐标</param>
        /// <returns></returns>
        public Parts GetPartPointOn(Point point)
        {
            foreach (var (key, rect) in CellPartsRealRect)
            {
                if (rect.Contains(point))
                {
                    return key;
                }
            }
            return Parts.Leave;
        }

        #endregion
    }
}