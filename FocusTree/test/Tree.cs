using LocalUtilities.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace test;

public class Tree(int width, int height, (int, int)[] roots, int walkerNumber) : Roster<(int, int), Walker>
{
    public int WalkerNumber { get; set; } = walkerNumber;

    public Rectangle Bounds { get; set; } = new(0, 0, width, height);

    public (int X, int Y)[] Roots { get; set; } = roots;

    public int HeightMax { get; set; } = 0;

    public Tree() : this(0, 0, [], 0)
    {

    }

    public void Generate()
    {
        foreach (var item in Roots)
            RosterMap[item] = new(item.X, item.Y);
        Walker.MapBounds = Bounds;
        for (int i = 0; RosterMap.Count < WalkerNumber; i++)
        {
            AddWalker(out var walker);
            RosterMap[(walker.X, walker.Y)] = walker;
        }
    }

    public void Generate((int Width, int Height) rootNumber)
    {
        var widthUnit = Bounds.Width / rootNumber.Width;
        var heightUnit = Bounds.Height / rootNumber.Height;
        List<(int X, int Y)> list = new();
        for (int i = 0; i < rootNumber.Width; i++)
        {
            for (int j = 0; j < rootNumber.Height; j++)
            {
                var left = Bounds.Left + widthUnit * i;
                var top = Bounds.Top + heightUnit * j;
                list.Add((
                new Random().Next(left, left + widthUnit + 1),
                new Random().Next(top, top + heightUnit + 1)
                ));
            }
        }
        Roots = list.ToArray();
        Generate();
    }

    private void AddWalker(out Walker walker)
    {
        walker = new Walker();
        do
        {
            walker.Walk();
        } while (!walker.CheckStuck(RosterMap));
    }

    public void ComputeDirectionLevel()
    {
        foreach (var pair in RosterMap)
        {
            var walker = pair.Value;
            CheckDirection(Direction.Left, walker);
            CheckDirection(Direction.Top, walker);
            CheckDirection(Direction.Right, walker);
            CheckDirection(Direction.Bottom, walker);
            CheckDirection(Direction.LeftTop, walker);
            CheckDirection(Direction.TopRight, walker);
            CheckDirection(Direction.LeftBottom, walker);
            CheckDirection(Direction.BottomRight, walker);
            var height = walker.Height;
            HeightMax = Math.Max(HeightMax, height);
        }
    }

    private int CheckDirection(Direction direction, Walker walker)
    {
        if (!walker.ConnetNumber.ContainsKey(direction))
        {
            if (walker.Neighbor.TryGetValue(direction, out var neighbor))

                walker.ConnetNumber[direction] = CheckDirection(direction, RosterMap[neighbor]) + 1;
            else
                walker.ConnetNumber[direction] = 0;
        }
        return walker.ConnetNumber[direction];
    }

    public void ResetRelations()
    {
        foreach (var walker in RosterMap.Values)
        {
            var x = walker.X;
            var y = walker.Y;
            var left = Math.Max(x - 1, Bounds.Left);
            var top = Math.Max(y - 1, Bounds.Top);
            var right = Math.Min(x + 1, Bounds.Right);
            var bottom = Math.Min(y + 1, Bounds.Bottom);
            if (RosterMap.TryGetValue((left, y), out var other))
            {
                walker.Neighbor[Direction.Left] = (left, y);
                other.Neighbor[Direction.Right] = (x, y);
            }
            if (RosterMap.TryGetValue((right, y), out other))
            {
                walker.Neighbor[Direction.Right] = (right, y);
                other.Neighbor[Direction.Left] = (x, y);
            }
            if (RosterMap.TryGetValue((x, top), out other))
            {
                walker.Neighbor[Direction.Top] = (x, top);
                other.Neighbor[Direction.Bottom] = (x, y);
            }
            if (RosterMap.TryGetValue((x, bottom), out other))
            {
                walker.Neighbor[Direction.Bottom] = (x, bottom);
                other.Neighbor[Direction.Top] = (x, y);
            }
            if (RosterMap.TryGetValue((left, top), out other))
            {
                walker.Neighbor[Direction.LeftTop] = (left, top);
                other.Neighbor[Direction.BottomRight] = (x, y);
            }
            if (RosterMap.TryGetValue((left, bottom), out other))
            {
                walker.Neighbor[Direction.LeftBottom] = (left, bottom);
                other.Neighbor[Direction.TopRight] = (x, y);
            }
            if (RosterMap.TryGetValue((right, top), out other))
            {
                walker.Neighbor[Direction.TopRight] = (right, top);
                other.Neighbor[Direction.LeftBottom] = (x, y);
            }
            if (RosterMap.TryGetValue((right, bottom), out other))
            {
                walker.Neighbor[Direction.BottomRight] = (right, bottom);
                other.Neighbor[Direction.LeftTop] = (x, y);
            }
        }
    }
}
