using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace test;

public class Program
{
    static Dictionary<(int X, int Y), int> CountCache = new();

    public static void Main()
    {
        
        var tree = new Tree() { Roots = [(25, 25)] };
        tree.Generate();
        //tree.ComputeLevel();
        //Count(tree.Roster.ToArray());

        var image = new Bitmap(tree.Bounds.Width, tree.Bounds.Height);
        var g = Graphics.FromImage(image);
        g.Clear(Color.Black);
        g.Flush(); g.Dispose();

        PointBitmap pBack = new(image);
        pBack.LockBits();
        foreach(var pair in tree.Roster)
        {
            //var bright = pair.Value;
            //bright = bright > 255 ? 255 : bright;
            //pBack.SetPixel(pair.Key.X, pair.Key.Y, Color.FromArgb(bright, Color.Yellow));
            //var bright = pair.Value.Level * 30;
            //pBack.SetPixel(pair.Key.X, pair.Key.Y, (Color.FromArgb(bright > 255 ? 255 : bright, Color.White)));
            pBack.SetPixel(pair.Key.X, pair.Key.Y, (Color.White));
        }
        pBack.UnlockBits();

        image.Save("1.bmp");

        var scale = new Bitmap(tree.Bounds.Width * 2, tree.Bounds.Height * 2);
        g = Graphics.FromImage(scale);
        g.CompositingMode = CompositingMode.SourceCopy;
        g.CompositingQuality = CompositingQuality.Invalid;
        g.InterpolationMode = InterpolationMode.HighQualityBilinear;
        g.DrawImage(image, 0, 0, image.Width, image.Height);
        scale.Save("2.bmp");

        var list = new List<(int X, int Y)>();
        pBack = new(scale);
        pBack.LockBits();
        for (var i = 0; i < image.Width; i++)
        {
            for (var j = 0; j < image.Height; j++)
            {
                var color = pBack.GetPixel(i, j);
                if (color.ToArgb() != Color.Black.ToArgb())
                    list.Add((i, j));

            }
        }
        pBack.UnlockBits();
        tree = new() { Roots = list.ToArray(), Bounds = new(0, 0, scale.Width, scale.Height)};
        tree.Generate();
        g.Clear(Color.Black);
        pBack.LockBits();
        foreach (var pair in tree.Roster)
        {
            //var bright = pair.Value;
            //bright = bright > 255 ? 255 : bright;
            //pBack.SetPixel(pair.Key.X, pair.Key.Y, Color.FromArgb(bright, Color.Yellow));
            //var bright = pair.Value.Level * 30;
            //pBack.SetPixel(pair.Key.X, pair.Key.Y, (Color.FromArgb(bright > 255 ? 255 : bright, Color.White)));
            pBack.SetPixel(pair.Key.X, pair.Key.Y, (Color.White));
        }
        pBack.UnlockBits();

        scale.Save("3.bmp");

        //double[,] kernel = new double[5, 5] { 
        //    { 1.0 / 9.0, 1.0 / 9.0, 1.0 / 9.0 , 1.0 / 9.0 , 1.0 / 9.0},
        //    { 1.0 / 9.0, 1.0 / 9.0, 1.0 / 9.0 , 1.0 / 9.0 , 1.0 / 9.0},
        //    { 1.0 / 9.0, 1.0 / 9.0, 1.0 / 9.0 , 1.0 / 9.0 , 1.0 / 9.0},
        //    { 1.0 / 9.0, 1.0 / 9.0, 1.0 / 9.0 , 1.0 / 9.0 , 1.0 / 9.0},
        //    { 1.0 / 9.0, 1.0 / 9.0, 1.0 / 9.0 , 1.0 / 9.0 , 1.0 / 9.0},};

        //就像前面说的，权值都设置为九分之一

        //image.Save("test.bmp");
        ////image = image.Convolution_calculation(kernel);
        ////image.Dispose();
        //image.Save("b.bmp");

        var result = new Bitmap(image.Width, image.Height + 200);

        g = Graphics.FromImage(result);
        g.Clear(Color.Black);
        pBack = new(image);
        pBack.LockBits();
        var pResult = new PointBitmap(result);
        pResult.LockBits();
        for (var i = 0; i < image.Width; i++)
        {
            for (var j = 0; j < image.Height; j++)
            {
                var pixel = pBack.GetPixel(i, j);
                if (pixel.A <= 0)
                    pResult.SetPixel(i, j, Color.Blue);
                else if (pixel.A > 0 && pixel.A <= 75)
                    pResult.SetPixel(i, j, Color.Yellow);
                else if (pixel.A > 75 && pixel.A <= 150)
                    pResult.SetPixel(i, j, Color.DarkGreen);
                else
                    pResult.SetPixel(i, j, Color.Black);
                
            }
        }
        pBack.UnlockBits();
        pResult.UnlockBits();
        g.DrawString($"\n根数 {tree.RootNumber}\n\n点数上限 {tree.MaxWalkerNumber}\n\n范围 {tree.Bounds}", new("仿宋", 15, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.White), new RectangleF(0, result.Height - 200, result.Width, 200));
        g.Flush(); g.Dispose();

        //result = result.Convolution_calculation(kernel);

        result.Save(".\\a.bmp");

        Console.WriteLine("OK");

        //Console.ReadKey();
    }

    public static void Count(Point[] points)
    {
        foreach (var point in points)
        {
            var key = (point.X, point.Y);
            if (CountCache.ContainsKey(key))
                CountCache[key]++;
            else
                CountCache[key] = 0;
        }
    }
}