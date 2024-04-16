using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test;

public class Tree
{
    public List<Point> StuckedList { get; set; } = new();

    public int MaxWalkerNumber { get; set; } = 10000;

    public Rectangle Bounds { get; set; } = new(0, 0, 100, 100);

    public Point[] Roots { get; set; } = [new(70, 33), new(25, 60) ];

    public int RootNumber { get; set; } = 1;

    public Tree()
    {
        for (int i = 0; i < RootNumber; i++)
        {
            StuckedList.Add(new(
                new Random().Next(Bounds.Left, Bounds.Right + 1),
                new Random().Next(Bounds.Top, Bounds.Bottom + 1)
                ));
        }
        //StuckedList.AddRange(Roots);
    }

    public void Generate()
    {
        Walker.MapBounds = Bounds;
        while (StuckedList.Count < MaxWalkerNumber)
        {
            AddWalker(out var walker);
            StuckedList.Add(new(walker.X, walker.Y));

        }
    }

    private void AddWalker(out Walker walker)
    {
        walker = new Walker();
        do
        {
            walker.Walk();
        } while (!walker.CheckStuck(StuckedList.ToArray()));
    }
}
