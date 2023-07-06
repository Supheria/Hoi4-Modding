namespace LocalUtilities.Interface;

public interface ISerialization<T>
{
    T Source { get; set; }

    string LocalName { get; }
}