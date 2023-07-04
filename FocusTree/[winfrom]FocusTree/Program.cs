// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
//#define MAIN
using FocusTree.UI.Graph;
using FocusTree.Utilities.test;

internal static class Program
{
    public static TestInfo TestInfo { get; } = new();

    /// <summary>
    /// 应用程序的主入口点。
    /// </summary>
    [STAThread]
    private static void Main()
    {
        //testInfo.Show();
        Application.Run(new GraphForm());
    }
}