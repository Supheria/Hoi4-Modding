using FormatRawEffectSentence.Data;
using FormatRawEffectSentence.LocalSign;
using FormatRawEffectSentence.Model;
using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.RegexUtilities;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace FormatRawEffectSentence
{
    /// <summary>
    /// 格式化原始效果语句
    /// </summary>
    public class RawEffectSentenceFormatter
    {
        private const string XmlFilePath = "raw effect format match patterns.xml";

        private static string _rawSentence = "";

        private static RawPattern[] _patterns = Array.Empty<RawPattern>();

        private record MotionCollect(Motions Motion, (Types Type, string Value) Value);

        public static bool Format(string sentence, out List<EffectSentence> formattedList)
        {
            _rawSentence = sentence.Trim();
            _patterns = RawPatternArrayUtilities.LoadRawPatternArray(XmlFilePath);

            RawPatternArrayUtilities.SaveRawPatternArray(XmlFilePath, ref _patterns);
            return TestSinglePatterns(out formattedList);
            //return SinglePatternFormatter(sentence, out formattedList) ||
            //       ComplexPatternFormatter(sentence, out formattedList);
        }

        #region ==== 单语句 ====

        private static bool TestSinglePatterns(out List<EffectSentence> formattedList)
        {
            formattedList = new();
            foreach (var pattern in _patterns)
            {
                if (TestPatternForMatch(pattern, out formattedList))
                    return true;
            }
            return false;
        }
        private static bool TestPatternForMatch(RawPattern pattern, out List<EffectSentence> formattedList)
        {
            formattedList = new();

            static Motions Motion(RawPattern p, IReadOnlyList<string> parts)
            {
                var key = (uint)p.Motion.PartIndex < parts.Count ? parts[p.Motion.PartIndex] : "";
                return p.Motion.ConditionMap.TryGetValue(key, out var value) ? value : p.Motion.ConditionMap[""];
            }

            static (Types Type, string Value) Value(RawPattern p, IReadOnlyList<string> parts)
            {
                if (p.Value.Type is Types.None)
                    return (p.Value.Type, "");
                var sb = new StringBuilder();
                foreach (var index in p.Value.PartIndexOrder.Where(index => (uint)index < parts.Count))
                    sb.Append($"{parts[index]}{GeneralMark.ElementSplitter}");

                var str = sb.ToString();
                if (str.EndsWith(GeneralMark.ElementSplitter) && str.Length > 0)
                    str = str[..^1];
                return (p.Value.Type, str);
            }

            if (!MatchSinglePatternEffectSentence(pattern.Trigger, pattern.Motion.Pattern,
                    parts => new(Motion(pattern, parts), Value(pattern, parts)), out var sentence))
                return false;
            formattedList = new() { sentence };
            return true;
        }

        private static bool MatchSinglePatternEffectSentence(MotionTrigger trigger,
            string motionPattern, Func<string[], MotionCollect> getMotionCollect,
            [NotNullWhen(true)] out EffectSentence? sentence)
        {
            sentence = null;
            var pattern = $"^({trigger.Pattern})*{motionPattern}$";
            if (!RegexMatchTool.GetMatch(_rawSentence, pattern, out _))
                return false;
            var triggers = new List<string>();
            Match? match;
            if (trigger.Type is not Types.None && trigger.Pattern is not "")
                while (RegexMatchTool.GetMatch(_rawSentence, $"^{trigger.Pattern}(.+)$", out match))
                {
                    triggers.Add(match.Groups[1].Value);
                    _rawSentence = match.Groups[2].Value;
                }

            if (!RegexMatchTool.GetMatch(_rawSentence, motionPattern, out match))
                return false;
            var parts = new List<string>();
            for (var i = 0; i < match.Groups.Count; i++)
            {
                var group = match.Groups[i];
                if (group.Name is not "0")
                    parts.Add(group.Value);
            }

            var motionCollect = getMotionCollect(parts.ToArray());
            if (motionCollect.Motion is Motions.None)
                return false;
            sentence = new(motionCollect.Motion, motionCollect.Value.Type, motionCollect.Value.Value, trigger.Type, triggers.ToArray());
            return true;
        }

        /// <summary>
        /// 格式化单句
        /// </summary>
        /// <param name="rawSentence"></param>
        /// <param name="formattedList">格式化后的语句</param>
        /// <returns>返回格式化后的单语句，如果无匹配的格式化模式则返回null</returns>
        private static bool SinglePatternFormatter(string rawSentence, out List<EffectSentence> formattedList)
        {
            formattedList = new();

            //// 单语句 - （某国）触发事件
            //// 触发事件“骑着青牛的老者？”。
            //// 触发事件：“借地？”
            //// （妖精乐园）触发事件：“解释”
            //// 错误
            //// 神灵庙触发事件“道教该如何面对道家？”
            //if (MatchSinglePatternEffectSentence(rawSentence, new(Types.State, "（([^触）]+)）"),
            //        "触发事件：?“([\u4e00-\u9fa5？《》]+)”。?",
            //        parts => new(Motions.Trigger, (Types.Event, parts[0])),
            //        out var sentence))
            //    formattedList = new() { sentence };
            //// 单语句 - 固定值或类型
            //// 将平均灵力值固定为50%
            //// 将世界观固定为唯心世界观
            //else if (MatchSinglePatternEffectSentence(rawSentence, null, "将(.+)固定为([\u4e00-\u9fa5\\d%]+)",
            //             parts => new(Motions.Lock,
            //                 (Types.Variable, parts[0] + PublicSplitter + parts[1])),
            //             out sentence))
            //    formattedList = new() { sentence };
            //// 单语句-移除固定
            //// 移除对势力规模的固定
            //else if (MatchSinglePatternEffectSentence(rawSentence, null, "移除对(.+)的固定",
            //             parts => new(Motions.Unlock, (Types.Variable, parts[0])),
            //             out sentence))
            //    formattedList = new() { sentence };
            //// 单语句-加|减值
            //// 灵力系科研速度：+35%
            //// 稳定度：-30%
            //// 每日获得的政治点数：+0.1
            //// （人类村落）政治点数：+300
            //else if (MatchSinglePatternEffectSentence(rawSentence, new(Types.State, "（([^（]+)）"),
            //             "([^：]+)：?([+-])([\\d.]+%?)",
            //             parts => new(parts[1] is "+" ? Motions.Add : Motions.Sub,
            //                 (Types.Variable, $"{parts[0]}{PublicSplitter}{parts[2]}")), out sentence))
            //    formattedList = new() { sentence };
            //// 单语句-修正值
            //// 适役人口修正：15%
            //else if (MatchSinglePatternEffectSentence(rawSentence, null, "([^修]+)修正：?([\\d.]+%?)", parts => new(Motions.Modify, (Types.Variable, parts[0] + PublicSplitter + parts[1])), out sentence))
            //    formattedList = new() { sentence };
            //// 单语句-获得|增加|移除个数
            ///// 获得1个科研槽
            ///// 增加10个建筑位
            ///// 移除1个民用工厂
            //else if (GetMatch(str, "^(获得|增加|添加|移除)(\\d+)个([\u4e00-\u9fa5]+)$", out match))
            //{
            //    var motion = (match.Groups[1].Value == "移除" ? Motions.Sub : Motions.Add);
            //    var valueType = Types.Variable;
            //    var value = match.Groups[3].Value + PublicSplitter + match.Groups[2].Value;
            //    formattedList = new() { new(motion, valueType, value, null, null, null) };
            //}
            //// 单语句-某国值升高|降低点数
            ///// （人类村落）厌战降低50点
            //else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）([\u4e00-\u9fa5]+)(升高|降低)([\\d.]+%?)点$", out match))
            //{
            //    var motion = (match.Groups[3].Value == "降低" ? Motions.Sub : Motions.Add);
            //    var valueType = Types.Variable;
            //    var value = match.Groups[2].Value + PublicSplitter + match.Groups[4].Value;
            //    var triggerType = Types.State;
            //    var trigger = new[] { match.Groups[1].Value };
            //    formattedList = new() { new(motion, valueType, value, triggerType, trigger, null) };
            //}
            //// 单语句-宣战可用性
            ///// 规则修改：不能宣战
            ///// 规则修改：可以宣战
            ///// 不能宣战
            //else if (GetMatch(str, "^(?:规则修改：)?(不能|可以)宣战$", out match))
            //{
            //    var motion = (match.Groups[1].Value == "可以" ? Motions.Gain : Motions.Remove);
            //    var valueType = Types.AbleToDeclareWar;
            //    formattedList = new() { new(motion, valueType, null, null, null, null) };
            //}
            //// 单语句-可以开启某项决议
            ///// 可以通过决议发动周边国家的内战使其变为附庸
            //else if (GetMatch(str, "^可以通过决议([\u4e00-\u9fa5]+)$", out match))
            //{
            //    var motion = Motions.Gain;
            //    var valueType = Types.AbleToStartResolution;
            //    var value = match.Groups[1].Value;
            //    formattedList = new() { new(motion, valueType, value, null, null, null) };
            //}
            //// 单语句-开启某项决议
            ///// 可以通过决议发动周边国家的内战使其变为附庸
            //else if (GetMatch(str, "^开启([\u4e00-\u9fa5]+)决议$", out match))
            //{
            //    var motion = Motions.Start;
            //    var valueType = Types.Resolution;
            //    var value = match.Groups[1].Value;
            //    formattedList = new() { new(motion, valueType, value, null, null, null) };
            //}
            //// 单语句-自动获得核心可用性
            ///// 幽灵种族的省份将自动获得核心：是
            //else if (GetMatch(str, "^([\u4e00-\u9fa5]+的省份)将自动获得核心：(是|否)$", out match))
            //{
            //    var motion = (match.Groups[2].Value == "是" ? Motions.Gain : Motions.Remove);
            //    var valueType = Types.AbleToGainCoreAuto;
            //    var triggerType = Types.Province;
            //    var trigger = new string[] { match.Groups[1].Value };
            //    formattedList = new() { new(motion, valueType, null, triggerType, trigger, null) };
            //}
            //// 单语句-某国的区域获得核心
            ///// 山童镇，山童实验场，水坝控制站，河童大坝，蒸汽村落，南玄武川城，中玄武川镇，三平实验室：隐居村落获得该地区的核心。
            //else if (GetMatch(str, "^([\u4e00-\u9fa5]+)((，[\u4e00-\u9fa5]+)+：[\u4e00-\u9fa5]+获得该地区的核心。$)", out match))
            //{
            //    List<string> regions = new();
            //    do
            //    {
            //        str = match.Groups[2].Value;
            //        regions.Add(match.Groups[1].Value);
            //    } while (GetMatch(str, "^，([\u4e00-\u9fa5]+)(.+)$", out match));
            //    GetMatch(str, "^：([\u4e00-\u9fa5]+)获得该地区的核心。$", out match);
            //    var motion = Motions.Gain;
            //    var valueType = Types.RegionCore;
            //    var triggerType = Types.State;
            //    var t = match.Groups[1].Value;
            //    foreach (var region in regions)
            //    {
            //        t += PublicSplitter + region;
            //    }
            //    var trigger = new string[] { t }; // eg.隐居村落|山童镇|山童实验场|...
            //    formattedList = new() { new(motion, valueType, null, triggerType, trigger, null) };
            //}
            //// 单语句-数值修正
            ///// （天狗共和国）我国对其进攻修正：+20%
            //else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）我国(对其[\u4e00-\u9fa5]+)修正：([+-])(\\d+%?)$", out match))
            //{
            //    var motion = (match.Groups[3].Value == "+" ? Motions.Add : Motions.Sub);
            //    var valueType = Types.Variable;
            //    var value = match.Groups[2].Value + PublicSplitter + match.Groups[1].Value + PublicSplitter + match.Groups[4].Value; // eg.对其进攻|天狗共和国|20%
            //    formattedList = new() { new(motion, valueType, value, null, null, null) };
            //}
            //// 单语句-获得对他国的战争目标
            ///// 获得对守矢神社的吞并战争目标
            //else if (GetMatch(str, "^获得对([\u4e00-\u9fa5]+)的([\u4e00-\u9fa5]+)战争目标", out match))
            //{
            //    var motion = Motions.Gain;
            //    var valueType = Types.WarGoal;
            //    var value = match.Groups[2].Value + PublicSplitter + match.Groups[1].Value; // eg.吞并|守矢神社
            //    formattedList = new() { new(motion, valueType, value, null, null, null) };
            //}
            //// 单语句-研究加成
            ///// 3x60%研究加成：基础武器。
            //else if (GetMatch(str, "^(\\dx\\d+%?)研究加成：([\u4e00-\u9fa5]+)。?$", out match))
            //{
            //    var motion = Motions.Bonus;
            //    var valueType = Types.Research;
            //    var value = match.Groups[2].Value + PublicSplitter + match.Groups[1].Value; // eg.基础武器|3x60%
            //    formattedList = new() { new(motion, valueType, value, null, null, null) };
            //}
            //// 单语句-修改规则：可以创建阵营
            ///// 可以创建阵营
            ///// 获得允许创建阵营
            //else if (GetMatch(str, "^可以创建阵营|获得允许创建阵营$", out match))
            //{
            //    var motion = Motions.Gain;
            //    var valueType = Types.AbleToCreateCamp;
            //    formattedList = new() { new(motion, valueType, null, null, null, null) };
            //}
            //// 单语句-暂时无影响
            ///// 这项国策目前没有实际影响。但随着世界局势的变化可能会发生改变。
            //else if (GetMatch(str, "这项国策目前没有实际影响。但随着世界局势的变化可能会发生改变。", out match))
            //{
            //    var motion = Motions.NoneButMayChange;
            //    formattedList = new() { new(motion, null, null, null, null, null) };
            //}
            //// 单语句-创建阵营
            ///// 获得允许创建阵营，创建阵营：道盟
            //else if (GetMatch(str, "^获得允许创建阵营，创建阵营：([\u4e00-\u9fa5]+)。?$", out match))
            //{
            //    var motion = Motions.Create;
            //    var valueType = Types.Camp;
            //    var value = match.Groups[1].Value;
            //    formattedList = new() { new(motion, valueType, value, null, null, null) };
            //}
            //// 单语句-某国创建指定属性的阵营
            ///// （守矢神社）获得允许创建防御性阵营，创建阵营：妖怪山自卫联盟。
            //else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）获得允许创建([\u4e00-\u9fa5]+)阵营，创建阵营：([\u4e00-\u9fa5]+)。?$", out match))
            //{
            //    var motion = Motions.Create;
            //    var valueType = Types.Camp;
            //    var value = match.Groups[3].Value + PublicSplitter + match.Groups[2].Value; // eg.妖怪山自卫联盟|防御性
            //    var triggerType = Types.State;
            //    var trigger = new string[] { match.Groups[1].Value };
            //    formattedList = new() { new(motion, valueType, value, triggerType, trigger, null) };
            //}
            //// 单语句-创建阵营
            ///// 创建阵营：第二次人类复权运动——北线
            //else if (GetMatch(str, "^创建阵营：([\u4e00-\u9fa5—]+)。?$", out match))
            //{
            //    var motion = Motions.Create;
            //    var valueType = Types.Camp;
            //    var value = match.Groups[1].Value;
            //    formattedList = new() { new(motion, valueType, value, null, null, null) };
            //}
            //// 单语句-不允许加入阵营
            ///// x可以加入阵营
            //else if (GetMatch(str, "^x可以加入阵营$", out match))
            //{
            //    var motion = Motions.Remove;
            //    var valueType = Types.AbleToJoinCamp;
            //    formattedList = new() { new(motion, valueType, null, null, null, null) };
            //}
            //// 单语句-某国加入阵营
            ///// 神灵庙加入阵营
            ///// 隐居村落加入妖怪山自卫联盟
            ///// （妖精乐园）加入第二次人类复权运动——北线
            //else if (GetMatch(str, "^（?([\u4e00-\u9fa5]+)）?加入([\u4e00-\u9fa5—]+)$", out match))
            //{
            //    var motion = Motions.Join;
            //    var valueType = Types.Camp;
            //    var value = match.Groups[2].Value;
            //    var triggerType = Types.State;
            //    var trigger = new string[] { match.Groups[1].Value };
            //    formattedList = new() { new(motion, valueType, value, triggerType, trigger, null) };
            //}
            //// 某国吞并他国
            ///// 隐居村落吞并河童长老会
            //else if (GetMatch(str, "^([\u4e00-\u9fa5]+)吞并([\u4e00-\u9fa5]+)$", out match))
            //{
            //    var motion = Motions.Annexed;
            //    var valueType = Types.State;
            //    var value = match.Groups[2].Value;
            //    var triggerType = Types.State;
            //    var trigger = new string[] { match.Groups[1].Value };
            //    formattedList = new() { new(motion, valueType, value, triggerType, trigger, null) };
            //}
            //// 单语句-获得|移除标签
            ///// 获得五弊三缺
            ///// 移除道教的探索精神
            ///// 失去“？”
            //else if (GetMatch(str, "^(获得|移除|失去)“?([\u4e00-\u9fa5？]+)”?$", out match))
            //{
            //    var motion = (match.Groups[1].Value == "获得" ? Motions.Gain : Motions.Remove);
            //    var valueType = Types.Label;
            //    var value = match.Groups[2].Value;
            //    formattedList = new() { new(motion, valueType, value, null, null, null) };
            //}
            //// 单语句-某国获得|移除标签
            ///// （人类村落）移除道具的保护
            //else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）(获得|移除|失去)“?([\u4e00-\u9fa5？]+)”?$", out match))
            //{
            //    var motion = (match.Groups[2].Value == "获得" ? Motions.Gain : Motions.Remove);
            //    var valueType = Types.Label;
            //    var value = match.Groups[3].Value;
            //    var triggerType = Types.State;
            //    var trigger = new[] { match.Groups[1].Value };
            //    formattedList = new() { new(motion, valueType, value, triggerType, trigger, null) };
            //}
            //// 单语句-某国获得标签
            /////（神灵庙）获得命莲寺的宣称
            //else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）获得([\u4e00-\u9fa5]+)$", out match))
            //{
            //    var motion = Motions.Gain;
            //    var valueType = Types.Label;
            //    var value = match.Groups[2].Value;
            //    var triggerType = Types.State;
            //    var trigger = new[] { match.Groups[1].Value };
            //    formattedList = new() { new(motion, valueType, value, triggerType, trigger, null) };
            //}
            //// 单语句-增加部队
            ///// 将会出现6个编制为神灵庙护卫编制的部队
            //else if (GetMatch(str, "^将会出现(\\d+)个编制为([\u4e00-\u9fa5]+)编制的部队$", out match))
            //{
            //    var motion = Motions.Add;
            //    var valueType = Types.Troop;
            //    var value = match.Groups[2].Value + PublicSplitter + match.Groups[1].Value; // eg.神灵庙护卫|6
            //    formattedList = new() { new(motion, valueType, value, null, null, null) };
            //}
            //else
            {
                return false;
            }
            return true;
        }

        #endregion

        #region ==== 复语句 ====

        /// <summary>
        /// 格式化复语句
        /// </summary>
        /// <param name="str">原始语句</param>
        /// <param name="formattedList">格式化后的语句，默认值为 null</param>
        /// <returns>格式化成功返回true，否则返回false。若子语句中有一个或以上的短句无法格式化，同样判定为格式化失败返回false</returns>
        private static bool ComplexPatternFormatter(string rawSentence, out List<EffectSentence> formattedList)
        {
            formattedList = new();

            //// 复语句-获得理念类标签
            ///// 获得世界线的观察见证者，其效果为（理念类，每日获得的政治点数：-0.05，理念类花费：+10%，稳定度：+35%，建造速度：+25%，部队核心领土攻击：+30%，部队核心领土防御：+30%，ai修正：专注防御：+30%，防御战争对稳定度修正：+20%，孤立倾向：+0.01，路上要塞建造速度：+25%，防空火炮建造速度：+25%，雷达站建造速度：+25%）
            //if (GetMatch(str, "^(获得|移除)([\u4e00-\u9fa5\\d/]+)，其效果为（(理念类)，(.+)）$", out var match))
            //{
            //    if (!GetSubSentence(match.Groups[4].Value, "，", out var subSentences))
            //    {
            //        return false;
            //    }
            //    var motion = (match.Groups[1].Value == "获得" ? Motions.Gain : Motions.Remove);
            //    var valueType = Types.Label;
            //    var value = match.Groups[3].Value + PublicSplitter + match.Groups[2].Value; // eg.理念类|世界线的观察见证者
            //    formattedList = new() { new(motion, valueType, value, null, null, subSentences) };
            //}
            //// 复语句-获得限时标签
            ///// 获得为期365天的“强大的威望”，其效果为（每日获得的政治点数：+0.25，部队组织度：+15%，稳定度：+20%，战争支持度：+20%，建造速度：+10%，部队攻击：+5%，部队防御：+5%，科研速度：+10%，工厂产出：+10%）
            //else if (GetMatch(str, "^获得(?:为期)?(\\d+[天月年])的“([\u4e00-\u9fa5]+)”，其效果为（(.+)）$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[3].Value, "，", out var subSentences))
            //    {
            //        return false;
            //    }
            //    var motion = Motions.Gain;
            //    var valueType = Types.Label;
            //    var value = match.Groups[2].Value + PublicSplitter + match.Groups[1].Value; // eg.强大的威望|365天
            //    formattedList = new() { new(motion, valueType, value, null, null, subSentences) };
            //}
            //// 复语句-某国获得限时标签
            ///// 所有敌国获得为期365天的“必有凶年”，其效果为（生活消费品工厂：+30%，适役人口修正：-80%）
            //else if (GetMatch(str, "^([\u4e00-\u9fa5\\d/]+)获得(?:为期)?(\\d+[天月年])的“([\u4e00-\u9fa5]+)”，其效果为（(.+)）$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[4].Value, "，", out var subSentences))
            //    {
            //        return false;
            //    }
            //    var motion = Motions.Gain;
            //    var valueType = Types.Label;
            //    var value = match.Groups[3].Value + PublicSplitter + match.Groups[2].Value;
            //    var triggerType = Types.State;
            //    var trigger = new string[] { match.Groups[1].Value };
            //    formattedList = new() { new(motion, valueType, value, triggerType, trigger, subSentences) };
            //}
            //// 复语句-获得|移除标签 + 效果说明
            ///// 获得无为而治，其效果为（每周人口：+1，每月人口：+15%）
            ///// 移除圣人，其效果为（孤立倾向：+0.1，每日唯心度变化：+0.04%）
            ///// 获得幽灵/亡灵，其效果为（战略资源获取率：+10%，部队组织度：-5%，适役人口：+0.2%，移动中组织度损失：+5%，部队损耗：-20%，补给损耗：-15%，部队组织度恢复：+10%，步兵部队攻击：-30%，步兵部队防御：-30%，训练时间：-40%，每日人类影响力基础变化：+0.02，征兵法案花费：+50%）
            ///// 获得“我蛮夷也”，其效果为（政治点数：-20%，部队组织度：+20%，制造战争的紧张度限制：-20%，正当化战争目标时间：-25%）
            //else if (GetMatch(str, "^(获得|移除)“?([\u4e00-\u9fa5\\d/]+)”?，其效果为（(.+)）$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[3].Value, "，", out var subSentences))
            //    {
            //        return false;
            //    }
            //    var motion = (match.Groups[1].Value == "获得" ? Motions.Gain : Motions.Remove);
            //    var valueType = Types.Label;
            //    var value = match.Groups[2].Value;
            //    formattedList = new() { new(motion, valueType, value, null, null, subSentences) };
            //}
            //// 复语句-获得|移除标签 + 外交关系
            ///// 获得强烈的战略结盟（对隐居村落的关系：+100）
            //else if (GetMatch(str, "^(获得|移除)([\u4e00-\u9fa5]+)（(.+)）$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[3].Value, "）（", out var subSentences))
            //    {
            //        return false;
            //    }
            //    var motion = (match.Groups[1].Value == "获得" ? Motions.Gain : Motions.Remove);
            //    var valueType = Types.Label;
            //    var value = match.Groups[2].Value;
            //    formattedList = new() { new(motion, valueType, value, null, null, subSentences) };
            //}
            //// 复语句-某国获得|失去标签
            ///// （守矢神社）获得万民自化，其效果为（每周稳定度：+0.1%，每周战争支持度：-0.1%，战争支持度：+100%）
            //else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）(获得|失去)([\u4e00-\u9fa5]+)，其效果为（(.+)）$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[4].Value, "，", out var subSentences))
            //    {
            //        return false;
            //    }
            //    var motion = (match.Groups[2].Value == "获得" ? Motions.Gain : Motions.Remove);
            //    var valueType = Types.Label;
            //    var value = match.Groups[3].Value;
            //    var triggerType = Types.State;
            //    var trigger = new string[] { match.Groups[1].Value };
            //    formattedList = new() { new(motion, valueType, value, triggerType, trigger, subSentences) };
            //}
            //// 复语句-某国获得|失去标签
            ///// （隐居村落）获得自古以来（对命莲寺关系：+200）
            ///// （幻想风洞）获得合纵（和草原流亡者的关系+500）（和妖精乐园的关系-500）
            ///// （妖精乐园）获得“？”（和草原流亡者的关系-100）
            //else if (GetMatch(str, "^（([\u4e00-\u9fa5]+)）(获得|失去)“?([\u4e00-\u9fa5？]+)”?（(.+)）$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[4].Value, "）（", out var subSentences))
            //    {
            //        return false;
            //    }
            //    var motion = (match.Groups[2].Value == "获得" ? Motions.Gain : Motions.Remove);
            //    var valueType = Types.Label;
            //    var value = match.Groups[3].Value;
            //    var triggerType = Types.State;
            //    var trigger = new string[] { match.Groups[1].Value };
            //    formattedList = new() { new(motion, valueType, value, triggerType, trigger, subSentences) };
            //}
            //// 复语句-每个国家获得标签
            ///// 每个国家获得：道德观不同（对隐世村落关系：-20）
            //else if (GetMatch(str, "^([\u4e00-\u9fa5]+)(获得|失去)：([\u4e00-\u9fa5]+)（(.+)）$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[4].Value, "，", out var subSentences))
            //    {
            //        return false;
            //    }
            //    var motion = (match.Groups[2].Value == "获得" ? Motions.Gain : Motions.Remove);
            //    var valueType = Types.Label;
            //    var value = match.Groups[3].Value;
            //    var triggerType = Types.State;
            //    var trigger = new string[] { match.Groups[1].Value };
            //    formattedList = new() { new(motion, valueType, value, triggerType, trigger, subSentences) };
            //}
            //// 复语句-变更等级
            ///// 向保守社会发展一级，以保守社会5取代保守社会4，效果变化（稳定度：+6% 科研速度：-3% 加密：+0.5 可出口资源：-5% 意识形态变化抵制力度：+10%）
            //else if (GetMatch(str, "^[\u4e00-\u9fa5]+，以([\u4e00-\u9fa5]+\\d)取代([\u4e00-\u9fa5]+\\d)，效果变化（(.+)）$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[3].Value, " ", out var subSentences))
            //    {
            //        return false;
            //    }
            //    var motion = Motions.Replace;
            //    var valueType = Types.Grade;
            //    var value = match.Groups[2].Value + PublicSplitter + match.Groups[1].Value; // eg.保守社会4|保守社会5
            //    formattedList = new() { new(motion, valueType, value, null, null, subSentences) };
            //}
            //// 复语句-追加效果
            ///// 天地不仁追加效果：稳定度：-5%，适役人口：+2%
            //else if (GetMatch(str, "^([\u4e00-\u9fa5]+)追加效果：(.+)$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[2].Value, "，", out var subSentences))
            //    {
            //        return false;
            //    }
            //    foreach (var subSentence in subSentences)
            //    {
            //        subSentence.TriggerType = Types.Label;
            //        subSentence.Triggers = new() { match.Groups[1].Value };
            //        formattedList.Add(subSentence);
            //    }
            //}
            //// 复语句-某国触发可同意事件
            ///// 每个孤立国家触发事件“隐世村落的无偿教导？”。如果他们同意，则获得强烈的战略结盟（对隐居村落的关系：+100）
            //else if (GetMatch(str, "^(每个孤立国家)触发事件“([\u4e00-\u9fa5？]+)”。如果他们同意，则(.+)$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[3].Value, null, out var subSentences))
            //    {
            //        return false;
            //    }
            //    var motion = Motions.Trigger;
            //    var valueType = Types.RequestEvent;
            //    var value = match.Groups[2].Value;
            //    var triggerType = Types.State;
            //    var trigger = new string[] { match.Groups[1].Value };
            //    formattedList = new() { new(motion, valueType, value, triggerType, trigger, subSentences) };
            //}
            //// 复语句-多国触发事件
            ///// （天狗帝国）（山姥部落）（妖兽仙界）触发事件“有偿帮助”。如果他们同意，则获得保护自己的臣民，其效果为（陆上要塞：建造速度：+25%）
            ///// （斯卡雷特帝国）触发事件：“承认流亡者？”。如果他们同意，则获得幻想乡正规势力，其效果为（政治点数：+20%，部队组织度：+10%，制造战争的紧张度限制：+20%，正当化战争目标所需时间：+25%）
            //else if (GetMatch(str, "^(（[\u4e00-\u9fa5？]+）)+触发事件：?“([\u4e00-\u9fa5？]+)”。如果他们同意，则(.+)$", out match))
            //{
            //    List<string> states = new();
            //    while (GetMatch(str, "^（([\u4e00-\u9fa5？]+)）(.+)$", out match))
            //    {
            //        states.Add(match.Groups[1].Value);
            //        str = match.Groups[2].Value;
            //    }
            //    GetMatch(str, "^触发事件：?“([\u4e00-\u9fa5？]+)”。如果他们同意，则(.+)$", out match);
            //    if (!GetSubSentence(match.Groups[2].Value, null, out var subSentences))
            //    {
            //        return false;
            //    }
            //    var motion = Motions.Trigger;
            //    var valueType = Types.RequestEvent;
            //    var value = match.Groups[1].Value;
            //    var triggerType = Types.State;
            //    var trigger = states.ToArray(); // eg.天狗帝国|山姥部落|妖兽仙界
            //    formattedList = new() { new(motion, valueType, value, triggerType, trigger, subSentences) };
            //}
            //// 复语句-开启国策后立即实施效果
            ///// 当选中此项时：所有敌国获得为期365天的“必有凶年”，其效果为（生活消费品工厂：+30%，适役人口修正：-80%）
            //else if (GetMatch(str, "^当选中此项时：(.+)$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[1].Value, null, out var subSentences))
            //    {
            //        return false;
            //    }
            //    foreach (var subSentence in subSentences)
            //    {
            //        if (subSentence.Motion == Motions.None)
            //        {
            //            return false;
            //        }
            //        subSentence.Motion = Motions.Instantly | subSentence.Motion ^ Motions.AfterDone;
            //        formattedList.Add(subSentence);
            //    }
            //}
            //// 复语句-某国增加效果
            ///// 守矢神社增加：科研共享加成+5%
            //else if (GetMatch(str, "^([\u4e00-\u9fa5]+)增加：(.+)$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[2].Value, null, out var subSentences))
            //    {
            //        return false;
            //    }
            //    foreach (var subSentence in subSentences)
            //    {
            //        subSentence.TriggerType = Types.State;
            //        subSentence.Triggers = new() { match.Groups[1].Value };
            //        formattedList.Add(subSentence);
            //    }
            //}
            //// 复语句-ai修正
            ///// ai修正：专注防御：+30%
            //else if (GetMatch(str, "^ai修正：(.+)$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[1].Value, null, out var subSentences))
            //    {
            //        return false;
            //    }
            //    foreach (var subSentence in subSentences)
            //    {
            //        subSentence.TriggerType = Types.AiModifier;
            //        formattedList.Add(subSentence);
            //    }
            //}
            //// 复语句-区域增加效果
            ///// 村民生活区（增加10个建筑位，增加10个民用工厂）
            ///// 山中鸡场（移除1个民用工厂，增加1个军用工厂）
            //else if (GetMatch(str, "^([\u4e00-\u9fa5]+)（(.+)）$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[2].Value, "，", out var subSentences))
            //    {
            //        return false;
            //    }
            //    foreach (var subSentence in subSentences)
            //    {
            //        subSentence.TriggerType = Types.Region;
            //        subSentence.Triggers = new() { match.Groups[1].Value };
            //        formattedList.Add(subSentence);
            //    }
            //}
            //// 复语句-增加部队
            ///// 所有拥有的地区：添加5个基础设施
            ///// 所有陆军指挥官：获得特质魅力非凡
            //else if (GetMatch(str, "^([\u4e00-\u9fa5]+)：(.+)$", out match))
            //{
            //    if (!GetSubSentence(match.Groups[2].Value, "，", out var subSentences))
            //    {
            //        return false;
            //    }
            //    var motion = Motions.Modify;
            //    var valueType = Types.Resource;
            //    var value = match.Groups[1].Value;
            //    formattedList = new() { new(motion, valueType, value, null, null, subSentences) };
            //}

            //else
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 从复合子句中格式化所有短句
        /// </summary>
        /// <param name="subSentence">子句</param>
        /// <param name="splitter">复合子句分割符号</param>
        /// <param name="subSentences">拆分后的所有格式化的子句</param>
        /// <returns>全部子句格式化成功返回true，有一个或以上失败则返回false</returns>
        private static bool GetSubSentence(string subSentence, string? splitter, out List<EffectSentence> subSentences)
        {
            subSentences = new();
            var clauses = splitter == null ? new[] { subSentence } : subSentence.Split(splitter);
            if (clauses.Length is 0)
            {
                return false;
            }
            for (var i = 0; i < clauses.Length; i++)
            {
                if (!SinglePatternFormatter(clauses[i], out var formattedList) &&
                    !ComplexPatternFormatter(clauses[i], out formattedList))
                {
                    return false;
                }
                foreach (var formatted in formattedList)
                {
                    subSentences.Add(formatted);
                }
            }
            return true;
        }

        #endregion
    }
}
