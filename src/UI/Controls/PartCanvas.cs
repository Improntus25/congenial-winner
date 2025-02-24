using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ModernNesting.Models;

namespace ModernNesting.UI.Controls
{
    public class PartCanvas : Canvas
    {
        private readonly List<Part> parts = new List<Part>();
        private readonly List<Sheet> sheets = new List<Sheet>();
        private double scale = 1.0;
        private Point panOffset = new Point(0, 0);
        private bool isPanning = false;
        private Point lastMousePosition;
        private Part highlightedPart = null;

        public PartCanvas()
        {
            Background = Brushes.White;
            ClipToBounds = true;

            // Enable pan and zoom
            MouseWheel += OnMouseWheel;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
            MouseMove += OnMouseMove;
        }

        public void SetParts(List<Part> newParts)
        {
            parts.Clear();
            parts.AddRange(newParts);
            FitToScreen();
            InvalidateVisual();
        }

        public void SetSheets(List<Sheet> newSheets)
        {
            sheets.Clear();
            sheets.AddRange(newSheets);
            FitToScreen();
            InvalidateVisual();
        }

        public void ZoomIn()
        {
            scale *= 1.1;
            InvalidateVisual();
        }

        public void ZoomOut()
        {
            scale *= 0.9;
            scale = Math.Max(0.1, scale);
            InvalidateVisual();
        }

        public void HighlightPart(Part part)
        {
            highlightedPart = part;
            InvalidateVisual();
        }

        public void FitToScreen()
        {
            if (!parts.Any() && !sheets.Any()) return;

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            // Consider parts
            foreach (var part in parts)
            {
                minX = Math.Min(minX, part.Position.X);
                minY = Math.Min(minY, part.Position.Y);
                maxX = Math.Max(maxX, part.Position.X + part.Width);
                maxY = Math.Max(maxY, part.Position.Y + part.Height);
            }

            // Consider sheets
            foreach (var sheet in sheets)
            {
                maxX = Math.Max(maxX, sheet.Width);
                maxY = Math.Max(maxY, sheet.Height);
            }

            if (minX == double.MaxValue) return;

            double contentWidth = maxX - minX;
            double contentHeight = maxY - minY;

            // Calculate scale to fit
            double scaleX = ActualWidth / (contentWidth + 40);  // 20px padding on each side
            double scaleY = ActualHeight / (contentHeight + 40);
            scale = Math.Min(scaleX, scaleY);

            // Center content
            panOffset.X = (ActualWidth - (contentWidth * scale)) / 2 - (minX * scale);
            panOffset.Y = (ActualHeight - (contentHeight * scale)) / 2 - (minY * scale);

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // Apply transform for pan and zoom
            dc.PushTransform(new TranslateTransform(panOffset.X, panOffset.Y));
            dc.PushTransform(new ScaleTransform(scale, scale));

            // Draw sheets
            foreach (var sheet in sheets)
            {
                DrawSheet(dc, sheet);
            }

            // Draw parts
            foreach (var part in parts)
            {
                DrawPart(dc, part, part == highlightedPart);
            }

            dc.Pop();
            dc.Pop();
        }

        private void DrawSheet(DrawingContext dc, Sheet sheet)
        {
            var rect = new Rect(0, 0, sheet.Width, sheet.Height);
            dc.DrawRectangle(null, new Pen(Brushes.Blue, 1 / scale), rect);
        }

        private void DrawPart(DrawingContext dc, Part part, bool isHighlighted)
        {
            var brush = isHighlighted ? Brushes.LightBlue : Brushes.LightGray;
            var pen = new Pen(isHighlighted ? Brushes.Blue : Brushes.Black, 1 / scale);

            if (part.Geometry != null && part.Geometry.Points.Count > 0)
            {
                var geometry = new StreamGeometry();
                using (var context = geometry.Open())
                {
                    context.BeginFigure(part.Geometry.Points[0], true, true);
                    context.PolyLineTo(part.Geometry.Points.Skip(1).ToList(), true, true);
                }
                geometry.Transform = new TranslateTransform(part.Position.X, part.Position.Y);

                dc.DrawGeometry(brush, pen, geometry);
            }
            else
            {
                var rect = new Rect(part.Position, new Size(part.Width, part.Height));
                dc.DrawRectangle(brush, pen, rect);
            }
        }

        private void OnMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var position = e.GetPosition(this);
            var oldScale = scale;

            // Adjust scale based on wheel delta
            scale *= e.Delta > 0 ? 1.1 : 0.9;
            scale = Math.Max(0.1, Math.Min(10.0, scale));

            // Adjust pan offset to zoom towards mouse position
            if (scale != oldScale)
            {
                var scaleFactor = scale / oldScale;
                panOffset.X = position.X - (position.X - panOffset.X) * scaleFactor;
                panOffset.Y = position.Y - (position.Y - panOffset.Y) * scaleFactor;
                InvalidateVisual();
            }
        }

        private void OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isPanning = true;
            lastMousePosition = e.GetPosition(this);
            CaptureMouse();
        }

        private void OnMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isPanning = false;
            ReleaseMouseCapture();
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isPanning)
            {
                var position = e.GetPosition(this);
                var delta = position - lastMousePosition;
                panOffset.X += delta.X;
                panOffset.Y += delta.Y;
                lastMousePosition = position;
                InvalidateVisual();
            }
        }
    }
}