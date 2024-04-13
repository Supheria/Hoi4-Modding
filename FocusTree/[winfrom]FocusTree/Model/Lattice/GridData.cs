using LocalUtilities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTree.Model.Lattice;

public class GridData
{
    public float FloatComparisonTolerance { get; set; } = 0.1f;
    /// <summary>
    /// 栅格坐标系原点X坐标
    /// </summary>
    public int OriginX { get; set; }
    /// <summary>
    /// 栅格坐标系原点Y坐标
    /// </summary>
    public int OriginY { get; set; }
    /// <summary>
    /// 栅格绘图区域矩形
    /// </summary>
    public Rectangle DrawRect { get; set; }
    /// <summary>
    /// 格元边框绘制用笔
    /// </summary>
    public Pen CellPen { get; } = new(Color.FromArgb(200, Color.AliceBlue), 1.5f);
    /// <summary>
    /// 节点边框绘制用笔（直线）
    /// </summary>
    public Pen NodePenLine { get; } = new(Color.FromArgb(150, Color.Orange), 1.75f);
    /// <summary>
    /// 节点边框绘制用笔（虚线）
    /// </summary>
    public Pen NodePenDash { get; } = new(Color.FromArgb(150, Color.Orange), 2f)
    {
        DashStyle = System.Drawing.Drawing2D.DashStyle.Custom,
        DashPattern = new float[] { 1.25f, 1.25f },
    };
    /// <summary>
    /// 坐标辅助线绘制用笔
    /// </summary>
    public Pen GuidePen { get; } = new(Color.FromArgb(200, Color.Red), 1.75f);
}
