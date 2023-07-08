namespace FocusTree.IO.Csv;

internal class CsvTreeNode<T>
{
    public T Value;
    public int Level;
    public CsvTreeNode<T>? Parent { get; set; }
    public HashSet<CsvTreeNode<T>> Children { get; private set; } = new();
    public CsvTreeNode(T value, int level)
    {
        Value = value;
        Level = level;
    }
    public void SetParent(CsvTreeNode<T> parent)
    {
        Parent = parent;
        Parent.Children.Add(this);
    }
}