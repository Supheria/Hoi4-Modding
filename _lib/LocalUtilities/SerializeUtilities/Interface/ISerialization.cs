namespace LocalUtilities.SerializeUtilities.Interface;

public interface ISerialization<T>
{
    T? Source { get; set; }

    string LocalRootName { get; }
}