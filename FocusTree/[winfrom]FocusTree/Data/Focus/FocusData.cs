using System.Xml.Serialization;
using FocusTree.Graph.Lattice;

namespace FocusTree.Data.Focus
{
    /// <summary>
    /// 国策数据
    /// </summary>
    public class FocusData
    {
        /// <summary>
        /// 栅格化坐标
        /// </summary>
        public LatticedPoint LatticedPoint = new();
        /// <summary>
        /// 节点ID
        /// </summary>
        public int Id = 0;
        /// <summary>
        /// 国策名称
        /// </summary>
        public string Name = "";
        /// <summary>
        /// 实施国策所需的天数
        /// </summary>
        public int Duration = 0;
        /// <summary>
        /// 国策描述
        /// </summary>
        public string Description = "";
        /// <summary>
        /// 备注
        /// </summary>
        public string Ps = "";
        /// <summary>
        /// 字段是否以 * 开头
        /// </summary>
        public bool BeginWithStar = false;
        /// <summary>
        /// 原始效果语句
        /// </summary>
        public List<string> RawEffects = new();
        /// <summary>
        /// 依赖组
        /// </summary>
        public List<HashSet<int>> Requires = new();

        public FocusData()
        {
        }
    }
}
