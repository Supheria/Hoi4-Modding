using FormatRawEffectSentence.LocalSign;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeGeneral.Convert;

namespace FormatRawEffectSentence.Model;

public class EffectSentence : ISsSerializable
{
    /// <summary>
    /// 执行动作
    /// </summary>
    public Motions Motion { get; private set; }
    /// <summary>
    /// 值类型
    /// </summary>
    public Types ValueType { get; private set; }
    /// <summary>
    /// 执行值
    /// </summary>
    public string Value { get; private set; }
    /// <summary>
    /// 触发者类型
    /// </summary>
    public Types TriggerType { get; private set; }
    /// <summary>
    /// 动作触发者
    /// </summary>
    public string[] Triggers { get; private set; }
    /// <summary>
    /// 子句
    /// </summary>
    public List<EffectSentence> SubSentences { get; private set; } = [];

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
        ValueType = valueType;
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
    public override string ToString()
    {
        var serializer = new SsSerializer(this, new(true));
        return serializer.Serialize();
    }

    public static EffectSentence Parse(string str)
    {
        var sentence = new EffectSentence();
        sentence.ParseSsString(str);
        return sentence;
    }

    public string LocalName { get; set; } = "Effect";

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(Motion), Motion.ToString());
        serializer.WriteTag(nameof(ValueType), ValueType.ToString());
        serializer.WriteTag(nameof(Value), Value);
        serializer.WriteTag(nameof(TriggerType), TriggerType.ToString());
        serializer.WriteTag(nameof(Triggers), Triggers.ToArrayString());
        serializer.WriteObjects(LocalName, SubSentences);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        Motion = deserializer.ReadTag(nameof(Motion), s => s.ToEnum<Motions>());
        ValueType = deserializer.ReadTag(nameof(ValueType), s => s.ToEnum<Types>());
        Value = deserializer.ReadTag(nameof(Value), s => s ?? Value);
        TriggerType = deserializer.ReadTag(nameof(TriggerType), s => s.ToEnum<Types>());
        Triggers = deserializer.ReadTag(nameof(Triggers), s => s ?? Triggers.ToArrayString()).ToArray();
        SubSentences = deserializer.ReadObjects<EffectSentence>(LocalName);
    }
}