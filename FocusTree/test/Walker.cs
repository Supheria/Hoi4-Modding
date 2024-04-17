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

public class Walker(int x, int y)
{
    public static Rectangle MapBounds { get; set; } = new();

    public int X { get; private set; } = x;

    public int Y { get; private set; } = y;

    public (int X, int Y)? Left { get; set; } = null;

    public (int X, int Y)? Top { get; set; } = null;

    public (int X, int Y)? Right { get; set; } = null;

    public (int X, int Y)? Bottom { get; set; } = null;

    public int Level { get; set; } = 0;

    public Walker() : this(
        new Random().Next(MapBounds.Left, MapBounds.Right + 1), 
        new Random().Next(MapBounds.Top, MapBounds.Bottom + 1)
        )
    {

    }

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

    public bool CheckStuck(Dictionary<(int, int), Walker> stuckedPoint)
    {
        var surround = Surround();
        var done = false;
        foreach (var pair in stuckedPoint)
        {
            var stucked = pair.Value;
            if (stucked.Y == Y)
            {
                if (stucked.X == X)
                    return true;
                if (stucked.X == surround.Left)
                {
                    Left = pair.Key;
                    stucked.Right = (X, Y);
                    done = true;
                }
                else if (stucked.X == surround.Right)
                {
                    Right = pair.Key;
                    stucked.Left = (X, Y);
                    done = true;
                }
            }
            if (stucked.X == X)
            {
                if (stucked.Y == Y)
                    return true;
                if (stucked.Y == surround.Top)
                {
                    Top = pair.Key;
                    stucked.Bottom = (X, Y);
                    done = true;
                }
                else if (stucked.Y == surround.Bottom)
                {
                    Bottom = pair.Key;
                    stucked.Top = (X, Y);
                    done = true;
                }
            }
        }
        return done;
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

