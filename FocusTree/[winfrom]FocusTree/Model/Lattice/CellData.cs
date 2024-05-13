using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeBundle;

namespace FocusTree.Model.Lattice;

public class CellData : ISsSerializable
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

    public string LocalName { get; set; } = nameof(CellData);

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(EdgeLengthMin), EdgeLengthMin.ToString());
        serializer.WriteTag(nameof(EdgeLengthMax), EdgeLengthMax.ToString());
        serializer.WriteTag(nameof(EdgeLength), EdgeLength.ToString());
        serializer.WriteTag(nameof(NodePaddingFactorMin), NodePaddingFactorMin.ToString());
        serializer.WriteTag(nameof(NodePaddingFactorMax), NodePaddingFactorMax.ToString());
        serializer.WriteTag(nameof(NodePaddingWidthFactor), NodePaddingWidthFactor.ToString());
        serializer.WriteTag(nameof(NodePaddingHeightFactor), NodePaddingHeightFactor.ToString());
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        EdgeLengthMin = deserializer.ReadTag(nameof(EdgeLengthMin), s => s.ToInt(EdgeLengthMin));
        EdgeLengthMax = deserializer.ReadTag(nameof(EdgeLengthMax), s => s.ToInt(EdgeLengthMax));
        EdgeLength = deserializer.ReadTag(nameof(EdgeLength), s => s.ToInt(EdgeLength));
        NodePaddingFactorMin = deserializer.ReadTag(nameof(NodePaddingFactorMin), s => s.ToFloat(NodePaddingFactorMin));
        NodePaddingFactorMax = deserializer.ReadTag(nameof(NodePaddingFactorMax), s => s.ToFloat(NodePaddingFactorMax));
        NodePaddingWidthFactor = deserializer.ReadTag(nameof(NodePaddingWidthFactor), s => s.ToFloat(NodePaddingWidthFactor));
        NodePaddingHeightFactor = deserializer.ReadTag(nameof(NodePaddingHeightFactor), s => s.ToFloat(NodePaddingHeightFactor));
    }
}
