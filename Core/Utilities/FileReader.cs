using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Utilities
{
    /// <summary>
    /// Light-weight file reader to reader files without locking the file
    /// </summary>
    public class FileReader : IFileReader
    {
        private readonly bool removeWhitespaces;

        public FileReader()
        {
            this.removeWhitespaces = true;
        }

        public FileReader(bool removeWhitespaces)
        {
            this.removeWhitespaces = removeWhitespaces;
        }

        public IEnumerable<string> ReadLinesFromInputFile(string filePath)
        {
            List<string> lines = new List<string>();

            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = this.removeWhitespaces ? Regex.Replace(reader.ReadLine().Trim(), @"\s+", " ") : reader.ReadLine();

                        lines.Add(line);
                    }
                }
            }

            return lines;
        }
    }
}