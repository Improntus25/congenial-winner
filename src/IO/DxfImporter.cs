using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ModernNesting.Models;
using netDxf;
using netDxf.Entities;
using System.Collections.ObjectModel;
using System.Collections;

namespace ModernNesting.IO
{
    public class DxfImporter : IFileImporter
    {
        public bool CanHandle(string extension)
        {
            return extension.ToLower() == ".dxf";
        }

        public List<Part> Import(string filePath)
        {
            var parts = new List<Part>();
            try
            {
                var doc = DxfDocument.Load(filePath);
                if (doc == null) return parts;

                // Process polylines
                var polylines = new List<LwPolyline>();
                foreach (var entity in doc.Entities)
                {
                    if (entity is LwPolyline polyline)
                    {
                        polylines.Add(polyline);
                    }
                }

                foreach (var lwPolyline in polylines)
                {
                    var part = ProcessPolyline(lwPolyline);
                    if (part != null)
                    {
                        parts.Add(part);
                    }
                }

                // Process lines
                var lines = new List<Line>();
                foreach (var entity in doc.Entities)
                {
                    if (entity is Line line)
                    {
                        lines.Add(line);
                    }
                }

                if (lines.Any())
                {
                    var lineGroups = GroupConnectedLines(lines);
                    foreach (var lineGroup in lineGroups)
                    {
                        var part = ProcessLineGroup(lineGroup);
                        if (part != null)
                        {
                            parts.Add(part);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error importing DXF: {ex.Message}");
            }

            return parts;
        }

        // ... resto del código igual ...
    }
}