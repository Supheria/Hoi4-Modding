namespace test;

public class Program
{
    public static void Main()
    {
        var path = "test.yml";
        var test = new TestYaml()
        {
            Info = new()
            {
                new()
                {
                    Ipv4 = "pc1",
                    Port = 12345
                },
                new()
                {
                    Ipv4 = "pc2",
                    Port = 12345
                },
                new()
                {
                    Ipv4 = "pc3",
                    Port = 12345
                },
            },
            Args = new()
            {
                "这是1",
                "这是2",
                "这是3",

            }

        };
        test.SaveToYaml(path);
        var a = YamlSaverLoader.LoadFromYaml<TestYaml>(path);

        Console.ReadKey();
    }
}