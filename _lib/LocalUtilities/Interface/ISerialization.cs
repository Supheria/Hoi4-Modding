namespace LocalUtilities.Interface;

public interface ISerialization<T>
{
    T? Source { get; set; }

    string LocalRootName { get; }
}