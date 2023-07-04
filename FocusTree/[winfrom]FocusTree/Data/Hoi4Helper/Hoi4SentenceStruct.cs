using static FocusTree.Data.Hoi4Helper.PublicSign;

namespace FocusTree.Data.Hoi4Helper
{
    /// <summary>
    /// 句子属性
    /// </summary>
    public class Hoi4SentenceStruct
    {
        /// <summary>
        /// 执行动作
        /// </summary>
        public Motions Motion = Motions.None;
        /// <summary>
        /// 值类型
        /// </summary>
        public Types ValueType = Types.None;
        /// <summary>
        /// 执行值
        /// </summary>
        public string Value = "";
        /// <summary>
        /// 触发者类型
        /// </summary>
        public Types TriggerType = Types.None;
        /// <summary>
        /// 动作触发者
        /// </summary>
        public string[] Triggers = Array.Empty<string>();

        public Hoi4SentenceStruct()
        {
        }
    }
}
