using LocalUtilities.SimpleScript.Serialization;

namespace FormatRawEffectSentence.Model.Pattern;

public class RawPattern : ISsSerializable
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
    public string LocalName { get; set; } = nameof(RawPattern);

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteComment($"{(IsComplex ? "complex" : "single")}: {Title}");
        foreach (var sample in Samples)
            serializer.WriteComment($"{sample}");
        serializer.WriteObject(Trigger);
        serializer.WriteObject(Motion);
        serializer.WriteObject(Value);
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        Trigger = deserializer.ReadObject(Trigger);
        Motion = deserializer.ReadObject(Motion);
        Value = deserializer.ReadObject(Value);
    }
}
