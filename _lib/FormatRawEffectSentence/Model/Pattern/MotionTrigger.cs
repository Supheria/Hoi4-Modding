using FormatRawEffectSentence.LocalSign;
using LocalUtilities.StringUtilities;

namespace FormatRawEffectSentence.Model.Pattern;

public class MotionTrigger
{
    internal Types Type { get; }

    internal string Pattern { get; }

    public MotionTrigger(Types type, string pattern)
    {
        Type = type;
        Pattern = pattern is "" ? pattern : RegexPatternTool.ExcludeBlankInExclusiveOrUnlimitedCollection(pattern);
    }

    public MotionTrigger(Types type) : this(type, "")
    {
    }

    public MotionTrigger() : this(Types.None, "")
    {
    }
}