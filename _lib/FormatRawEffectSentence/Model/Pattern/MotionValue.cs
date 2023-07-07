using FormatRawEffectSentence.InternalSign;

namespace FormatRawEffectSentence.Model.Pattern;

public class MotionValue
{
    internal Types Type { get; }

    internal List<int> PartIndexOrder { get; } = new();

    public MotionValue(Types type)
    {
        Type = type;
    }

    public MotionValue() : this(Types.None)
    {
    }
}