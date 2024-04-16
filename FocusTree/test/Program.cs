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
        
        var tree = new Tree();
        tree.Generate();
        Count(tree.StuckedList.ToArray());

        var image = new Bitmap(tree.Bounds.Width, tree.Bounds.Height);
        var g = Graphics.FromImage(image);
        g.Clear(Color.Black);
        g.Flush(); g.Dispose();

        PointBitmap pBack = new(image);
        pBack.LockBits();
        foreach(var pair in CountCache)
        {
            //var bright = pair.Value;
            //bright = bright > 255 ? 255 : bright;
            //pBack.SetPixel(pair.Key.X, pair.Key.Y, Color.FromArgb(bright, Color.Yellow));
            pBack.SetPixel(pair.Key.X, pair.Key.Y, (Color.White));
        }
        pBack.UnlockBits();

        double[,] kernel = new double[3, 3] { { 1.0 / 9.0, 1.0 / 9.0, 1.0 / 9.0 },
                                                                { 1.0 / 9.0, 1.0 / 9.0, 1.0 / 9.0 },
                                                                { 1.0 / 9.0, 1.0 / 9.0, 1.0 / 9.0 } };

        //就像前面说的，权值都设置为九分之一

        image = image.Convolution_calculation(kernel);

        var result = new Bitmap(200, 200);
        g = Graphics.FromImage(result);
        //g.CompositingMode = CompositingMode.SourceCopy;
        //g.CompositingQuality = CompositingQuality.Invalid;
        g.InterpolationMode = InterpolationMode.Bilinear;
        g.DrawImage(image, 0, 0, 200, 200);
        g.Flush(); g.Dispose();

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