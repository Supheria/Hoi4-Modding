using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace test;

public class Tree(int width, int height, (int, int)[] roots, int rollTimes)
{
    public Dictionary<(int X, int Y), Walker> Roster { get; set; } = new();

    public int RollTimes { get; set; } = rollTimes;

    public Rectangle Bounds { get; set; } = new(0, 0, width, height);

    public (int X, int Y)[] Roots { get; set; } = roots;

    public void Generate()
    {
        foreach (var item in Roots)
            Roster[item] = new(item.X, item.Y);
        Walker.MapBounds = Bounds;
        for (int i = 0; Roster.Count < RollTimes; i++)
        {
            AddWalker(out var walker);
            Roster[(walker.X, walker.Y)] = walker;
        }
    }

    public void Generate(int rootNumber)
    {
        List<(int X, int Y)> list = new();
        for (int i = 0; i < rootNumber; i++)
            list.Add((
                new Random().Next(Bounds.Left, Bounds.Right + 1),
                new Random().Next(Bounds.Top, Bounds.Bottom + 1)
                ));
        Roots = list.ToArray();
        Generate();
    }

    private void AddWalker(out Walker walker)
    {
        walker = new Walker();
        do
        {
            walker.Walk();
        } while (!walker.CheckStuck(Roster));
    }

    public void ComputeDirectionLevel()
    {
        foreach (var pair in Roster)
        {
            var walker = pair.Value;
            CheckDirection(Direction.Left, walker);
            CheckDirection(Direction.Top, walker);
            CheckDirection(Direction.Right, walker);
            CheckDirection(Direction.Bottom, walker);
        }
    }

    private int CheckDirection(Direction direction, Walker walker)
    {
        if (walker.ConnetNumber[direction] < 0)
        {
            var neighbor = walker.Neighbor[direction];
            if (neighbor is null)
                walker.ConnetNumber[direction] = 0;
            else
                walker.ConnetNumber[direction] = CheckDirection(direction, Roster[neighbor.Value]) + 1;
        }
        return walker.ConnetNumber[direction];
    }

    public void ResetRelations()
    {
        foreach (var walker in Roster.Values)
        {
            walker.Neighbor[Direction.Left] = null;
            walker.Neighbor[Direction.Top] = null;
            walker.Neighbor[Direction.Right] = null;
            walker.Neighbor[Direction.Bottom] = null;
            walker.ConnetNumber[Direction.Left] = -1;
            walker.ConnetNumber[Direction.Top] = -1;
            walker.ConnetNumber[Direction.Right] = -1;
            walker.ConnetNumber[Direction.Bottom] = -1;
            var surround = walker.Surround();
            foreach (var pair in Roster)
            {
                var stucked = pair.Value;
                if (stucked.Y == walker.Y)
                {
                    if (walker.Neighbor[Direction.Left] is null && stucked.X == surround.Left)
                    {
                        walker.Neighbor[Direction.Left] = pair.Key;
                        stucked.Neighbor[Direction.Right] = (walker.X, walker.Y);
                    }
                    else if (walker.Neighbor[Direction.Right] is null && stucked.X == surround.Right)
                    {
                        walker.Neighbor[Direction.Right] = pair.Key;
                        stucked.Neighbor[Direction.Left] = (walker.X, walker.Y);
                    }
                }
                if (stucked.X == walker.X)
                {
                    if (walker.Neighbor[Direction.Top] is null && stucked.Y == surround.Top)
                    {
                        walker.Neighbor[Direction.Top] = pair.Key;
                        stucked.Neighbor[Direction.Bottom] = (walker.X, walker.Y);
                    }
                    else if (walker.Neighbor[Direction.Bottom] is null && stucked.Y == surround.Bottom)
                    {
                        walker.Neighbor[Direction.Bottom] = pair.Key;
                        stucked.Neighbor[Direction.Top] = (walker.X, walker.Y);
                    }
                }
            }
        }
    }
}
