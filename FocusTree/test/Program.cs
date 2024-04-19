using LocalUtilities.FileUtilities;
using LocalUtilities.Serializations;
using LocalUtilities.SerializeUtilities;
using LocalUtilities.StringUtilities;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Xml;

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
        foreach (var point in tree.RosterList)
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
        tree = GetScaledRoots(image, scaleAdd, initial.WalkerNumber);
        for (int i = 0; i < rollTimes; i++)
        {
            image = GetDlaImage(tree);
            tree = GetScaledRoots(image, scaleAdd, tree.WalkerNumber);
        }
    }

    public static void Main()
    {
        //TestBelin();
        var a = new TreeXmlSerialization().LoadFromXml(out _);

        var tree = new Tree(200, 200, [(100, 100)], 18000);
        tree.Generate((2, 1));
        tree.ComputeDirectionLevel();
        new TreeXmlSerialization() { Source = tree }.SaveToXml();
        tree = new TreeXmlSerialization().LoadFromXml(out _);
        tree.ResetRelations();
        tree.ComputeDirectionLevel();


        var image = new Bitmap(tree.Bounds.Width, tree.Bounds.Height + 200);
        var g = Graphics.FromImage(image);
        g.Clear(Color.Black);
        g.FillRectangle(new SolidBrush(Color.LightYellow), tree.Bounds);
        var pImage = new PointBitmap(image);
        pImage.LockBits();
        var forestRatio = 0.0835f; // 1/12
        var mountainRatio = 0.2505f; // 3/12
        // waterRatio                // 8/12
        double mountain = 0, water = 0, forest = 0;
        foreach (var walker in tree.RosterList)
        {
            float heightRatio = (float)walker.Height / (float)tree.HeightMax;
            if (heightRatio <= forestRatio)
            {
                pImage.SetPixel(walker.X, walker.Y, Color.ForestGreen);
                forest++;
            }
            else if (heightRatio > forestRatio && heightRatio <= forestRatio + mountainRatio)
            {
                pImage.SetPixel(walker.X, walker.Y, Color.Black);
                mountain++;
            }
            else
            {
                pImage.SetPixel(walker.X, walker.Y, Color.SkyBlue);
                water++;
            }
        }
        pImage.UnlockBits();
        var total = tree.Bounds.Width * tree.Bounds.Height;
        mountain = Math.Round(mountain / total * 100, 2);
        water = Math.Round(water / total * 100, 2);
        forest = Math.Round(forest / total * 100, 2);
        var plain = Math.Round(100 - (mountain + water + forest), 2);
        g.DrawString($"\n根数 {tree.Roots.Length}\n\n增点数 {tree.RosterList.Length}\n\n范围 {tree.Bounds}\n\n山地{mountain}% 平原{plain}%\n河水{water}% 树林{forest}%",
            new("仿宋", 15, FontStyle.Bold, GraphicsUnit.Pixel), new SolidBrush(Color.White), new RectangleF(0, image.Height - 200, image.Width, 200));
        g.Flush(); g.Dispose();


        image.Save("_scale_gen.bmp");

        Console.WriteLine("OK");
    }
}

public class TreeXmlSerialization : RosterXmlSerialization<Tree, (int, int), Walker>
{
    protected override string RosterName => "Items";

    public TreeXmlSerialization() : base(new(), new WalkerXmlSerialization())
    {
        OnRead += TreeXmlSerialization_OnRead;
        OnWrite += TreeXmlSerialization_OnWrite;
    }

    private void TreeXmlSerialization_OnRead(XmlReader reader)
    {
        Source.HeightMax = reader.GetAttribute(nameof(Source.HeightMax)).ToInt() ?? Source.HeightMax;
        while (reader.Read())
        {
            if (reader.Name == nameof(Source.Bounds))
            {
                Source.Bounds = new RectangleXmlSerialization(nameof(Source.Bounds)).Deserialize(reader);
                break;
            }
        }
    }

    private void TreeXmlSerialization_OnWrite(XmlWriter writer)
    {
        writer.WriteAttributeString(nameof(Source.HeightMax), Source.HeightMax.ToString());
        new RectangleXmlSerialization(nameof(Source.Bounds)) { Source = Source.Bounds }.Serialize(writer);
    }

    public override string LocalName => nameof(Tree);
}

public class WalkerXmlSerialization() : XmlSerialization<Walker>(new())
{
    public override string LocalName => nameof(Walker);

    public override void ReadXml(XmlReader reader)
    {
        var x = reader.GetAttribute(nameof(Source.X)).ToInt() ?? Source.X;
        var y = reader.GetAttribute(nameof(Source.Y)).ToInt() ?? Source.Y;
        Source.SetSignature = (x, y);
        Source.Height = reader.GetAttribute(nameof(Source.Height)).ToInt() ?? Source.Height;
    }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString(nameof(Source.X), Source.X.ToString());
        writer.WriteAttributeString(nameof(Source.Y), Source.Y.ToString());
        writer.WriteAttributeString(nameof(Source.Height), Source.Height.ToString());
    }
}