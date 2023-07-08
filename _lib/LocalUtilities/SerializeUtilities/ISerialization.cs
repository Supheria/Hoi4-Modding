namespace LocalUtilities.SerializeUtilities;

public interface ISerialization<T>
{
    T? Source { get; set; }

    string LocalRootName { get; }
}