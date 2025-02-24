using System;
using System.Collections.Generic;
using System.Windows;
using ModernNesting.Models;

namespace ModernNesting.Core
{
    public class CollisionDetector
    {
        private const double EPSILON = 1e-10;

        public bool CheckCollision(Part part1, Point pos1, Part part2, Point pos2)
        {
            // Primero verificamos el bounding box para optimización
            if (!CheckBoundingBoxCollision(part1, pos1, part2, pos2))
            {
                return false;
            }

            // Si hay colisión de bounding box, verificamos la colisión precisa
            return CheckPreciseCollision(part1, pos1, part2, pos2);
        }

        private bool CheckBoundingBoxCollision(Part part1, Point pos1, Part part2, Point pos2)
        {
            double left1 = pos1.X;
            double right1 = pos1.X + part1.Width;
            double top1 = pos1.Y;
            double bottom1 = pos1.Y + part1.Height;

            double left2 = pos2.X;
            double right2 = pos2.X + part2.Width;
            double top2 = pos2.Y;
            double bottom2 = pos2.Y + part2.Height;

            return !(right1 < left2 || left1 > right2 || bottom1 < top2 || top1 > bottom2);
        }

        private bool CheckPreciseCollision(Part part1, Point pos1, Part part2, Point pos2)
        {
            // Esta es una implementación básica que será mejorada
            // con algoritmos más sofisticados de detección de colisiones
            return CheckBoundingBoxCollision(part1, pos1, part2, pos2);
        }

        public bool IsInsideSheet(Part part, Point position, Sheet sheet)
        {
            return position.X >= 0 &&
                   position.Y >= 0 &&
                   position.X + part.Width <= sheet.Width &&
                   position.Y + part.Height <= sheet.Height;
        }
    }
}