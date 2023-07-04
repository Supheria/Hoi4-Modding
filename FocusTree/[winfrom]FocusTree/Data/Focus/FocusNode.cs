#define FORMAT_TEST
#define RAW_EFFECTS
using FocusTree.Data.Hoi4Helper;
using System.Xml;
using FocusTree.IO;

namespace FocusTree.Data.Focus
{
    /// <summary>
    /// 国策节点控制类
    /// </summary>
    public class FocusNode
    {
        #region ==== 国策信息 ====

        public FocusData FData;
        /// <summary>
        /// 国策效果
        /// </summary>
        public List<string> Effects
        {
            get => _effects.Select(x => x.ToString()).ToList();
            set => value.ForEach(x => _effects.Add(Hoi4Sentence.FromString(x)));
        }
        private readonly List<Hoi4Sentence> _effects = new();


        #endregion

        public FocusNode(FocusData fData)
        {
            FData = fData;
        }

        #region ==== 序列化方法 ====

        /// <summary>
        /// 用于序列化
        /// </summary>
        public FocusNode()
        {
            FData = new();
        }
        public void ReadXml(XmlReader reader, string nodeName)
        {
            Effects = new();
            FData = new()
            {
                Id = int.Parse(reader.GetAttribute("ID") ?? throw new ArgumentException()),
                Name = reader.GetAttribute("Name") ?? FData.Name,
                BeginWithStar = bool.Parse(reader.GetAttribute("Star") ?? "false"),
                Duration = int.Parse(reader.GetAttribute("Duration") ?? "0"),
                Description = reader.GetAttribute("Description") ?? FData.Description,
                Ps = reader.GetAttribute("Ps.") ?? FData.Ps,
            };
            var pair = XmlHelper.ReadArrayString(reader.GetAttribute("Point"));
            if (pair is not { Length: 2 })
                FData.LatticedPoint = new(0, 0);
            else
                FData.LatticedPoint = new(int.Parse(pair[0]), int.Parse(pair[1]));

            while (reader.Read())
            {
                if (reader.Name == nodeName && reader.NodeType is XmlNodeType.EndElement) 
                    break;
                switch (reader.Name)
                {
#if FORMAT_TEST
                    //==== 读取 Effects ====//
                    case "Effects":
                        XmlHelper.ReadCollection(reader, _effects, "Effects", "Sentence", r =>
                        {
                            Hoi4Sentence sentence = new();
                            sentence.ReadXml(r);
                            return sentence;
                        });
                        continue;
#endif
                    //==== 读取 Requires ====//
                    case "Requires":
                        XmlHelper.ReadCollection(reader, FData.Requires, "Requires", "Require", r =>
                        {
                            r.Read();
                            return XmlHelper.ReadArrayString(r.Value).Select(int.Parse).ToHashSet();
                        });
                        continue;
                    //==== 读取 RawEffects ====//
                    case "RawEffects":
                        XmlHelper.ReadCollection(reader, FData.RawEffects, "RawEffects", "Effect", r =>
                        {
                            r.Read();
                            return r.Value;
                        });
                        continue;
                }
            }
#if FORMAT_TEST
            FormatRawEffects(FData.RawEffects, FData.Id);
#endif
        }
        [Obsolete("临时使用，作为转换语句格式的过渡")]
        private void FormatRawEffects(List<string> rawEffects, int id)
        {
            foreach (var raw in rawEffects)
            {
                Program.TestInfo.Total++;
                if (!FormatRawEffectSentence.Formatter(raw, out var formattedList))
                {
#if RAW_EFFECTS
                    Program.TestInfo.Error++;
                    Program.TestInfo.Good = Program.TestInfo.Total - Program.TestInfo.Error;
                    Program.TestInfo.Append($"[{id}] {raw}");
#endif
                    continue;
                }
                foreach (var formatted in formattedList)
                {
                    _effects.Add(formatted);
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("ID", FData.Id.ToString());
            writer.WriteAttributeString("Name", FData.Name);
            writer.WriteAttributeString("Star", FData.BeginWithStar.ToString());
            writer.WriteAttributeString("Duration", FData.Duration.ToString());
            writer.WriteAttributeString("Description", FData.Description);
            writer.WriteAttributeString("Ps.", FData.Ps);
            var point = FData.LatticedPoint;
            writer.WriteAttributeString("Point", XmlHelper.WriteArrayString(new[] { point.Col.ToString(), point.Row.ToString() }));
#if RAW_EFFECTS
            XmlHelper.WriteCollection(writer, FData.RawEffects, "RawEffects", "Effect",
                (w, str) => w.WriteValue(str));
#endif
#if FORMAT_TEST
            FormatRawEffects(FData.RawEffects, FData.Id);
            XmlHelper.WriteCollection(writer, _effects, "Effects", "Sentence", (w, sentence) => sentence.WriteXml(w));
#endif
            XmlHelper.WriteCollection(writer, FData.Requires, "Requires", "Require", (w, require) =>
            {
                if (require.ToArray().Length > 0)
                    w.WriteValue(XmlHelper.WriteArrayString(require.Select(x => x.ToString()).ToArray()));
            });
        }

        #endregion
    }
}
