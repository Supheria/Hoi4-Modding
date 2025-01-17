﻿using FocusTree.IO.Csv;
using FocusTree.IO.Xml;
using LocalUtilities.FileUtilities;

namespace FocusTree.Model.Focus;

partial class FocusGraph
{
    private static FocusNode CsvFocusDataConverter(CsvFocusData data)
    {
        return new() {
            SetSignature = data.Id,
            Name = data.Name,
            Duration = data.Duration,
            Description = data.Description,
            Ps = data.Ps,
            BeginWithStar = data.BeginWithStar,
            RawEffects = data.RawEffects,
            Requires = data.Requires,
        };
    }

    /// <summary>
    /// 获得整图元坐标矩形
    /// </summary>
    /// <returns></returns>
    public Rectangle GetGraphLatticedRect()
    {
        int top, right, bottom;
        var left = top = right = bottom = 0;
        foreach (var point in RosterList.Select(focus => focus.LatticedPoint))
        {
            if (point.Col < left) { left = point.Col; }
            else if (point.Col > right) { right = point.Col; }
            if (point.Row < top) { top = point.Row; }
            else if (point.Row > bottom) { bottom = point.Row; }
        }
        return new(left, top, right - left + 1, bottom - top + 1);
    }

    /// <summary>
    /// 所有节点的子链接（使用前调用 CreateNodeLinks ）
    /// </summary>
    public Dictionary<int, List<int>> GetNodeLinksMap()
    {
        var nodeLinksMap = new Dictionary<int, List<int>>();
        foreach (var node in RosterList)
            foreach (var id in node.Requires.SelectMany(requires =>
                         requires.Where(id => !nodeLinksMap.TryAdd(id, new() { node.Signature }))))
                nodeLinksMap[id].Add(node.Signature);
        return nodeLinksMap;
    }

    public List<int[]> GetAllRootNodesBranches()
    {
        var rootIds = GetRootNodeIds();
        return GetBranches(rootIds, true, true);
    }

    /// <summary>
    /// 获取所有无任何依赖的节点（根节点）  O(n)
    /// </summary>
    /// <returns>根节点</returns>
    public int[] GetRootNodeIds()
    {
        var result = new HashSet<int>();
        foreach (var focus in RosterList.Where(focus => focus.Requires.Sum(x => x.Count) == 0))
            result.Add(focus.Signature);
        return result.ToArray();
    }

    /// <summary>
    /// 获取若干个节点各自的所有分支
    /// </summary>
    /// <param name="focusNodes"></param>
    /// <param name="ids"></param>
    /// <param name="sort">是否按照节点ID排序</param>
    /// <param name="reverse">是否从根节点向末节点排序</param>
    /// <returns></returns>
    public List<int[]> GetBranches(int[] ids, bool sort, bool reverse)
    {
        var branches = new List<int[]>();
        var steps = new Stack<int>();
        foreach (var id in ids)
            GetBranches(id, ref branches, ref steps, sort, reverse);
        return branches;
    }

    private void GetBranches(int currentId, ref List<int[]> branches, ref Stack<int> steps, bool sort, bool reverse)
    {
        steps.Push(currentId);
        GetNodeLinksMap().TryGetValue(currentId, out var links);
        // 当前节点是末节点
        if (links == null)
            branches.Add(reverse ? steps.Reverse().ToArray() : steps.ToArray());
        else
        {
            var linkList = links.ToList();
            if (sort)
                linkList.Sort();
            foreach (var id in linkList)
                if (!steps.Contains(id))
                    GetBranches(id, ref branches, ref steps, sort, reverse);
        }

        steps.Pop();
    }

    /// <summary>
    /// 重置所有节点的元坐标
    /// 合并不同分支上的相同节点，并使节点在分支范围内尽量居中
    /// </summary>
    /// <returns></returns>
    public void AutoSetAllNodesLatticedPoint()
    {
        var branches = GetAllRootNodesBranches();
        if (branches.Count == 0) { return; }
        var width = branches.Count;
        var height = branches.Max(x => x.Length);
        Dictionary<int, int[]> nodeCoordinates = new();
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (y >= branches[x].Length)
                {
                    continue;
                }
                var id = branches[x][y];
                if (nodeCoordinates.ContainsKey(id))
                {
                    nodeCoordinates[id][0] = nodeCoordinates[id][0] < x ? nodeCoordinates[id][0] : x;
                    nodeCoordinates[id][1] = nodeCoordinates[id][1] > x ? nodeCoordinates[id][1] : x;
                    nodeCoordinates[id][2] = nodeCoordinates[id][2] > y ? nodeCoordinates[id][2] : y;
                }
                else
                {
                    nodeCoordinates.Add(id, new int[3] { x, x, y }); // [0]起始x, [1]终止x, [2]y
                }
            }
        }
        Dictionary<int, Point> metaPoints = new();
        foreach (var coordinate in nodeCoordinates)
        {
            var x = coordinate.Value[0] + (coordinate.Value[1] - coordinate.Value[0]) / 2;
            Point point = new(x, coordinate.Value[2]);
            metaPoints[coordinate.Key] = point;
        }

        if (metaPoints.Count == 0)
            return;
        // 清除横坐标之间无节点的间隙
        // 集合相同x值的元坐标
        Dictionary<int, Dictionary<int, Point>> xMetaPoints = new();
        foreach (var pair in metaPoints)
        {
            var x = pair.Value.X;
            if (xMetaPoints.ContainsKey(x) == false)
            {
                xMetaPoints.Add(x, new());
            }
            xMetaPoints[x].Add(pair.Key, pair.Value);
        }
        var blank = 0;
        width = metaPoints.Max(x => x.Value.X);
        for (var x = 0; x <= width; x++)
        {
            if (xMetaPoints.TryGetValue(x, out var metaPoint))
            {
                foreach (var nodePoint in metaPoint)
                    RosterMap[nodePoint.Key].LatticedPoint = new()
                    {
                        Col = nodePoint.Value.X - blank,
                        Row = nodePoint.Value.Y
                    };
            }
            else { blank++; }
        }
    }

    /// <summary>
    /// 按分支顺序从左到右、从上到下重排节点ID
    /// </summary>
    public void AutoSetAllNodesIdInOrder()
    {
        var branches = GetAllRootNodesBranches();
        if (branches.Count is 0)
            return;
        Dictionary<int, FocusNode> tempFocusCatalog = new();
        HashSet<int> visited = new();
        var newId = 1;
        foreach (var id in from branch in branches from id in branch where !visited.Contains(id) select id)
        {
            UpdateLinkNodesRequiresWithNewId(id, newId);
            visited.Add(id);
            RosterMap[id].SetSignature = newId;
            tempFocusCatalog.Add(newId, RosterMap[id]);
            newId++;
        }
        RosterMap = tempFocusCatalog;
    }

    /// <summary>
    /// 按新节点id更新原节点的所有子链接节点的依赖组
    /// </summary>
    /// <param name="focusNodesMap"></param>
    /// <param name="id"></param>
    /// <param name="newId"></param>
    /// <returns></returns>
    private void UpdateLinkNodesRequiresWithNewId(int id, int newId)
    {
        if (!GetNodeLinksMap().TryGetValue(id, out var links))
            return;
        foreach (var linkId in links)
        {
            List<HashSet<int>> newRequires = new();
            foreach (var require in RosterMap[linkId].Requires)
            {
                HashSet<int> newRequire = new();
                foreach (var requireId in require)
                {
                    newRequire.Add(requireId == id ? newId : requireId);
                }
                newRequires.Add(newRequire);
            }
            var focus = RosterMap[linkId];
            focus.Requires = newRequires;
            RosterMap[linkId] = focus;
        }
    }

    public static string? LoadFromFile(string filePath, out FocusGraph? focusGraph)
    {
        var extension = Path.GetExtension(filePath).ToLower();
        switch (extension)
        {
            case ".csv":
                return CsvLoader.LoadFromCsv(filePath, out focusGraph);
            default:
                focusGraph = new FocusGraphXmlSerialization().LoadFromXml(out var message, filePath);
                return message;
        }
    }
}