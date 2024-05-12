﻿using FormatRawEffectSentence.Model.Pattern;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.StringUtilities;

namespace FormatRawEffectSentence.IO.Pattern;

public class MotionSerialization : SsSerialization<Motion>
{
    public override string LocalName => "Motion";

    protected override void Serialize()
    {
        WriteTag(nameof(Source.Pattern), Source.Pattern);
        WriteTag(nameof(Source.PartIndex), Source.PartIndex.ToString());
        if (Source.PartIndex is -1)
            WriteTag(LocalName, Source.ConditionMap[""].ToString());
        else
            Serialize(Source.ConditionMap.ToList(), new MotionConditionSerialization());
    }

    protected override void Deserialize()
    {
        var partIndex = ReadTag(nameof(Source.PartIndex), s => s.ToInt(Source.PartIndex));
        var pattern = ReadTag(nameof(Source.Pattern), s => s ?? Source.Pattern);
        if (partIndex is -1)
        {
            Source = new(partIndex, pattern);
            Source.ConditionMap[""] = ReadTag(LocalName, s => s.ToEnum(Source.ConditionMap[""]));
        }
        else
        {
            var conditionMap = Deserialize(Source.ConditionMap.ToList(), new MotionConditionSerialization()).ToDictionary();
            Source = new(partIndex, pattern, conditionMap);
        }
    }
}