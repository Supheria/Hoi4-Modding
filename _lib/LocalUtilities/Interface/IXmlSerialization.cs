using System.Xml.Serialization;

namespace LocalUtilities.Interface;

public interface IXmlSerialization<T> : IXmlSerializable, ISerialization<T>
{
}