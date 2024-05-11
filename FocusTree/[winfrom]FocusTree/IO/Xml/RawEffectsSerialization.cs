using LocalUtilities.SerializeUtilities;
using LocalUtilities.SimpleScript.Data;
using LocalUtilities.SimpleScript.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace FocusTree.IO.Xml;

public class RawEffectsSerialization : SsSerialization<List<string>>
{
    public override string LocalName { get; }

    public RawEffectsSerialization(string localName) : base([])
    {
        LocalName = localName;
        OnSerialize += RawEffects_Serialize;
        OnDeserialize += RawEffects_Deserialize;
    }

    private void RawEffects_Serialize()
    {
        foreach(var str in Source)
            WriteToken(str);
    }

    private void RawEffects_Deserialize()
    {
        Deserialize(typeof(Token), token =>
        {
            Source.Add(token.Name.Text);
        });
    }
}