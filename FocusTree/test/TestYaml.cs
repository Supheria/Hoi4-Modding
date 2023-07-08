namespace test;

public class TestYaml
{
    public class PcClass
    {
        public string Ipv4 { get; set; } = "";

        public int Port { get; set; }
    }

    public List<PcClass> Info { get; set; } = new();

    public List<string> Args { get; set; } = new();
}