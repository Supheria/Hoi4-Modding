using LocalUtilities.SimpleScript.Data;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.StringUtilities;

namespace FocusTree.IO.Xml;

public class RequiresSerialization : SsSerialization<List<HashSet<int>>>
{
    public override string LocalName => "Require";

    protected override void Serialize()
    {
        foreach (var sets in Source)
        {
            if (sets.Count is not 0)
                WriteToken(sets.ToArrayString());
        }
    }

    protected override void Deserialize()
    {
        Deserialize(typeof(Token), token =>
        {
            var list = token.Name.Text.ToCollection(s => s.ToInt(null));
            if (list is not null)
                Source.Add(list.ToHashSet());
        });
    }
}