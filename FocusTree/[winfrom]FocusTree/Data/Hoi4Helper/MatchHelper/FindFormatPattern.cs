using FocusTree.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using static FocusTree.Data.Hoi4Helper.PublicSign;

namespace FocusTree.Data.Hoi4Helper.MatchHelper;

[XmlRoot("Patterns")]
public class FindFormatPattern : IXmlSerializable
{
    private readonly List<MatchPattern> _patterns = new();
    public static void ReadMatchPatternFile()
    {
        var patterns = XmlIO.LoadFromXml<FindFormatPattern>("test.xml")?._patterns ?? new();
    }

    public static void WriteMatchPatternFile()
    {
        var pattern = new MatchPattern
        {
            TriggerType = Types.State,
            TriggerPattern = "（([^（]+)）",
            MotionPattern = "([^：]+)：?([+-])([\\d.]+%?)",
            MotionPartIndex = 1,
            MotionConditionMap =
            {
                ["+"] = Motions.Add,
                [""] = Motions.Sub,
            },
            ValueType = Types.Variable,
            ValuePartIndexOrderMap =
            {
                [0] = 0,
                [1] = 2,
            }
        };
        XmlIO.SaveToXml(new FindFormatPattern() { _patterns = { pattern } }, "test.xml");
    }

    public XmlSchema? GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        XmlHelper.ReadCollection(reader, _patterns, "Patterns", "Pattern", r =>
        {
            var pattern = new MatchPattern();
            pattern.ReadXml(r, "Pattern");
            return pattern;
        });
    }

    public void WriteXml(XmlWriter writer) =>
        XmlHelper.WriteCollection(writer, _patterns, "Pattern", (w, pattern) => pattern.WriteXml(w));
}