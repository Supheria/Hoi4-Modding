namespace FormatRawEffectSentence.InternalSign;

/// <summary>
/// 执行动作
/// </summary>
[Flags]
public enum Motions
{
    None = 0,

    #region ==== 额外动作时机：不添加此项则默认为国策完成后开始实施 ====
    /// <summary>
    /// 开启国策后立即实施
    /// </summary>
    Instantly = 0b1,

    #endregion

    #region ==== 基础 ====

    NoneButMayChange = Instantly << 1,
    /// <summary>
    /// 触发
    /// </summary>
    Trigger = NoneButMayChange << 1,
    /// <summary>
    /// 修改
    /// </summary>
    Modify = Trigger << 1,
    /// <summary>
    /// 创建
    /// </summary>
    Create = Modify << 1,
    /// <summary>
    /// 加入
    /// </summary>
    Join = Create << 1,
    /// <summary>
    /// 吞并
    /// </summary>
    Annexed = Join << 1,
    /// <summary>
    /// 开启
    /// </summary>
    Start = Annexed << 1,

    #endregion

    #region ==== 修改 ====

    /// <summary>
    /// 加
    /// </summary>
    Add = Modify | Join << 1,
    /// <summary>
    /// 减
    /// </summary>
    Sub = Modify | (Add ^ Modify) << 1,
    /// <summary>
    /// 固定
    /// </summary>
    Fixed = Modify | (Sub ^ Modify) << 1,
    /// <summary>
    /// 取消固定
    /// </summary>
    Unpin = Modify | (Fixed ^ Modify) << 1,
    /// <summary>
    /// 获得
    /// </summary>
    Gain = Modify | (Unpin ^ Modify) << 1,
    /// <summary>
    /// 移除
    /// </summary>
    Remove = Modify | (Gain ^ Modify) << 1,
    /// <summary>
    /// 加成
    /// </summary>
    Bonus = Modify | (Remove ^ Modify) << 1,
    /// <summary>
    /// 取代
    /// </summary>
    Replace = Modify | (Bonus ^ Modify) << 1

    #endregion
};