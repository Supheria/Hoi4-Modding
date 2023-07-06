using System.Xml.Serialization;

namespace FocusTree.IO.FileManage
{
    public interface IBackupable : IFileManageable
    {
        /// <summary>
        /// 获取对象哈希值字符串
        /// </summary>
        /// <returns></returns>
        string GetHashString() => GetHashString(this.GetCachePath("hash test"));

        string GetHashString(string filePath);
    }
}
