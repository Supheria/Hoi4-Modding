using LocalUtilities.DelegateUtilities;

namespace FocusTree.Model.WinFormGdiUtilities
{
    /// <summary>
    /// 绘制委托类型
    /// </summary>
    /// <param name="image"></param>
    public delegate void Drawer(Bitmap image);
    /// <summary>
    /// 绘制层级
    /// </summary>
    public class DrawLayers
    {
        Drawer?[] Layers { get; set; }

        public int LayerNumber => Layers.Length;
        /// <summary>
        /// 创建给定层级数的 Drawer 数组
        /// </summary>
        /// <param name="layerNumber"></param>
        public DrawLayers(uint layerNumber)
        {
            Layers = new Drawer[layerNumber];
        }
        /// <summary>
        /// 按层级序号顺序激发所有层级的委托方法
        /// </summary>
        /// <param name="image"></param>
        public void Invoke(Bitmap image) => Layers.ToList().ForEach(x => x?.Invoke(image));
        /// <summary>
        /// 激发指定层级的委托
        /// </summary>
        /// <param name="image"></param>
        /// <param name="layerIndex"></param>
        public void Invoke(uint layerIndex, Bitmap image) => Layers[layerIndex]?.Invoke(image);
        /// <summary>
        /// 清空所有层级的委托
        /// </summary>
        public void Clear() => Layers = Layers.Select(x => x.RemoveAllInvocations()).ToArray();
        /// <summary>
        /// 清空指定层级的委托
        /// </summary>
        /// <param name="layerIndex"></param>
        public void Clear(uint layerIndex) => Layers[layerIndex] = Layers[layerIndex].RemoveAllInvocations();
        /// <summary>
        /// 所有层级的委托方法个数
        /// </summary>
        public int MethodNumber() => Layers.Sum(x => x?.GetInvocationList().Length ?? 0);
        /// <summary>
        /// 指定层级的委托方法个数
        /// </summary>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public int MethodNumber(uint layerIndex) => Layers[layerIndex]?.GetInvocationList().Length ?? 0;

        /// <summary>
        /// 添加 drawer 到指定层级
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="layerDrawer"></param>
        /// <returns></returns>
        public static DrawLayers operator +(DrawLayers layer, (uint, Drawer) layerDrawer)
        {
            layer.Layers[layerDrawer.Item1] += layerDrawer.Item2;
            return layer;
        }
    }
}
