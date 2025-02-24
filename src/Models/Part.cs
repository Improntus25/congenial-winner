using System;
using System.Collections.Generic;

namespace ModernNesting.Models
{
    public class Part
    {
        public string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public List<Point> Vertices { get; set; }
        public double Rotation { get; set; }
        public double Area { get; private set; }
        public bool AllowRotation { get; set; }

        public Part(string name, double width, double height)
        {
            Name = name;
            Width = width;
            Height = height;
            Vertices = new List<Point>();
            Rotation = 0;
            AllowRotation = true;
            CalculateArea();
        }

        private void CalculateArea()
        {
            if (Vertices.Count > 0)
            {
                // Calculate area from vertices using shoelace formula
                double sum = 0;
                for (int i = 0; i < Vertices.Count; i++)
                {
                    var j = (i + 1) % Vertices.Count;
                    sum += Vertices[i].X * Vertices[j].Y;
                    sum -= Vertices[j].X * Vertices[i].Y;
                }
                Area = Math.Abs(sum) / 2;
            }
            else
            {
                Area = Width * Height;
            }
        }
    }

    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
