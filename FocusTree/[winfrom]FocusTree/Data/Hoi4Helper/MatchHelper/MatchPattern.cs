using FocusTree.IO;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using static FocusTree.Data.Hoi4Helper.PublicSign;

namespace FocusTree.Data.Hoi4Helper.MatchHelper;

public class MatchPattern
{
    public Types TriggerType { get; set; } = Types.None;

    public string TriggerPattern { get; set; } = "";

    public string MotionPattern { get; set; } = "";

    public int MotionPartIndex { get; set; } = -1;

    public Dictionary<string, Motions> MotionConditionMap { get; } = new() { [""] = Motions.None };

    public Types ValueType { get; set; } = Types.None;

    public Dictionary<int, uint> ValuePartIndexOrderMap { get; } = new();

    public void ReadXml(XmlReader reader, string nodeName)
    {
        TriggerPattern = reader.GetAttribute("Trigger") ?? "";
        MotionPattern = reader.GetAttribute("Motion") ?? "";

        do
        {
            if (reader.Name == nodeName && reader.NodeType is XmlNodeType.EndElement)
                break;
            switch (reader.Name)
            {
                case "TriggerType" when reader.NodeType is XmlNodeType.Element:
                    reader.Read();
                    TriggerType = XmlHelper.GetEnumValue<Types>(reader.Value) as Types? ?? Types.None;
                    continue;
                case "Motion" when reader.NodeType is XmlNodeType.Element:
                    reader.Read();
                    MotionConditionMap[""] = XmlHelper.GetEnumValue<Motions>(reader.Value) as Motions? ?? Motions.None;
                    continue;
                case "MotionConditionMap":
                    MotionPartIndex = XmlHelper.GetIntValue(reader.GetAttribute("MotionPartIndex"));
                    XmlHelper.ReadCollection(reader, MotionConditionMap, "MotionConditionMap", "Item",
                        r => (r.GetAttribute("Condition") ?? "",
                            XmlHelper.GetEnumValue<Motions>(reader.GetAttribute("Motion")) as Motions? ?? Motions.None));
                    continue;
                case "ValuePartIndexOrderMap":
                    ValueType = XmlHelper.GetEnumValue<Types>(reader.GetAttribute("ValueType")) as Types? ?? Types.None;
                    XmlHelper.ReadCollection(reader, ValuePartIndexOrderMap, "ValuePartIndexOrderMap", "Item",
                        r => (XmlHelper.GetIntValue(r.GetAttribute("Order")),
                            XmlHelper.GetUintValue(r.GetAttribute("PartIndex"))));
                    continue;
            }
        } while (reader.Read());
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Trigger", TriggerPattern);
        writer.WriteAttributeString("Motion", MotionPattern);

        writer.WriteElementString("TriggerType", TriggerType.ToString());

        if (MotionPartIndex is -1)
            writer.WriteElementString("Motion", MotionConditionMap.First().Value.ToString());
        else
        {
            // <MotionConditionMap>
            writer.WriteStartElement("MotionConditionMap");
            writer.WriteAttributeString("MotionPartIndex", MotionPartIndex.ToString());
            XmlHelper.WriteCollection(writer, MotionConditionMap, "Item", (w, item) =>
            {
                w.WriteAttributeString("Condition", item.Key);
                w.WriteAttributeString("Motion", item.Value.ToString());
            });
            // </MotionConditionMap>
            writer.WriteEndElement();
        }

        if (ValueType is Types.None)
            return;
        // <ValuePartIndexOrderMap>
        writer.WriteStartElement("ValuePartIndexOrderMap");
        writer.WriteAttributeString("ValueType", ValueType.ToString());
        XmlHelper.WriteCollection(writer, ValuePartIndexOrderMap, "Item", (w, item) =>
        {
            w.WriteAttributeString("Order", item.Key.ToString());
            w.WriteAttributeString("PartIndex", item.Value.ToString());
        });
        // </ValuePartIndexOrderMap>
        writer.WriteEndElement();
    }
}