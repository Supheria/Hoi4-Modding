using FocusTree.Model.Lattice;
using FormatRawEffectSentence.Model;
using LocalUtilities.Interface;
using LocalUtilities.MathBundle;

namespace FocusTree.Model.Focus
{
    /// <summary>
    /// 国策节点控制类
    /// </summary>
    public class FocusNode(int signature) : RosterItem<int>(signature)
    {
        public FocusNode() : this(0)
        {

        }
        /// <summary>
        /// 国策名称
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 实施国策所需的天数
        /// </summary>
        public int Duration { get; set; } = 0;

        /// <summary>
        /// 国策描述
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// 备注
        /// </summary>
        public string Ps { get; set; } = "";

        /// <summary>
        /// 字段是否以 * 开头
        /// </summary>
        public bool BeginWithStar { get; set; } = false;

        /// <summary>
        /// 栅格化坐标
        /// </summary>
        public LatticedPoint LatticedPoint { get; set; } = new();

        /// <summary>
        /// 原始效果语句
        /// </summary>
        public List<string> RawEffects { get; set; } = new();

        /// <summary>
        /// 依赖组
        /// </summary>
        public List<HashSet<int>> Requires { get; set; } = new();

        /// <summary>
        /// 国策效果
        /// </summary>
        public List<EffectSentence> Effects { get; set; } = new();
    }
}
