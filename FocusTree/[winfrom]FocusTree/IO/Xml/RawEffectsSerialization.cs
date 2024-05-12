using LocalUtilities.SimpleScript.Data;
using LocalUtilities.SimpleScript.Serialization;

namespace FocusTree.IO.Xml;

public class RawEffectsSerialization : SsSerialization<List<string>>
{
    public override string LocalName => "RawEffect";

    protected override void Serialize()
    {
        foreach (var str in Source)
            WriteToken(str);
    }

    protected override void Deserialize()
    {
        Deserialize(typeof(Token), token =>
        {
            Source.Add(token.Name.Text);
        });
    }
}