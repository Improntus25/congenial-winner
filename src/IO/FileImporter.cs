using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModernNesting.Models;

namespace ModernNesting.IO
{
    public class FileImporter
    {
        private readonly List<IFileImporter> importers;

        public FileImporter()
        {
            importers = new List<IFileImporter>
            {
                new DxfImporter(),
                // Se añadirán más importadores aquí
            };
        }

        public List<Part> ImportFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file does not exist.", filePath);
            }

            var extension = Path.GetExtension(filePath).ToLower();
            var importer = importers.FirstOrDefault(i => i.CanHandle(extension));

            if (importer == null)
            {
                throw new NotSupportedException($"File type {extension} is not supported");
            }

            try
            {
                var parts = importer.Import(filePath);

                // Post-processing
                foreach (var part in parts)
                {
                    // Ensure unique names
                    if (string.IsNullOrEmpty(part.Name))
                    {
                        part.Name = $"Part_{Guid.NewGuid().ToString().Substring(0, 8)}";
                    }

                    // Validate geometry
                    if (part.Width <= 0 || part.Height <= 0)
                    {
                        throw new InvalidDataException($"Invalid dimensions for part {part.Name}");
                    }
                }

                return parts;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error importing file: {ex.Message}", ex);
            }
        }

        public bool CanImportFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;

            var extension = Path.GetExtension(filePath).ToLower();
            return importers.Any(i => i.CanHandle(extension));
        }

        public string GetSupportedFileTypes()
        {
            return "All Supported Files|*.dxf;*.svg;*.ai;*.cdr|" +
                   "DXF Files (*.dxf)|*.dxf|" +
                   "SVG Files (*.svg)|*.svg|" +
                   "Adobe Illustrator Files (*.ai)|*.ai|" +
                   "CorelDRAW Files (*.cdr)|*.cdr";
        }

        public List<string> GetSupportedExtensions()
        {
            return new List<string> { ".dxf", ".svg", ".ai", ".cdr" };
        }
    }
}