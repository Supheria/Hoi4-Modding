using System.Xml;
using System.Xml.Serialization;

namespace LocalUtilities.Interface;

public interface IXmlSerialization<T> : IXmlSerializable, ISerialization<T>
{
    public void WriteXmlComment(XmlWriter writer)
    {
    }
}