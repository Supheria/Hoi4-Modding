using System.DirectoryServices.ActiveDirectory;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using FocusTree.IO;
using static FocusTree.Data.Hoi4Helper.PublicSign;

namespace FocusTree.Data.Hoi4Helper
{
    public class Hoi4Sentence
    {
        #region ==== 基本变量 ====

        /// <summary>
        /// 主句属性
        /// </summary>
        private readonly Hoi4SentenceStruct _main;
        /// <summary>
        /// 子句
        /// </summary>
        public List<Hoi4Sentence> SubSentences { get; private set; } = new();

        #endregion

        #region ==== 构造函数 ====

        /// <summary>
        /// 
        /// </summary>
        /// <param name="motion">执行动作，不可为空</param>
        /// <param name="valueType">值类型</param>
        /// <param name="value">执行值</param>
        /// <param name="triggerType">触发者类型</param>
        /// <param name="triggers">动作触发者</param>
        /// <param name="subSentences">子句</param>
        /// <returns></returns>
        public Hoi4Sentence(
            Motions motion,
            Types? valueType,
            string? value,
            Types? triggerType,
            string[]? triggers,
            List<Hoi4Sentence>? subSentences
        )
        {
            _main = new()
            {
                Motion = motion,
                ValueType = valueType ?? Types.None,
                Value = value ?? "",
                TriggerType = triggerType ?? Types.None,
                Triggers = triggers ?? Array.Empty<string>()
            };
            SubSentences = subSentences ?? new();
        }
        /// <summary>
        /// 用于序列化
        /// </summary>
        public Hoi4Sentence()
        {
            _main = new();
        }

        #endregion

        #region ==== 序列化方法 ====

        public void ReadXml(XmlReader reader)
        {
            SubSentences = new();

            //==== 读取主句属性 ====//
            _main.Motion = XmlHelper.GetEnumValue<Motions>(reader.GetAttribute("Motion")) as Motions? ?? Motions.None;
            var typePair = reader.GetAttribute("Type");
            var valuePair = reader.GetAttribute("Value");
            XmlHelper.ReadPair(typePair, ref _main.ValueType, ref _main.TriggerType,
                str => XmlHelper.GetEnumValue<Types>(str) as Types? ?? Types.None,
                str => XmlHelper.GetEnumValue<Types>(str) as Types? ?? Types.None);
            XmlHelper.ReadPair(valuePair, ref _main.Value, ref _main.Triggers, str => str, XmlHelper.ReadArrayString);

            XmlHelper.ReadCollection(reader, SubSentences, "Sentence", "Sentence", r =>
            {
                var sentence = new Hoi4Sentence();
                sentence.ReadXml(r);
                return sentence;
            });
        }
        public void WriteXml(XmlWriter writer)
        {
            //==== 序列化主句 ====//

            writer.WriteAttributeString("Motion", _main.Motion.ToString());
            writer.WriteAttributeString("Type",
                XmlHelper.WritePair(_main.ValueType.ToString(), _main.TriggerType.ToString()));
            writer.WriteAttributeString("Value",
                XmlHelper.WritePair(_main.Value, XmlHelper.WriteArrayString(_main.Triggers)));

            //==== 序列化子句 ====//

            foreach (var subSentence in SubSentences)
                subSentence.WriteXml(writer);
        }

        private string WriteTypePair()
        {
            return $"({_main.ValueType}),({_main.TriggerType})";
        }
        private string WriteValuePair()
        {
            return $"({_main.Value}),({XmlHelper.WriteArrayString(_main.Triggers)})";
        }

        #endregion

        #region ==== 拓展序列化方法 ====

        /// <summary>
        /// 转换为json字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder().AppendLine($"Motion=\"{_main.Motion}\", Type=\"{WriteTypePair()}\", Value=\"{WriteValuePair()}\"");
            foreach (var sub in SubSentences)
            {
                sb.AppendLine(sub.ToString(1));
            }
            return sb.ToString();
        }

        public string ToString(int tabTime)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < tabTime; i++)
                sb.Append('\t');
            sb.Append($"Motion=\"{_main.Motion}\", Type=\"{WriteTypePair()}\", Value=\"{WriteValuePair()}\"");
            foreach (var sub in SubSentences)
            {
                sb.Append($"\n{sub.ToString(tabTime + 1)}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 用json字符串生成
        /// </summary>
        /// <param name="jsonString"></param>
        public static Hoi4Sentence FromString(string jsonString)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
