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

public enum Direction
{
    Left,
    Top,
    Right,
    Bottom,
}

public class Walker(int x, int y)
{
    public static Rectangle MapBounds { get; set; } = new();

    public int X { get; private set; } = x;

    public int Y { get; private set; } = y;

    public Dictionary<Direction, (int X, int Y)?> Neighbor { get; } = new() {
        [Direction.Left] = null,
        [Direction.Top] = null,
        [Direction.Right] = null,
        [Direction.Bottom] = null,
    };

    public Dictionary<Direction, int> ConnetNumber { get; } = new() {
        [Direction.Left] = -1,
        [Direction.Top] = -1,
        [Direction.Right] = -1,
        [Direction.Bottom] = -1,
    };

    public Walker() : this(
        new Random().Next(MapBounds.Left, MapBounds.Right + 1),
        new Random().Next(MapBounds.Top, MapBounds.Bottom + 1)
        )
    {

    }

    public int Height()
    {
        var verticalHeight = ConnetNumber[Direction.Top] + ConnetNumber[Direction.Bottom] - 
            Math.Abs(ConnetNumber[Direction.Top] - ConnetNumber[Direction.Bottom]);
        var horizontalHeight = ConnetNumber[Direction.Left] + ConnetNumber[Direction.Right] -
            Math.Abs(ConnetNumber[Direction.Left] - ConnetNumber[Direction.Right]);
        return Math.Max(verticalHeight, horizontalHeight);
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
        foreach (var pair in stuckedPoint)
        {
            var stucked = pair.Value;
            if (stucked.X < surround.Left || stucked.X > surround.Right ||
                stucked.Y < surround.Top || stucked.Y > surround.Bottom)
                continue;
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

