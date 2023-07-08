using System.Diagnostics.CodeAnalysis;

namespace FocusTree.Model.Lattice
{
    /// <summary>
    /// 栅格化坐标
    /// </summary>
    public class LatticedPoint
    {
        /// <summary>
        /// 所在栅格列数
        /// </summary>
        public int Col { get; }

        /// <summary>
        /// 所在栅格行数
        /// </summary>
        public int Row { get; }

        public LatticedPoint()
        {
            Col = 0;
            Row = 0;
        }
        public LatticedPoint(int col, int row)
        {
            Col = col;
            Row = row;
        }

        /// <summary>
        /// 使用真实坐标创建，将坐标转换为栅格化坐标
        /// </summary>
        public LatticedPoint(Point realPoint)
        {
            var widthDiff = realPoint.X - LatticeGrid.OriginLeft;
            var heightDiff = realPoint.Y - LatticeGrid.OriginTop;
            Col = widthDiff / LatticeCell.Length;
            Row = heightDiff / LatticeCell.Length;
            if (widthDiff < 0) { Col--; }
            if (heightDiff < 0) { Row--; }
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
