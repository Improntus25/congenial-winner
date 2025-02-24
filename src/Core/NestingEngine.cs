using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ModernNesting.Models;

namespace ModernNesting.Core
{
    public class NestingEngine
    {
        private List<Sheet> availableSheets;
        private List<Part> parts;
        private NestingConfig config;

        public NestingEngine(List<Sheet> sheets, List<Part> parts, NestingConfig config)
        {
            this.availableSheets = sheets;
            this.parts = parts.OrderByDescending(p => p.Area).ToList();
            this.config = config;
        }

        public NestingResult ProcessNesting()
        {
            var result = new NestingResult();
            var remainingParts = new List<Part>(parts);
            var currentSheetIndex = 0;

            while (remainingParts.Any() && currentSheetIndex < availableSheets.Count)
            {
                var currentSheet = availableSheets[currentSheetIndex];
                var sheetResult = ProcessSheet(currentSheet, remainingParts);

                result.SheetResults.Add(sheetResult);
                remainingParts = remainingParts.Except(sheetResult.PlacedParts).ToList();
                currentSheetIndex++;
            }

            result.Success = !remainingParts.Any();
            result.UnplacedParts = remainingParts;
            CalculateEfficiency(result);

            return result;
        }

        private SheetResult ProcessSheet(Sheet sheet, List<Part> partsToPlace)
        {
            var sheetResult = new SheetResult { Sheet = sheet };
            var placedParts = new List<Part>();
            var positions = new Dictionary<Part, Point>();

            foreach (var part in partsToPlace)
            {
                var position = FindBestPosition(part, sheet, placedParts);
                if (position.HasValue)
                {
                    placedParts.Add(part);
                    positions[part] = position.Value;
                }
            }

            sheetResult.PlacedParts = placedParts;
            sheetResult.Positions = positions;
            return sheetResult;
        }

        private Point? FindBestPosition(Part part, Sheet sheet, List<Part> placedParts)
        {
            // Implementación básica - será mejorada con algoritmo genético
            // Por ahora solo verifica si cabe en la posición (0,0)
            if (sheet.CanFit(part))
            {
                return new Point(0, 0);
            }
            return null;
        }

        private void CalculateEfficiency(NestingResult result)
        {
            double totalSheetArea = 0;
            double totalUsedArea = 0;

            foreach (var sheetResult in result.SheetResults)
            {
                totalSheetArea += sheetResult.Sheet.Area;
                totalUsedArea += sheetResult.PlacedParts.Sum(p => p.Area);
            }

            result.Efficiency = totalUsedArea / totalSheetArea * 100;
        }
    }

    public class NestingConfig
    {
        public double Spacing { get; set; } = 1.0;
        public bool AllowRotation { get; set; } = true;
        public int MaxIterations { get; set; } = 1000;
        public double MinEfficiency { get; set; } = 70.0;
    }

    public class NestingResult
    {
        public List<SheetResult> SheetResults { get; set; } = new List<SheetResult>();
        public List<Part> UnplacedParts { get; set; } = new List<Part>();
        public bool Success { get; set; }
        public double Efficiency { get; set; }
    }

    public class SheetResult
    {
        public Sheet Sheet { get; set; }
        public List<Part> PlacedParts { get; set; } = new List<Part>();
        public Dictionary<Part, Point> Positions { get; set; } = new Dictionary<Part, Point>();
    }
}