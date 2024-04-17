using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test;

public class Tree
{
    public Dictionary<(int X, int Y), Walker> Roster { get; set; } = new();

    public int MaxWalkerNumber { get; set; } = 100;

    public Rectangle Bounds { get; set; } = new(0, 0, 50, 50);

    public (int X, int Y)[] Roots { get; set; } = [];

    public int RootNumber { get; set; } = 1;

    public Tree()
    {
        //for (int i = 0; i < RootNumber; i++)
        //{
        //    var x = new Random().Next(Bounds.Left, Bounds.Right + 1);
        //    var y = new Random().Next(Bounds.Top, Bounds.Bottom + 1);
        //    Roster[(x, y)] = new(x, y);
        //}
    }

    public void Generate()
    {
        foreach (var item in Roots)
            Roster[item] = new(item.X, item.Y);
        Walker.MapBounds = Bounds;
        for (int i = 0; i < MaxWalkerNumber; i++)
        {
            AddWalker(out var walker);
            Roster[(walker.X, walker.Y)] = walker;

        }
    }

    private void AddWalker(out Walker walker)
    {
        walker = new Walker();
        do
        {
            walker.Walk();
        } while (!walker.CheckStuck(Roster));
    }

    public void ComputeLevel()
    {
        var number = Roster.Count;
        var read = false;
        for (int level = 1; number > 0; level++)
        {
            read = false;
            foreach (var pair in Roster)
            {
                var walker = pair.Value;
                if (walker.Level is not 0)
                    continue;
                if ((walker.Left is null || walker.Right is null) &&
                    (walker.Top is null || walker.Bottom is null))
                {
                    walker.Level = level;
                    if (walker.Left is not null)
                        Roster[walker.Left.Value].Right = null;
                    if (walker.Right is not null)
                        Roster[walker.Right.Value].Left = null;
                    if (walker.Top is not null)
                        Roster[walker.Top.Value].Bottom = null;
                    if (walker.Bottom is not null)
                        Roster[walker.Bottom.Value].Top = null;
                    number--;
                    read = true;
                }
            }
            if (read == false)
                break;
        }
            
    }
}
