namespace FormatRawEffectSentence.LocalSign;

/// <summary>
/// 对象类型
/// </summary>
[Flags]
public enum Types
{
    None = 0,
    //
    //
    // 基础
    //
    //
    /// <summary>
    /// 事件
    /// </summary>
    Event = 0b1,
    /// <summary>
    /// 可同意的事件
    /// </summary>
    RequestEvent = Event << 1,
    /// <summary>
    /// 标签
    /// </summary>
    Label = RequestEvent << 1,
    /// <summary>
    /// 变量
    /// </summary>
    Variable = Label << 1,
    /// <summary>
    /// 可用性
    /// </summary>
    Availability = Variable << 1,
    /// <summary>
    /// 战争目标
    /// </summary>
    WarGoal = Availability << 1,
    /// <summary>
    /// 决议
    /// </summary>
    Resolution = WarGoal << 1,
    /// <summary>
    /// 阵营
    /// </summary>
    Camp = Resolution << 1,
    /// <summary>
    /// 研究
    /// </summary>
    Research = Camp << 1,
    /// <summary>
    /// 等级
    /// </summary>
    Grade = Research << 1,
    /// <summary>
    /// 部队
    /// </summary>
    Troop = Grade << 1,
    /// <summary>
    /// 国家
    /// </summary>
    State = Troop << 1,
    /// <summary>
    /// 省份
    /// </summary>
    Province = State << 1,
    /// <summary>
    /// 区域
    /// </summary>
    Region = Province << 1,
    /// <summary>
    /// Ai修正
    /// </summary>
    AiModifier = Region << 1,
    /// <summary>
    /// 区域核心
    /// </summary>
    RegionCore = AiModifier << 1,
    /// <summary>
    /// 资源
    /// </summary>
    Resource = RegionCore << 1,
    //
    //
    // 可用性
    //
    //
    /// <summary>
    /// 可以宣战
    /// </summary>
    AbleToDeclareWar = Availability | Resource << 1,
    /// <summary>
    /// 可以自动获取核心
    /// </summary>
    AbleToGainCoreAuto = Availability | (AbleToDeclareWar ^ Availability) << 1,
    /// <summary>
    /// 可以创建阵营
    /// </summary>
    AbleToCreateCamp = Availability | (AbleToGainCoreAuto ^ Availability) << 1,
    /// <summary>
    /// 可以加入阵营
    /// </summary>
    AbleToJoinCamp = Availability | (AbleToCreateCamp ^ Availability) << 1,
    /// <summary>
    /// 可以开启决议
    /// </summary>
    AbleToStartResolution = Availability | (AbleToJoinCamp ^ Availability) << 1,
}