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
    /// patterns within () mean to retain as key word
    /// </summary>
#if DEBUG
    internal readonly List<RawPattern> Patterns =

#else
    internal static List<RawPattern> Patterns =
#endif
    [
        new(false, @$"（某国）触发事件", new[]
        {
            @$"触发事件“骑着青牛的老者？”。",
            @$"触发事件：“借地？”",
            @$"（妖精乐园）触发事件：“解释”",
            @$"[x] 神灵庙触发事件“道教该如何面对道家？”",
        })
        {
            Trigger = new(Types.State, @$"（({ZnCharCollection}+)）"),
            Motion = new(Motions.Trigger, @$"触发事件：?“(({ZnCharCollection}|{ZnMarkCollection})+)”。?"),
            Value = new(Types.Event),
        },

        new(false, @$"固定值或类型", new[]
        {
            @$"将平均灵力值固定为50%",
            @$"将世界观固定为唯心世界观",
        })
        {
            Motion = new(Motions.Lock, @$"将({ZnCharCollection}+)固定为({ZnCharCollection}+|{NumericValue})"),
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
            Motion = new(Motions.Unlock, @$"移除对({ZnCharCollection}+)的固定"),
            Value = new(Types.Variable),
        },

        new(false, @$"+|-值", new[]
        {
            @$"灵力系科研速度：+35%",
            @$"稳定度：-30%",
            @$"每日获得的政治点数：+0.1",
            @$"（人类村落）政治点数：+300",
        })
        {
            Trigger = new(Types.State, @$"（({ZnCharCollection}+)）"),
            Motion = new(1,  @$"({ZnCharCollection}+)：?([+-])({NumericValue})", new()
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

        new(false, @$"{AddSubString}个数", new[]
        {
            @$"获得1个科研槽",
            @$"增加10个建筑位",
            @$"移除1个民用工厂",
        })
        {
            Motion = new(0, @$"({AddSubString})(\d+)个({ZnCharCollection}+)", ReverseMotionConditionMap(new()
            {
                [Motions.Add] = AddWords,
                [Motions.Sub] = SubWords,
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

        new(false, @$"（某国）值{AddSubString}点", new[]
        {
            @$"（人类村落）厌战降低50点",
        })
        {
            Trigger = new(Types.State, @$"（({ZnCharCollection}+)）"),
            Motion = new(1, @$"({ZnCharCollection}+)({AddSubString})({NumericValue})点", ReverseMotionConditionMap(new()
            {
                [Motions.Add] = AddWords,
                [Motions.Sub] = SubWords,
            })),
            Value = new(Types.Variable)
            {
                PartIndexOrder =
                {
                    0,
                    2,
                }
            },
        },

        new(false, @$"宣战可用性", new[]
        {
            @$"规则修改：不能宣战",
            @$"规则修改：可以宣战",
            @$"不能宣战",
        })
        {
            Motion = new(0, @$"(?:规则修改：)?({AbleUnableString})(宣战)", ReverseMotionConditionMap(new()
            {
                [Motions.Gain] = AbleWords,
                [Motions.Remove] = UnableWords,
            })),
            Value = new(Types.Availability)
            {
                PartIndexOrder =
                {
                    1,
                }
            },
        },

        new(false, @$"可以开启某项决议", new[]
        {
            @$"可以通过决议发动周边国家的内战使其变为附庸",
        })
        {
            Motion = new(Motions.Gain, @$"可以通过(决议)({ZnCharCollection}+)"),
            Value = new(Types.Availability)
            {
                PartIndexOrder =
                {
                    0,
                    1,
                }
            },
        },

        new(false, @$"开启某项决议", new[]
        {
            @$"?",
        })
        {
            Motion = new(Motions.Start, @$"开启({ZnCharCollection}+)决议"),
            Value = new(Types.Resolution),
        },

        new(false, @$"自动获得核心可用性", new[]
        {
            @$"（幽灵种族）的省份将自动获得核心：是",
        })
        {
            Trigger = new(Types.Province, @$"（({ZnCharCollection}+)）"),
            Motion = new(1, @$"的省份将(自动获得核心)：(是|否)", ReverseMotionConditionMap(new()
            {
                [Motions.Gain] = AbleWords,
                [Motions.Remove] = UnableWords,
            })),
            Value = new(Types.Availability),
        },

        new(false, @$"获得科技", new[]
        {
            $@"获得科技：147季无线电",
        })
        {
            Motion = new(Motions.Gain, $@"获得科技：(.+)"),
            Value = new(Types.Technology),
        },
    ];
}