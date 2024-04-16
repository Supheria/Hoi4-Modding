using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace test;

public class Walker
{
    public static Rectangle MapBounds { get; set; } = new();

    public int X { get; private set; } = new Random().Next(MapBounds.Left, MapBounds.Right + 1);

    public int Y { get; private set; } = new Random().Next(MapBounds.Top, MapBounds.Bottom + 1);

    public (int Left, int Top, int Right, int Bottom) Surround()
    {
        var left = X - 1;
        //left = left < MapBounds.Left ? MapBounds.Left : left;
        var top = Y - 1;
        //top = top < MapBounds.Top ? MapBounds.Top : top;
        var right = X + 1;
        //right = right > MapBounds.Right ? MapBounds.Right : right;
        var bottom = Y + 1;
        //bottom = bottom > MapBounds.Bottom ? MapBounds.Bottom : bottom;
        return new(left, top, right, bottom);
    }

    public bool CheckStuck(Point[] stuckedPoint)
    {
        var surround = Surround();
        foreach (var point in stuckedPoint)
        {
            if (point.X < surround.Left || point.X > surround.Right ||
                point.Y < surround.Top || point.Y > surround.Bottom)
                continue;
            if (point.X == X ||  point.Y == Y)
                return true;
        }
        return false;
    }

    public void Walk()
    {
        switch (new Random().Next(0, 8))
        {
            case 0: // left
                X--;
                break;
            case 1: // right
                X++;
                break;
            case 2: // up
                Y--;
                break;
            case 3: // down
                Y++;
                break;
            case 4: // left up
                X--;
                Y--;
                break;
            case 5: // up right
                X++;
                Y--;
                break;
            case 6: // bottom right
                X++;
                Y++;
                break;
            case 7: // left bottom
                X--;
                Y++;
                break;
        }
        if (X < MapBounds.Left || X > MapBounds.Right)
            X = new Random().Next(MapBounds.Left, MapBounds.Right + 1);
        if (Y < MapBounds.Top || Y > MapBounds.Bottom)
            Y = new Random().Next(MapBounds.Top, MapBounds.Bottom + 1);
    }
}

