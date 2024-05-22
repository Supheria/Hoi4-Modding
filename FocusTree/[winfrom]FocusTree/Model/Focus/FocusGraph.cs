using FocusTree.IO.Csv;
using LocalUtilities.FileHelper;
using LocalUtilities.SimpleScript.Serialization;
using LocalUtilities.TypeGeneral;
using LocalUtilities.TypeToolKit.Text;
using System.Diagnostics.CodeAnalysis;

namespace FocusTree.Model.Focus
{
    public partial class FocusGraph : Roster<int, FocusNode>, IHistoryRecordable
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }

        public FocusGraph(string name)
        {
            Name = name is "" ? "unknown" : name;
        }

        public FocusGraph(string name, List<CsvFocusData> focusData) : this(name)
        {
            focusData.ForEach(x => Add(CsvFocusDataConverter(x)));
        }

        public FocusGraph() : this("")
        {
        }

        /// <summary>
        /// 删除节点 O(2n+)，绘图时记得重新调用 GetFocusMap
        /// </summary>
        /// <returns>是否成功删除</returns>
        public override void Remove(int id)
        {
            if (!RosterMap.ContainsKey(id))
                return;
            // 在所有的节点依赖组合中删除此节点
            foreach (var require in RosterMap.Values.SelectMany(focus => focus.Require))
                require.Remove(id);
            // 从节点表中删除此节点
            RosterMap.Remove(id);
        }

        /// <summary>
        /// 判断给定栅格化坐标是否存在于节点列表中
        /// </summary>
        /// <param name="latticedPoint"></param>
        /// <param name="focus"></param>
        /// <returns>如果有则返回true，id为节点id；否则返回false，id为-1</returns>
        public bool ContainLatticedPoint(LatticePoint latticedPoint, [NotNullWhen(true)] out FocusNode? focus)
        {
            focus = null;
            foreach (var f in RosterMap.Values.Where(f => latticedPoint == f.LatticedPoint))
            {
                focus = f;
                return true;
            }
            return false;
        }

        public bool ContainLatticedPoint(LatticePoint latticedPoint)
        {
            return RosterMap.Values.Any(f => latticedPoint == f.LatticedPoint);
        }

        public string FileManageDirName => $"FG{Name.ToMd5HashString()}";

        public int CurrentHistoryIndex { get; set; }

        public int CurrentHistoryLength { get; set; }

        public string[] HistoryCache { get; set; } = new string[20];

        public int LastSavedIndex { get; set; }

        public string HashCachePath => this.GetCacheFilePath("hash test");

        public string ToHashString()
        {
            this.SaveToSimpleScript(false, HashCachePath);
            using var data = new FileStream(HashCachePath, FileMode.Open);
            var hashString = data.ToMd5HashString(); ;
            if (!File.Exists(hashString))
                this.SaveToSimpleScript(false, this.GetCacheFilePath(hashString));
            return new(hashString);
        }

        public string ToHashString(string filePath)
        {
            var focusGraph = this.LoadFromSimpleScript();
            focusGraph.SaveToSimpleScript(false, HashCachePath);
            using var data = new FileStream(HashCachePath, FileMode.Open);
            return data.ToMd5HashString();
        }

        public void FromHashString(string data)
        {
            this.LoadFromSimpleScript(this.GetCacheFilePath(data));
        }

        public override string LocalName { get; set; } = "NationalFocus";

        protected override void SerializeRoster(SsSerializer serializer)
        {
            serializer.WriteTag(nameof(Name), Name);
        }

        protected override void DeserializeRoster(SsDeserializer deserializer)
        {
            Name = deserializer.ReadTag(nameof(Name), s => s ?? Name);
        }
    }
}
