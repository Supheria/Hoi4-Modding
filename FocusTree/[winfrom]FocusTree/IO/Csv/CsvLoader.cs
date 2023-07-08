using CSVFile;
using FocusTree.Model.Focus;

namespace FocusTree.IO.Csv
{
    internal class CsvLoader
    {
        /// <summary>
        /// 从 xml 文件中反序列化 FGraph
        /// </summary>
        /// <param name="path">xml文件路径</param>
        /// <param name="focusGraph"></param>
        /// <returns>FGraph</returns>
        public static string LoadFromCsv(string path, out FocusGraph? focusGraph)
        {
            focusGraph = null;
            try
            {
                var nodes = new List<CsvFocusData>();
                ReadGraphFromCsv(path, nodes);
                focusGraph = new(Path.GetFileNameWithoutExtension(path), nodes);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 读取Csv文件，获得 string[][] 二维文本
        /// </summary>
        /// <param name="filePath">Csv文件路径</param>
        /// <returns>string[][] csv纯文本表格</returns>
        private static IEnumerable<string[]> ReadCsv(string filePath)
        {
            // csv 读取设置
            var settings = new CSVSettings
            {
                HeaderRowIncluded = false // 第一行不做标头
            };

            // 返回读取结果
            string[][] data;
            try
            {
                var csvData = CSVReader.FromFile(filePath, settings);
                data = csvData.ToArray(); //作为二维数组返回
                csvData.Dispose();
            }
            catch (Exception ex)
            {
                throw new($"[2302152043] 读取CSV文件失败：{ex.Message}");
            }
            return data;
        }

        private static void ReadGraphFromCsv(string path, List<CsvFocusData> nodes)
        {
            var data = ReadCsv(path);

            // 上一次循环处理的节点 <id>
            CsvTreeNode<int>? last = null;

            // 循环处理到的行数
            var rowCount = 0;
            // 遍历所有行
            foreach (var row in data)
            {
                //行数从1开始
                rowCount++;
                // 获取该行非空列的所在位置
                // 从头循环匹配所有为空并统计总数，数量就是第一个非空的index
                var level = row.TakeWhile(string.IsNullOrWhiteSpace).Count();
                // 获取原始字段
                CsvFocusData focusData;
                try
                {
                    focusData = new(rowCount, row[level]);
                }
                catch (Exception ex)
                {
                    throw new($"无法读取第{rowCount}行原始字段，{ex.Message}");
                }

                //== 转换 ==//

                // 节点是其中一个根节点时 
                if (last == null)
                {
                    last = new CsvTreeNode<int>(rowCount, 0);
                    nodes.Add(focusData);
                    // 这里不需要添加它的 Require
                    continue; // 这个要加的
                }
                // 如果新节点与上一节点的右移距离大于1，则表示产生了断层
                if (level > last.Level + 1)
                    throw new Exception($"位于 {rowCount} 行: 本行节点与上方节点的层级有断层。");
                // 如果新节点与上一节点的右移距离等于1，则新节点是上一节点的子节点
                if (level == last.Level + 1)
                {
                    var newNode = new CsvTreeNode<int>(focusData.Id, last.Level + 1); // 新节点

                    newNode.SetParent(last);

                    //添加新节点并创建依赖
                    nodes.Add(focusData);
                    focusData.Requires.Add(new() { last.Value });

                    last = newNode;
                }
                // 如果新节点与上一节点在同列或更靠左，向上寻找新节点所在列的父节点
                else
                {
                    do
                    {
                        // 已经没有更上级的节点，当前节点就是顶级节点
                        last = last.Parent; // lastNode指向自己的父节点
                    } // 当指向的父节点是新节点所在列的父节点时结束循环
                    while (last is not null && level - 1 != last.Level);
                    // 这个是同级节点
                    var newNode = new CsvTreeNode<int>(rowCount, level); // lastNode指向新的节点
                    nodes.Add(focusData);
                    if (last is null) // 新节点是根节点时直接添加
                    {
                        last = newNode;
                    }
                    else // 新节点有父节点
                    {
                        newNode.SetParent(last);
                        focusData.Requires.Add(new() { last.Value });
                        last = newNode;
                    }
                }
            }
        }
    }
}
