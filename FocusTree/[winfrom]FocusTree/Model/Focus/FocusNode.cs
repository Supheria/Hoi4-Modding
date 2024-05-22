#define test_format

using FormatRawEffectSentence;
using FormatRawEffectSentence.Model;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;

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
        public List<string> RawEffect { get; set; } = new();

        /// <summary>
        /// 依赖组
        /// </summary>
        public List<HashSet<int>> Require { get; set; } = new();

        /// <summary>
        /// 国策效果
        /// </summary>
        public List<EffectSentence> Effects { get; set; } = new();

        public override string LocalName { get; set; } = nameof(FocusNode);

        public override void Serialize(SsSerializer serializer)
        {
            serializer.WriteTag(nameof(Signature), Signature.ToString());
            serializer.WriteTag(nameof(Name), Name);
            serializer.WriteTag(nameof(BeginWithStar), BeginWithStar.ToString());
            serializer.WriteTag(nameof(Duration), Duration.ToString());
            serializer.WriteTag(nameof(Description), Description);
            serializer.WriteTag(nameof(Ps), Ps);
            serializer.WriteTag(nameof(LatticedPoint), LatticedPoint.ToString());
            serializer.WriteValues(nameof(RawEffect), RawEffect, s => s);
            //FormatRawEffects();
            serializer.WriteObjects(nameof(Effects), Effects);
            serializer.WriteValues(nameof(Require), Require, x => x.ToArrayString());
        }

        public override void Deserialize(SsDeserializer deserializer)
        {
            SetSignature = deserializer.ReadTag(nameof(Signature), int.Parse);
            Name = deserializer.ReadTag(nameof(Name), s => s);
            BeginWithStar = deserializer.ReadTag(nameof(BeginWithStar), bool.Parse);
            Duration = deserializer.ReadTag(nameof(Duration), int.Parse);
            Description = deserializer.ReadTag(nameof(Description), s => s);
            Ps = deserializer.ReadTag(nameof(Ps), s => s);
            LatticedPoint = deserializer.ReadTag(nameof(LatticedPoint), LatticedPoint.Parse);
            RawEffect = deserializer.ReadValues(nameof(RawEffect), s => s);
            Effects = deserializer.ReadObjects<EffectSentence>(nameof(Effects));
            Require = deserializer.ReadValues(nameof(Require), s => s.ToArray().Select(int.Parse).ToHashSet());
        }

        [Obsolete("临时使用，作为转换语句格式的过渡")]
        private void FormatRawEffects()
        {
#if test_format
            Effects.Clear();
            foreach (var raw in RawEffect)
            {
                Program.TestInfo.Total++;
                if (!RawEffectSentenceFormatter.Format(raw, out var formattedList))
                {
                    Program.TestInfo.Error++;
                    Program.TestInfo.Good = Program.TestInfo.Total - Program.TestInfo.Error;
                    Program.TestInfo.Append($"[{Signature}] {raw}");
                    continue;
                }
                foreach (var formatted in formattedList)
                {
                    Effects.Add(formatted);
                }
            }
#endif
        }
    }
}
