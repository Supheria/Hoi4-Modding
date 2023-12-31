using FocusTree.IO.Csv;
using FocusTree.IO.Xml;
using FocusTree.Model.Lattice;
using LocalUtilities.ManageUtilities;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.StringUtilities;
using System.Diagnostics.CodeAnalysis;

namespace FocusTree.Model.Focus
{
    public class FocusGraph : IHistoryRecordable, IFileBackupManageable
    {
        /// <summary>
        /// 以 ID 作为 Key 的所有节点
        /// </summary>
        private Dictionary<int, FocusNode> _focusNodesMap;

        /// <summary>
        /// 国策列表
        /// </summary>
        public FocusNode[] FocusNodes => _focusNodesMap.Values.ToArray();

        /// <summary>
        /// 通过国策 ID 获得国策（不应该滥用，仅用在 require id 获取国策时），或修改国策
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FocusNode this[int id]
        {
            get => _focusNodesMap[id];
            set => _focusNodesMap[id] = value;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; }

        public int BranchNumber => FocusGraphUtilities.GetAllRootNodesBranches(ref _focusNodesMap).Count;

        public FocusGraph(string name)
        {
            Name = name is "" ? "unknown" : name;
            _focusNodesMap = new();
        }

        public FocusGraph(string name, FocusNode[] focusNodes) : this(name)
        {
            foreach (var node in focusNodes)
                _focusNodesMap[node.Id] = node;
        }

        public FocusGraph(string name, List<CsvFocusData> focusData) : this(name)
        {
            foreach (var data in focusData)
                _focusNodesMap[data.Id] = FocusGraphUtilities.CsvFocusDataConverter(data);
        }

        public FocusGraph() : this("")
        {
        }

        /// <summary>
        /// 删除节点 O(2n+)，绘图时记得重新调用 GetFocusMap
        /// </summary>
        /// <returns>是否成功删除</returns>
        public void RemoveNode(int id)
        {
            if (!_focusNodesMap.ContainsKey(id))
                return;
            // 在所有的节点依赖组合中删除此节点
            foreach (var require in _focusNodesMap.Values.SelectMany(focus => focus.Requires))
                require.Remove(id);
            // 从节点表中删除此节点
            _focusNodesMap.Remove(id);
        }

        /// <summary>
        /// 判断给定栅格化坐标是否存在于节点列表中
        /// </summary>
        /// <param name="latticedPoint"></param>
        /// <param name="focus"></param>
        /// <returns>如果有则返回true，id为节点id；否则返回false，id为-1</returns>
        public bool ContainLatticedPoint(LatticedPoint latticedPoint, [NotNullWhen(true)] out FocusNode? focus)
        {
            focus = null;
            foreach (var f in _focusNodesMap.Values.Where(f => latticedPoint == f.LatticedPoint))
            {
                focus = f;
                return true;
            }
            return false;
        }

        public bool ContainLatticedPoint(LatticedPoint latticedPoint) =>
            _focusNodesMap.Values.Any(f => latticedPoint == f.LatticedPoint);

        /// <summary>
        /// 获取某个节点的所有分支
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="sort">是否按照节点ID排序</param>
        /// <param name="reverse">是否从根节点向末节点排序</param>
        /// <returns></returns>
        public List<int[]> GetBranches(int id, bool sort, bool reverse) => GetBranches(new[] { id }, sort, reverse);

        /// <summary>
        /// 获取若干个节点各自的所有分支
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="sort">是否按照节点ID排序</param>
        /// <param name="reverse">是否从根节点向末节点排序</param>
        /// <returns></returns>
        public List<int[]> GetBranches(int[] ids, bool sort, bool reverse)
        {
            var focusNodes = FocusNodes;
            return FocusGraphUtilities.GetBranches(ref focusNodes, ref ids, sort, reverse);
        }

        /// <summary>
        /// 重置所有节点的元坐标
        /// 合并不同分支上的相同节点，并使节点在分支范围内尽量居中
        /// </summary>
        /// <returns></returns>
        public void AutoSetAllNodesPosition() => FocusGraphUtilities.AutoSetAllNodesLatticedPoint(ref _focusNodesMap);

        /// <summary>
        /// 按分支顺序从左到右、从上到下重排节点ID
        /// </summary>
        public void ReorderNodeIds() => FocusGraphUtilities.AutoSetAllNodesIdInOrder(ref _focusNodesMap);

        /// <summary>
        /// 获得整图元坐标矩形
        /// </summary>
        /// <returns></returns>
        public Rectangle GetMetaRect()
        {
            var focusNodes = FocusNodes;
            return FocusGraphUtilities.GetNodesLatticedRect(ref focusNodes);
        }

        //
        // interface
        //

        public string FileManageDirName => $"FG{Name.ToMd5HashString()}";

        private string CachePath => this.GetCachePath("hash test");

        public string GetHashString()
        {
            this.SaveToXml(CachePath, new FocusGraphXmlSerialization());
            using var data = new FileStream(CachePath, FileMode.Open);
            return data.ToMd5HashString();
        }

        public string GetHashStringFromFilePath(string filePath)
        {
            new FocusGraphXmlSerialization().LoadFromXml(filePath)
                ?.SaveToXml(CachePath, new FocusGraphXmlSerialization());
            using var data = new FileStream(CachePath, FileMode.Open);
            return data.ToMd5HashString();
        }

        public int HistoryIndex { get; set; }

        public int CurrentHistoryLength { get; set; }

        public FormattedData[] History { get; set; } = new FormattedData[20];

        public int LatestIndex { get; set; }

        public FormattedData ToFormattedData()
        {
            var hashString = this.GetHashString();
            if (!Directory.Exists(hashString))
                this.SaveToXml(this.GetCachePath(hashString), new FocusGraphXmlSerialization());
            return new(hashString);
        }

        public void FromFormattedData(FormattedData data) => _focusNodesMap =
            new FocusGraphXmlSerialization().LoadFromXml(this.GetCachePath(data.Items[0]))?._focusNodesMap ?? new();
    }
}
