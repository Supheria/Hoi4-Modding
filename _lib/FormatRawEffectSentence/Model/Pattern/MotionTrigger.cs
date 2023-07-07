using FormatRawEffectSentence.InternalSign;
using LocalUtilities.RegexUtilities;

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

    public MotionTrigger() : this(Types.None, "")
    {
    }
}