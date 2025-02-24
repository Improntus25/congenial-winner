using System.Collections.Generic;
using ModernNesting.Models;

namespace ModernNesting.IO
{
    public interface IFileImporter
    {
        List<Part> Import(string filePath);
        bool CanHandle(string extension);
    }
}