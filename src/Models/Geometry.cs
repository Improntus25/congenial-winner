using System;
using System.Collections.Generic;
using System.Windows;

namespace ModernNesting.Models
{
    public class Geometry
    {
        public List<Point> Points { get; set; } = new List<Point>();
        public double Width { get; private set; }
        public double Height { get; private set; }
        public Point Center { get; private set; }

        public Geometry()
        {
            Points = new List<Point>();
        }

        public void AddPoint(double x, double y)
        {
            Points.Add(new Point(x, y));
            RecalculateBounds();
        }

        private void RecalculateBounds()
        {
            if (Points.Count == 0) return;

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (var point in Points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
            }

            Width = maxX - minX;
            Height = maxY - minY;
            Center = new Point(minX + Width / 2, minY + Height / 2);
        }

        public void Normalize()
        {
            if (Points.Count == 0) return;

            // Find current bounds
            double minX = double.MaxValue;
            double minY = double.MaxValue;

            foreach (var point in Points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
            }

            // Translate all points so minX and minY are 0
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] = new Point(
                    Points[i].X - minX,
                    Points[i].Y - minY
                );
            }

            RecalculateBounds();
        }

        public double CalculateArea()
        {
            // Using shoelace formula to calculate area
            double area = 0;
            for (int i = 0; i < Points.Count; i++)
            {
                Point current = Points[i];
                Point next = Points[(i + 1) % Points.Count];
                area += (current.X * next.Y) - (next.X * current.Y);
            }
            return Math.Abs(area) / 2;
        }
    }
}