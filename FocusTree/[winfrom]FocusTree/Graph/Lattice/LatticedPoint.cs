using System.Diagnostics.CodeAnalysis;

namespace FocusTree.Graph.Lattice
{
    /// <summary>
    /// 栅格化坐标
    /// </summary>
    public class LatticedPoint
    {
        /// <summary>
        /// 所在栅格列数
        /// </summary>
        private int _colNumber;
        /// <summary>
        /// 所在栅格行数
        /// </summary>
        private int _rowNumber;

        /// <summary>
        /// 所在栅格列数
        /// </summary>
        public int Col
        {
            get => _colNumber;
            set => _colNumber = value;
        }

        /// <summary>
        /// 所在栅格行数
        /// </summary>
        public int Row
        {
            get => _rowNumber;
            set => _rowNumber = value;
        }
        public LatticedPoint()
        {
            _colNumber = 0;
            _rowNumber = 0;
        }
        public LatticedPoint(int col, int row)
        {
            _colNumber = col;
            _rowNumber = row;
        }

        /// <summary>
        /// 使用真实坐标创建，将坐标转换为栅格化坐标
        /// </summary>
        public LatticedPoint(Point realPoint)
        {
            var widthDiff = realPoint.X - LatticeGrid.OriginLeft;
            var heightDiff = realPoint.Y - LatticeGrid.OriginTop;
            _colNumber = widthDiff / LatticeCell.Length;
            _rowNumber = heightDiff / LatticeCell.Length;
            if (widthDiff < 0) { _colNumber--; }
            if (heightDiff < 0) { _rowNumber--; }
        }
        /// <summary>
        /// 行列数是否都相等
        /// </summary>
        /// <param name="lhd"></param>
        /// <param name="rhd"></param>
        /// <returns></returns>
        public static bool operator ==(LatticedPoint lhd, LatticedPoint rhd) => lhd.Col == rhd.Col && lhd.Row == rhd.Row;
        /// <summary>
        /// 行列数是否有任一不相等
        /// </summary>
        /// <param name="lhd"></param>
        /// <param name="rhd"></param>
        /// <returns></returns>
        public static bool operator !=(LatticedPoint lhd, LatticedPoint rhd) => lhd.Col != rhd.Col || lhd.Row != rhd.Row;
        /// <summary>
        /// 尚未重写 Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override bool Equals([NotNullWhen(true)] object? obj) => obj is LatticedPoint point && Equals(point);
        public bool Equals(LatticedPoint other) => this == other;
        /// <summary>
        /// 已重写 GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => HashCode.Combine(Col, Row);
    }
}
