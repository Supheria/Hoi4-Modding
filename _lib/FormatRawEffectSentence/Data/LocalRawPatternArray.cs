using FormatRawEffectSentence.LocalSign;
using FormatRawEffectSentence.Model.Pattern;
using static FormatRawEffectSentence.Data.RawPatternArrayUtilities;

namespace FormatRawEffectSentence.Data;

internal class LocalRawPatternArray
{
    private static Dictionary<string, Motions> ReverseMotionConditionMap(Dictionary<Motions, string[]> rawDictionary) =>
        ReverseDictionary(rawDictionary);

    /// <summary>
    /// use [\.] rather than [.] for character collection
    /// </summary>
#if DEBUG
    internal readonly RawPattern[] Patterns =
#else
    internal static RawPattern[] Patterns =
#endif
    {
        new(false, @$"（某国）触发事件", new[]
        {
            @$"触发事件“骑着青牛的老者？”。",
            @$"触发事件：“借地？”",
            @$"（妖精乐园）触发事件：“解释”",
            @$"[x] 神灵庙触发事件“道教该如何面对道家？”",
        })
        {
            Trigger = new(Types.State, @$"（({CollectZnChar}+)）"),
            Motion = new(Motions.Trigger, @$"触发事件：?“(({CollectZnChar}|{CollectZnMark})+)”。?"),
            Value = new(Types.Event)
            {
                PartIndexOrder =
                {
                    0,
                }
            },
        },

        new(false, @$"固定值或类型", new[]
        {
            @$"将平均灵力值固定为50%",
            @$"将世界观固定为唯心世界观",
        })
        {
            Motion = new(Motions.Fixed, @$"将({CollectZnChar}+)固定为({CollectZnChar}+|{NumericValue})"),
            Value = new(Types.Variable)
            {
                PartIndexOrder =
                {
                    0,
                    1,
                }
            },
        },

        new(false, @$"移除固定", new []
        {
            @$"移除对势力规模的固定",
        })
        {
            Motion = new(Motions.Unpin, @$"移除对({CollectZnChar}+)的固定"),
            Value = new(Types.Variable)
            {
                PartIndexOrder =
                {
                    0,
                }
            },
        },

        new(false, @$"+|-值", new[]
        {
            @$"灵力系科研速度：+35%",
            @$"稳定度：-30%",
            @$"每日获得的政治点数：+0.1",
            @$"（人类村落）政治点数：+300",
        })
        {
            Trigger = new(Types.State, @$"（({CollectZnChar}+)）"),
            Motion = new(1,  @$"({CollectZnChar}+)：?([+-])({NumericValue})", new()
            {
                ["+"] = Motions.Add,
                ["-"] = Motions.Sub,
            }),
            Value = new(Types.Variable)
            {
                PartIndexOrder =
                {
                    0,
                    2,
                }
            },
        },

        new(false, @$"修正值", new[]
        {
            @$"适役人口修正：15%",
        })
        {
            Motion = new(Motions.Modify, @$"([^修]+)修正：?({NumericValue})"),
            Value = new(Types.Variable)
            {
                PartIndexOrder =
                {
                    0,
                    1,
                }
            },
        },

        new(false, @$"{AddClausesString}|{SubClausesString}个数", new[]
        {
            @$"获得1个科研槽",
            @$"增加10个建筑位",
            @$"移除1个民用工厂",
        })
        {
            Motion = new(0, @$"({AddClausesString}|{SubClausesString})(\d+)个({CollectZnChar}+)", ReverseMotionConditionMap(new()
            {
                [Motions.Add] = AddClauses,
                [Motions.Sub] = SubClauses,
            })),
            Value = new(Types.Variable)
            {
                PartIndexOrder =
                {
                    2,
                    1,
                }
            },
        },
    };
}