using LocalUtilities.FileUtilities;
using LocalUtilities.Serializations;
using LocalUtilities.SimpleScript.Data;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.StringUtilities;
using LocalUtilities.UIUtilities;
using System.Diagnostics;
using System.Text;

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
            //var dic = new Dictionary<string, string>() { ["a "] = "a"};
            //new TestDictionarySerialization("test") { Source = dic.ToList()}.SaveToFile(true);
            var a = new TestDictionarySerialization("test").LoadFromFile(out var m);
            var dic = a.ToDictionary();

            var watch = new Stopwatch();
            watch.Start();
            var data = new TestFormDataSerialization("ShitForm").LoadFromFile(out m);
            watch.Stop();
            var time = watch.ElapsedMilliseconds;
            new TestFormDataSerialization("ShitForm") { Source = data }.SaveToFile(false);

            Application.Run(new Form1());
        }
    }

    public class TestDictionarySerialization(string localName) : KeyValuePairsSerialization<string, string>()
    {
        public override string LocalName => localName;

        protected override Func<string, string> ReadKey => str => str;

        protected override Func<string, string> ReadValue => str => str;

        protected override Func<string, string> WriteKey => str => str;

        protected override Func<string, string> WriteValue => str => str;
    }

    public class TestFormData : FormData
    {
        public override Size MinimumSize { get; set; }

        public List<TestFormData> Datas { get; set; } = [];
    }

    public class TestFormDataSerialization : FormDataSerialization<TestFormData>
    {
        public TestFormDataSerialization(string localName) : base(localName, new())
        {
            OnSerialize += FormData_Serialize;
            OnDeserialize += FormData_Deserialize;
        }

        private void FormData_Serialize()
        {
            Serialize(Source.Datas, new TestFormDataSerialization(LocalName));
        }

        private void FormData_Deserialize()
        {
            Deserialize(LocalName, token =>
            {
                var itemSerialization = new TestFormDataSerialization(LocalName);
                if (itemSerialization.Deserialize(token))
                    Source.Datas.Add(itemSerialization.Source);
            });
        }
    }
}