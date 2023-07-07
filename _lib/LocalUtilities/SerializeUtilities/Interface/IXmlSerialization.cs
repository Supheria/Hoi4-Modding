using System.Xml.Serialization;

namespace LocalUtilities.SerializeUtilities.Interface;

public interface IXmlSerialization<T> : IXmlSerializable, ISerialization<T>
{
}