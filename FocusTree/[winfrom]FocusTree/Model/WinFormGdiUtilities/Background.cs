using FocusTree.Properties;
using LocalUtilities.GraphUtilities;
using System.Drawing.Drawing2D;

namespace FocusTree.Model.WinFormGdiUtilities
{
    public static class Background
    {
        public static string BackImagePath { get; set; } = "Background.jpg";

        /// <summary>
        /// 当前的背景图片
        /// </summary>
        private static Bitmap? _image;

        /// <summary>
        /// 应该仅当使用 PointBitmap 时才调用此成员，不应对此有任何直接赋值的操作
        /// </summary>
        public static Bitmap BackImage => _image ?? InitializeImage();

        /// <summary>
        /// 背景图片的大小
        /// </summary>
        public static Size Size => BackImage.Size;

        /// <summary>
        /// 无图片背景
        /// </summary>
        public static Color BlankBackground { get; set; } = Color.WhiteSmoke;

        /// <summary>
        /// 是否显示背景图片
        /// </summary>
        public static bool Show { get; set; } = true;

        /// <summary>
        /// 新键背景缓存，并重绘背景
        /// </summary>
        /// <param name="image"></param>
        public static void DrawNewBackGround(this Image image)
        {
            SetImage(image.Size);
            image.RedrawBackGround();
        }

        /// <summary>
        /// 重绘背景（首次重绘应该使用 DrawNew）
        /// </summary>
        /// <param name="image"></param>
        public static void RedrawBackGround(this Image image)
        {
            var g = Graphics.FromImage(image);
            g.CompositingMode = CompositingMode.SourceCopy;
            g.CompositingQuality = CompositingQuality.Invalid;
            g.DrawImage(BackImage, 0, 0);
            g.Flush(); g.Dispose();
        }

        /// <summary>
        /// 初始化图片
        /// </summary>
        /// <returns></returns>
        private static Bitmap InitializeImage()
        {
            if (File.Exists(BackImagePath))
                _image = (Bitmap)Image.FromFile(BackImagePath);
            else
                _image = Resources.BackImage;
            return _image;
        }

        /// <summary>
        /// 根据图源重设背景图片为指定大小
        /// </summary>
        /// <param name="size"></param>
        private static void SetImage(Size size)
        {
            if (Show)
            {
                InitializeFromSourceImage(size);
                return;
            }
            var width = size.Width;
            var height = size.Height;
            _image?.Dispose();
            _image = new(width, height);
            PointBitmap pCache = new(_image);
            pCache.LockBits();
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    pCache.SetPixel(i, j, BlankBackground);
                }
            }
            pCache.UnlockBits();
        }
        /// <summary>
        /// 从图源设置背景
        /// </summary>
        /// <param name="size"></param>
        private static void InitializeFromSourceImage(Size size)
        {
            Bitmap sourceImage;
            if (File.Exists(BackImagePath))
            {
                sourceImage = (Bitmap)Image.FromFile(BackImagePath);
            }
            else { sourceImage = Resources.BackImage; }
            var width = size.Width;
            var height = size.Height;
            var bkWidth = width;
            var bkHeight = height;
            var sourceRatio = (double)sourceImage.Width / sourceImage.Height;
            var clientRatio = (double)width / height;
            if (sourceRatio < clientRatio)
            {
                bkWidth = width;
                bkHeight = (int)(width / sourceRatio);
            }
            else if (sourceRatio > clientRatio)
            {
                bkHeight = height;
                bkWidth = (int)(height * sourceRatio);
            }

            if (_image is not null && _image.Width == bkWidth && _image.Height == bkHeight)
                return;
            _image?.Dispose();
            _image = new(bkWidth, bkHeight);
            var g = Graphics.FromImage(_image);
            g.CompositingMode = CompositingMode.SourceCopy;
            g.CompositingQuality = CompositingQuality.Invalid;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(sourceImage, 0, 0, bkWidth, bkHeight);
            g.Flush(); g.Dispose();
            sourceImage.Dispose();
        }
    }
}
