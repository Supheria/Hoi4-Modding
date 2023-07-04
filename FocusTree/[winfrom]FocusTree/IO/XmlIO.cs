using System.Xml.Serialization;

namespace FocusTree.IO
{
    public static class XmlIO
    {
        /// <summary>
        /// 将 FGraph 序列化成 xml
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path">保存路径</param>
        public static void SaveToXml<T>(T? obj, string path) where T : IXmlSerializable
        {
            if (obj is null)
                return;
            var file = File.Create(path);
            var writer = new XmlSerializer(typeof(T));
            writer.Serialize(file, obj);
            file.Close();
        }
        /// <summary>
        /// 从 xml 文件中反序列化 FGraph
        /// </summary>
        /// <param name="path">xml文件路径</param>
        /// <returns>FGraph</returns>
        public static T? LoadFromXml<T>(string path) where T : IXmlSerializable
        {
            try
            {
                if (!File.Exists(path))
                    return default;
                var file = File.OpenRead(path);
                var reader = new XmlSerializer(typeof(T));
                var obj = reader.Deserialize(file);
                file.Close();
                return obj is null ? default : (T)obj;
            }
            catch
            {
                return default;
            }
        }
    }
}
