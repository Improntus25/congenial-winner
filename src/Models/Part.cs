using System;
using System.Collections.Generic;
using System.Windows;

namespace ModernNesting.Models
{
    public class Part
    {
        public string Name { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Geometry Geometry { get; set; }
        public double Rotation { get; set; }
        public double Area => Geometry?.CalculateArea() ?? (Width * Height);
        public bool AllowRotation { get; set; }
        public Point Position { get; set; }

        public Part()
        {
            Name = "Unnamed Part";
            Rotation = 0;
            AllowRotation = true;
            Position = new Point(0, 0);
        }

        public Part(string name, double width, double height)
        {
            Name = name;
            Width = width;
            Height = height;
            Rotation = 0;
            AllowRotation = true;
            Position = new Point(0, 0);
        }

        public bool CanFitInSheet(Sheet sheet)
        {
            // Basic check without considering rotation
            return Width <= sheet.Width && Height <= sheet.Height;
        }

        public bool Intersects(Part other)
        {
            // Basic bounding box intersection check
            // This should be replaced with proper geometry intersection check
            var thisRight = Position.X + Width;
            var thisBottom = Position.Y + Height;
            var otherRight = other.Position.X + other.Width;
            var otherBottom = other.Position.Y + other.Height;

            return !(Position.X >= otherRight ||
                    thisRight <= other.Position.X ||
                    Position.Y >= otherBottom ||
                    thisBottom <= other.Position.Y);
        }

        public Part Clone()
        {
            return new Part
            {
                Name = this.Name,
                Width = this.Width,
                Height = this.Height,
                Geometry = this.Geometry,
                Rotation = this.Rotation,
                AllowRotation = this.AllowRotation,
                Position = new Point(this.Position.X, this.Position.Y)
            };
        }

        public override string ToString()
        {
            return $"{Name} ({Width:F2}x{Height:F2})";
        }
    }
}