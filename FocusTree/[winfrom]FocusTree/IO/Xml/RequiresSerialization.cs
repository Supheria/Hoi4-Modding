using LocalUtilities.SerializeUtilities;
using LocalUtilities.SimpleScript.Data;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Serialization;

namespace FocusTree.IO.Xml;

public class RequiresSerialization : SsSerialization<List<HashSet<int>>>
{
    public override string LocalName => "Require";

    public RequiresSerialization()
    {
        OnSerialize += Serialize;
        OnDeserialize += Deserialize;
    }

    private void Serialize()
    {
        foreach (var sets in Source)
        {
            if (sets.Count is not 0)
                WriteToken(sets.ToArrayString());
        }
    }

    private void Deserialize()
    {
        Deserialize(typeof(Token), token =>
        {
            var list = token.Name.Text.ToCollection(s => s.ToInt(null));
            if (list is not null)
                Source.Add(list.ToHashSet());
        });
    }
}