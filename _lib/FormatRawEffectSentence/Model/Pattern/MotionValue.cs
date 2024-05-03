using FormatRawEffectSentence.LocalSign;

namespace FormatRawEffectSentence.Model.Pattern;

public class MotionValue
{
    internal Types Type { get; }

    internal HashSet<int> PartIndexOrder { get; } = new() { 0 };

    public MotionValue(Types type)
    {
        Type = type;
    }

    public MotionValue() : this(Types.None)
    {
    }
}