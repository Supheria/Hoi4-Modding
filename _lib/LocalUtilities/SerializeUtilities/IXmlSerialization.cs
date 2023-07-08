using System.Xml.Serialization;

namespace LocalUtilities.SerializeUtilities;

public interface IXmlSerialization<T> : IXmlSerializable, ISerialization<T>
{
}