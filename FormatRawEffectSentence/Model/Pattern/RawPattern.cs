using FormatRawEffectSentence.InternalSign;
using System.Xml;
using LocalUtilities.XmlUtilities;
using LocalUtilities.RegexUtilities;

namespace FormatRawEffectSentence.Model.Pattern;

public class RawPattern
{
    internal bool IsComplex { get; }

    internal string Title { get; }

    internal string[] Samples { get; }

    internal MotionTrigger Trigger { get; set; } = new();

    internal Motion Motion { get; set; } = new();

    internal MotionValue Value { get; set; } = new();

    public RawPattern(bool isComplex, string title, string[] samples)
    {
        IsComplex = isComplex;
        Title = title;
        Samples = samples;
    }

    public RawPattern() : this(false, "", Array.Empty<string>())
    {
    }
}