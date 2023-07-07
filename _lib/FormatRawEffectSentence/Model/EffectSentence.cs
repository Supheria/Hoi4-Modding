using FormatRawEffectSentence.InternalSign;
using FormatRawEffectSentence.IO;

namespace FormatRawEffectSentence.Model;

public class EffectSentence
{
    /// <summary>
    /// 执行动作
    /// </summary>
    public Motions Motion { get; }
    /// <summary>
    /// 值类型
    /// </summary>
    public Types Type { get; }
    /// <summary>
    /// 执行值
    /// </summary>
    public string Value { get; }
    /// <summary>
    /// 触发者类型
    /// </summary>
    public Types TriggerType { get; }
    /// <summary>
    /// 动作触发者
    /// </summary>
    public string[] Triggers { get; }
    /// <summary>
    /// 子句
    /// </summary>
    public List<EffectSentence> SubSentences { get; } = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="motion">执行动作</param>
    /// <param name="valueType">值类型</param>
    /// <param name="value">执行值</param>
    /// <param name="triggerType">触发者类型</param>
    /// <param name="triggers">动作触发者</param>
    /// <returns></returns>
    public EffectSentence(
        Motions motion,
        Types valueType,
        string value,
        Types triggerType,
        string[] triggers
    )
    {
        Motion = motion;
        Type = valueType;
        Value = value;
        TriggerType = triggerType;
        Triggers = triggers;
    }

    /// <summary>
    /// 用于序列化
    /// </summary>
    public EffectSentence() : this(Motions.None, Types.None, "", Types.None, Array.Empty<string>())
    {
    }

    /// <summary>
    /// 转换为json字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString() => new EffectSentenceStringSerialization { Source = this }.ToString();

    /// <summary>
    /// 用json字符串生成
    /// </summary>
    /// <param name="jsonString"></param>
    public static EffectSentence FromString(string jsonString)
    {
        throw new NotImplementedException();
    }
}