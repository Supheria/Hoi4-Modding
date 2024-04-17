using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace test;

public class Program
{
    public static void TestBelin()
    {
        var image = new Bitmap(500, 500);
        var g = Graphics.FromImage(image);
        g.Clear(Color.Black);
        g.Flush(); g.Dispose();

        var pImage = new PointBitmap(image);
        pImage.LockBits();
        for (double i = 0; i < image.Width; i++)
        {
            for (double j = 0; j < image.Height; j++)
            {
                var height = //new Perlin(255).OctavePerlin(i / 50, j / 50, 2, 5) * 0.5 +

                    new Perlin(55).OctavePerlin(i / 50, j / 50, 8, 2) * 0.5;
                pImage.SetPixel((int)i, (int)j, Color.FromArgb((int)(height * 255), Color.White));
            }
        }
        pImage.UnlockBits();

        image.Save("Perlin.bmp");
    }

    public static Tree GetScaledRoots(Bitmap source, (int Width, int Height) scaleAdd, int rollTimes)
    {
        var scale = new Bitmap((int)(source.Width + scaleAdd.Width), (int)(source.Height + scaleAdd.Height));
        var g = Graphics.FromImage(scale);
        g.Clear(Color.Black);
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.DrawImage(source, 0, 0, scale.Width, scale.Height);
        g.Flush(); g.Dispose();
        scale.Save("_scale.bmp");

        var list = new List<(int X, int Y)>();
        var pScale = new PointBitmap(scale);
        pScale.LockBits();
        for (var i = 0; i < scale.Width; i++)
        {
            for (var j = 0; j < scale.Height; j++)
            {
                var color = pScale.GetPixel(i, j);
                if (color.ToArgb() == Color.White.ToArgb())
                    list.Add((i, j));
            }
        }
        pScale.UnlockBits();
        var tree = new Tree(scale.Width, scale.Height, list.ToArray(), rollTimes);
        //tree.Generate();
        return tree;
    }

    public static Bitmap GetDlaImage(Tree tree)
    {
        //tree.ComputeDirectionLevel();

        var image = new Bitmap(tree.Bounds.Width, tree.Bounds.Height);
        var g = Graphics.FromImage(image);
        g.Clear(Color.Black);
        g.Flush(); g.Dispose();

        var pImage = new PointBitmap(image);
        pImage.LockBits();
        foreach (var point in tree.Roster.Keys)
        {
            //var bright = pair.Value.Height() * 30;
            //var a = (2 + 1) - Math.Abs(1 - 2);
            //pImage.SetPixel(pair.Key.X, pair.Key.Y, (Color.FromArgb(((int)bright) > 255 ? 255 : (int)bright, Color.White)));
            pImage.SetPixel(point.X, point.Y, Color.White);
        }
        pImage.UnlockBits();
        image.Save("_dlg.bmp");
        return image;
    }

    public static void Rolling(Tree initial, (int, int) scaleAdd, int rollTimes, out Tree tree)
    {
        var image = GetDlaImage(initial);
        tree = GetScaledRoots(image, scaleAdd, initial.RollTimes);
        for (int i = 0; i < rollTimes; i++)
        {
            image = GetDlaImage(tree);
            tree = GetScaledRoots(image, scaleAdd, tree.RollTimes);
        }
    }

    public static void Main()
    {
        //TestBelin();

        var tree = new Tree(200, 200, [(100, 100)], 15000);
        tree.Generate();
        //tree = new Tree(100, 100, tree.Roster.Keys.ToArray(), 1000);
        //tree.Generate();
        //tree.Generate();
        tree.ResetRelations();
        tree.ComputeDirectionLevel();
        ////Rolling(new(100, 100, [(50, 50)], 5000), (100, 100), 0, out var tree);
        //var image = GetDlaImage(tree);
        //tree = GetScaledRoots(image, (0, 0), 1000);
        //tree.Generate();
        //image = GetDlaImage(tree);
        //tree = GetScaledRoots(image, (0, 0), 1000);
        //tree.Generate();
        //image = GetDlaImage(tree);
        //tree.ResetRelations();
        //tree.ComputeDirectionLevel();
        var image = new Bitmap(tree.Bounds.Width, tree.Bounds.Height);
        var g = Graphics.FromImage(image);
        g.Clear(Color.Black);
        g.Flush(); g.Dispose();
        var pImage = new PointBitmap(image);
        pImage.LockBits();
        foreach (var pair in tree.Roster)
        {
            var bright = pair.Value.Height() * 30;
            pImage.SetPixel(pair.Key.X, pair.Key.Y, (Color.FromArgb(((int)bright) > 255 ? 255 : (int)bright, Color.White)));
        }
        pImage.UnlockBits();
        image.Save("_scale_gen.bmp");


        double[,] kernel = new double[3, 3] {
            { 1.0 / 9.0, 1.0 / 9.0, 1.0 / 9.0},
            { 1.0 / 9.0, 1.0 / 9.0, 1.0 / 9.0},
            { 1.0 / 9.0, 1.0 / 9.0, 1.0 / 9.0},
        };
        var a = image.Convolution_calculation(kernel);
        a.Save("_gaussian.bmp");

        //image = (Bitmap)Bitmap.FromFile("_gaussian.bmp");

        

        

        var result = new Bitmap(image.Width, image.Height + 200);

        g = Graphics.FromImage(result);
        g.Clear(Color.Black);
        pImage = new(image);
        pImage.LockBits();
        var pResult = new PointBitmap(result);
        pResult.LockBits();
        for (var i = 0; i < image.Width; i++)
        {
            for (var j = 0; j < image.Height; j++)
            {
                var pixel = pImage.GetPixel(i, j);
                if (pixel.ToArgb() == Color.Black.ToArgb())
                    pResult.SetPixel(i, j, Color.LightYellow);
                else
                {
                    if (pixel.A < 50)
                        pResult.SetPixel(i, j, Color.ForestGreen);
                    else if (pixel.A > 50 && pixel.A <= 150)
                        pResult.SetPixel(i, j, Color.Black);
                    else
                        pResult.SetPixel(i, j, Color.SkyBlue);
                }
                
                
            }
        }
        pImage.UnlockBits();
        pResult.UnlockBits();
        g.DrawString($"\n根数 {tree.Roots.Length}\n\n增点数 {tree.Roster.Count}\n\n范围 {tree.Bounds}", new("仿宋", 15, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.White), new RectangleF(0, result.Height - 200, result.Width, 200));
        g.Flush(); g.Dispose();

        //result = result.Convolution_calculation(kernel);

        result.Save(".\\a.bmp");

        Console.WriteLine("OK");

        //Console.ReadKey();
    }
}