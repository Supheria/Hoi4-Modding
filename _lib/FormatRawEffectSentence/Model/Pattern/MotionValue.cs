using FormatRawEffectSentence.LocalSign;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeBundle;

namespace FormatRawEffectSentence.Model.Pattern;

public class MotionValue : ISsSerializable
{
    internal Types Type { get; private set; }

    internal HashSet<int> PartIndexOrder { get; private set; } = [0];

    public MotionValue(Types type)
    {
        Type = type;
    }

    public MotionValue() : this(Types.None)
    {
    }

    public string LocalName { get; set; } = nameof(MotionValue);

    public void Serialize(SsSerializer serializer)
    {
        serializer.WriteTag(nameof(Type), Type.ToString());
        serializer.WriteTag(nameof(PartIndexOrder), PartIndexOrder.ToArrayString());
    }

    public void Deserialize(SsDeserializer deserializer)
    {
        Type = deserializer.ReadTag(nameof(Type), s => s.ToEnum(Type));
        PartIndexOrder = deserializer.ReadTag(nameof(PartIndexOrder), s => s.ToCollection(s => s.ToInt(null)))?.ToHashSet() ?? PartIndexOrder;
    }
}
