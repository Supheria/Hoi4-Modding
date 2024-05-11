using LocalUtilities.SerializeUtilities;
using LocalUtilities.SimpleScript.Data;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.StringUtilities;
using System.Xml;
using System.Xml.Serialization;

namespace FocusTree.IO.Xml;

public class RequiresSerialization : SsSerialization<List<HashSet<int>>>
{
    public override string LocalName { get; }

    public RequiresSerialization(string localName) : base([])
    {
        LocalName = localName;
        OnSerialize += Requires_Serialize;
        OnDeserialize += Requires_Deserialize;
    }

    private void Requires_Serialize()
    {
        foreach (var sets in Source)
        {
            if (sets.Count is not 0)
                WriteToken(sets.ToArrayString());
        }
    }

    private void Requires_Deserialize()
    {
        Deserialize(typeof(Token), token =>
        {
            var list = token.Name.Text.ToCollection(s => s.ToInt(null));
            if (list is not null)
                Source.Add(list.ToHashSet());
        });
    }
}