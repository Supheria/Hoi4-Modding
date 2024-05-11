using LocalUtilities.SerializeUtilities;
using LocalUtilities.SimpleScript.Data;
using LocalUtilities.SimpleScript.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace FocusTree.IO.Xml;

public class RawEffectsSerialization : SsSerialization<List<string>>
{
    public override string LocalName => "RawEffect";

    public RawEffectsSerialization()
    {
        OnSerialize += Serialize;
        OnDeserialize += Deserialize;
    }

    private void Serialize()
    {
        foreach(var str in Source)
            WriteToken(str);
    }

    private void Deserialize()
    {
        Deserialize(typeof(Token), token =>
        {
            Source.Add(token.Name.Text);
        });
    }
}