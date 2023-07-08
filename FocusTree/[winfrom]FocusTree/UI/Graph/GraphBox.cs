using FocusTree.IO.Csv;
using FocusTree.IO.Xml;
using FocusTree.Model.Focus;
using FocusTree.Model.Lattice;
using LocalUtilities.ManageUtilities;
using System.Diagnostics.CodeAnalysis;

namespace FocusTree.UI.Graph
{
    /// <summary>
    /// 封装给 UI 公用的
    /// </summary>
    public static class GraphBox
    {
        /// <summary>
        /// UI 不应该直接调用此对象的方法，而应该使用静态类封装好的
        /// </summary>
        public static FocusGraph? Graph { get; private set; }

        /// <summary>
        /// 元图文件路径
        /// </summary>
        public static string FilePath { get; private set; } = "";

        /// <summary>
        /// 是否只读（文件路径在备份文件夹）
        /// </summary>
        public static bool ReadOnly { get; private set; }

        /// <summary>
        /// 元图带上只读和未保存后缀的名称
        /// </summary>
        public static string Name
        {
            get
            {
                if (Graph is null)
                    return "Focus Tree";
                else if (ReadOnly)
                    return Graph.Name + "（只读）";
                else if (Graph.IsEdit())
                    return Graph.Name + "（未保存）";
                else
                    return Graph.Name;
            }
        }

        /// <summary>
        /// 是否已编辑
        /// </summary>
        public static bool Edited => Graph?.IsEdit() ?? false;

        /// <summary>
        /// 是否有向前的历史记录
        /// </summary>
        public static bool HasPrevHistory => Graph != null && Graph.HasPrevHistory();

        /// <summary>
        /// 是否有向后的历史记录
        /// </summary>
        public static bool HasNextHistory => Graph != null && Graph.HasNextHistory();

        /// <summary>
        /// 元图的国策列表
        /// </summary>
        public static FocusNode[] FocusList => Graph is null ? Array.Empty<FocusNode>() : Graph.FocusNodes;

        /// <summary>
        /// 元图节点数量
        /// </summary>
        public static int NodeCount => Graph is null ? 0 : Graph.FocusNodes.Length;
        /// <summary>
        /// 元图分支数量
        /// </summary>
        public static int BranchCount => Graph?.BranchNumber ?? 0;
        /// <summary>
        /// 元图备份列表
        /// </summary>
        /// <returns></returns>
        public static List<(string, string)> BackupList => Graph is null ? new() : Graph.GetBackupsList(FilePath);

        /// <summary>
        /// 元图元坐标矩形
        /// </summary>
        public static Rectangle MetaRect => Graph?.GetMetaRect() ?? new();
        /// <summary>
        /// 从文件路径加载元图，如果只读则封存文件路径
        /// </summary>
        /// <param name="filePath"></param>
        public static void Load(string filePath)
        {
            ReadOnly = Graph?.IsBackupFile(filePath) ?? false;
            if (!ReadOnly)
                FilePath = filePath;
            FileCacheManager.ClearCache(Graph);
            if (Path.GetExtension(filePath).ToLower() is ".csv")
                try
                {
                    Graph = CsvLoader.LoadFromCsv(filePath);
                }
                catch (Exception e)
                {
                    Graph = null;
                    Program.TestInfo.Append(e.Message);
                    Program.TestInfo.Show();
                }
            else
                Graph = new FocusXmlGraphSerialization().LoadFromXml(filePath);
            Graph?.NewHistory();
            Program.TestInfo.Renew();
        }

        /// <summary>
        /// 从封存文件路径重新加载元图（如果文件路径存在的话）
        /// </summary>
        public static void Reload()
        {
            if (!File.Exists(FilePath)) { return; }
            ReadOnly = false;
            FileCacheManager.ClearCache(Graph);
            if (Path.GetExtension(FilePath).ToLower() is ".csv")
                try
                {
                    Graph = CsvLoader.LoadFromCsv(FilePath);
                }
                catch (Exception e)
                {
                    Graph = null;
                    Program.TestInfo.Append(e.Message);
                    Program.TestInfo.Show();
                }
            else
                Graph = new FocusXmlGraphSerialization().LoadFromXml(FilePath);
            Graph?.NewHistory();
            Program.TestInfo.Renew();
        }
        /// <summary>
        /// 如果元图已修改，则备份源文件并保存到源文件
        /// </summary>
        public static void Save()
        {
            if (Graph is null)
                return;
            if (Path.GetExtension(FilePath).ToLower() is ".csv")
            {
                SaveToNew(Path.ChangeExtension(FilePath, ".xml"));
                return;
            }
            ReadOnly = false;
            Graph.Backup(FilePath);
            Graph.SaveToXml(FilePath, new FocusXmlGraphSerialization());
            Graph.UpdateLatest();
        }
        /// <summary>
        /// 将元图另存到新的文件路径（如果给定路径和静态文件路径相同，则执行备份和保存）
        /// </summary>
        /// <param name="filePath"></param>
        public static void SaveToNew(string filePath)
        {
            if (Graph is null)
                return;
            if (filePath == FilePath)
            {
                Save();
                return;
            }
            ReadOnly = false;
            FileCacheManager.ClearCache(Graph);
            Graph.SaveToXml(filePath, new FocusXmlGraphSerialization());
            Graph?.NewHistory();
            FilePath = filePath;
            Program.TestInfo.Renew();
        }
        /// <summary>
        /// 重做
        /// </summary>
        public static void Redo() => Graph?.Redo();
        /// <summary>
        /// 撤销
        /// </summary>
        public static void Undo() => Graph?.Undo();

        /// <summary>
        /// 从元图获取国策
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static FocusNode GetFocus(int id) => Graph?[id] ?? new();
        /// <summary>
        /// 修改元图国策（根据国策数据内的 ID 值索引）
        /// </summary>
        /// <param name="focus"></param>
        public static void SetFocus(FocusNode focus)
        {
            if (Graph == null) { return; }
            Graph[focus.Id] = focus;
            Graph.EnqueueHistory();
        }
        public static void RemoveFocusNode(FocusNode focus)
        {
            Graph?.RemoveNode(focus.Id);
            Graph?.EnqueueHistory();
        }
        /// <summary>
        /// 按分支顺序重排所有国策 ID
        /// </summary>
        public static void ReorderFocusNodesId()
        {
            Graph?.ReorderNodeIds();
            Graph?.EnqueueHistory();
        }
        /// <summary>
        /// 自动排版节点
        /// </summary>
        public static void AutoLayoutAllFocusNodes()
        {
            Graph?.AutoSetAllNodesPosition();
            Graph?.EnqueueHistory();
        }
        /// <summary>
        /// 元图包含给定栅格化坐标
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool ContainLatticedPoint(LatticedPoint point) => Graph != null && Graph.ContainLatticedPoint(point);

        /// <summary>
        /// 坐标是否处于任何国策节点的绘图区域中
        /// </summary>
        /// <returns>坐标所处于的节点id，若没有返回null</returns>
        public static bool PointInAnyFocusNode(Point point, [NotNullWhen(true)] out FocusNode? focus)
        {
            focus = null;
            if (Graph == null)
                return false;
            LatticeCell cell = new(new(point));
            if (!Graph.ContainLatticedPoint(cell.LatticedPoint, out focus))
                return false;
            var part = cell.GetPartPointOn(point);
            return part is LatticeCell.Parts.Node;
        }
        /// <summary>
        /// 删除当前备份
        /// </summary>
        public static void DeleteBackup()
        {
            if (MessageBox.Show("是否要删除当前备份？", "提示", MessageBoxButtons.YesNo) is DialogResult.Yes)
                Graph?.DeleteBackup();
        }
    }
}
