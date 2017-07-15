using System.Collections.Generic;

namespace Utilities
{
    public interface IFileReader
    {
        IEnumerable<string> ReadLinesFromInputFile(string filePath);
    }
}