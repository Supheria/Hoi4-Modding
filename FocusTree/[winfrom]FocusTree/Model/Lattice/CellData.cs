namespace FocusTree.Model.Lattice;

public class CellData
{
    /// <summary>
    /// 格元边长（限制最小值和最大值）
    /// </summary>
    public int EdgeLength
    {
        get => _edgeLength;
        set => _edgeLength
            = value < EdgeLengthMin ? EdgeLengthMin
            : value > EdgeLengthMax ? EdgeLengthMax
            : value;
    }
    int _edgeLength = 30;

    /// <summary>
    /// 最小尺寸
    /// </summary>
    public int EdgeLengthMin { get; set; } = 25;
    /// <summary>
    /// 最大尺寸
    /// </summary>
    public int EdgeLengthMax { get; set; } = 125;
    /// <summary>
    /// 节点横向空隙系数
    /// </summary>
    public float NodePaddingWidthFactor
    {
        get => _nodePaddingWidthFactor;
        set => _nodePaddingWidthFactor
            = value < NodePaddingFactorMin ? NodePaddingFactorMin
            : value > NodePaddingFactorMax ? NodePaddingFactorMax
            : value;
    }
    float _nodePaddingWidthFactor = 0.333f;
    /// <summary>
    /// 节点纵向空隙系数
    /// </summary>
    public float NodePaddingHeightFactor
    {
        get => _nodePaddingHeightFactor;
        set => _nodePaddingHeightFactor
            = value < NodePaddingFactorMin ? NodePaddingFactorMin
            : value > NodePaddingFactorMax ? NodePaddingFactorMax
            : value;
    }
    float _nodePaddingHeightFactor = 0.333f;
    /// <summary>
    /// 节点空隙系数最小值
    /// </summary>
    public float NodePaddingFactorMin { get; set; } = 0.01f;
    /// <summary>
    /// 节点空隙系数最大值
    /// </summary>
    public float NodePaddingFactorMax { get; set; } = 0.4f;
}
