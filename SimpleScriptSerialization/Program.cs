using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeBundle;
using LocalUtilities.UIUtilities;
using System.Diagnostics;

namespace SimpleScriptSerialization
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var rect = new Point(1, 2);
            var s = rect.ToArrayString();
            rect = s.ToPoint(new());
            var dic = new Dictionary<string, string>() { ["a "] = "a", ["b , "] = "b" };
            var d = new TestDictionary("test");
            d.Pairs.AddRange(dic.ToList());
            d.SaveToSimpleScript(true);

            var a = new TestDictionary("test").LoadFromSimpleScript();
            var newdic = a.Pairs.ToDictionary();

            var watch = new Stopwatch();
            watch.Start();
            var data = new TestFormData("ShitForm").LoadFromSimpleScript();
            watch.Stop();
            var time = watch.ElapsedMilliseconds;
            data.SaveToSimpleScript(true);

            Application.Run(new Form1());
        }
    }

    public class TestDictionary(string localName) : KeyValuePairs<string, string>()
    {
        public override string LocalName { get; set; } = localName;

        protected override Func<string, string> ReadKey => str => str;

        protected override Func<string, string> ReadValue => str => str;

        protected override Func<string, string> WriteKey => str => str;

        protected override Func<string, string> WriteValue => str => str;
    }

    public class TestFormData(string localName) : FormData(localName)
    {
        public TestFormData() : this(nameof(TestFormData))
        {

        }

        public override Size MinimumSize { get; set; }

        public List<TestFormData> Datas { get; set; } = [];

        protected override void SerializeFormData(SsSerializer serializer)
        {
            serializer.Serialize(Datas);
        }

        protected override void DeserializeFormData(SsDeserializer deserializer)
        {
            deserializer.Deserialize(new(LocalName), Datas);
        }
    }
}