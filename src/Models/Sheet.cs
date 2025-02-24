using System;

namespace ModernNesting.Models
{
    public class Sheet
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public string Material { get; set; }
        public double Thickness { get; set; }
        public int Quantity { get; set; }
        public double Area => Width * Height;

        public Sheet(double width, double height, string material = "Default", double thickness = 1.0)
        {
            Width = width;
            Height = height;
            Material = material;
            Thickness = thickness;
            Quantity = 1;
        }

        public bool CanFit(Part part)
        {
            return part.Width <= Width && part.Height <= Height;
        }
    }
}
