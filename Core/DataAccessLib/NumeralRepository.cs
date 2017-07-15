using System.Collections.Generic;
using Utilities;

namespace DataAccessLib
{
    public class NumeralRepository : INumeralRepository
    {
        private readonly IFileReader reader;
        private readonly string intergalacticProductsPath;
        private readonly string earthyProductsPath;
        private readonly string inputFilePath;

        public NumeralRepository(
            IFileReader reader,
            string intergalacticProductsPath, 
            string earthyProductsPath,
            string inputFilePath)
        {
            this.reader = reader;
            this.intergalacticProductsPath = intergalacticProductsPath;
            this.earthyProductsPath = earthyProductsPath;
            this.inputFilePath = inputFilePath;
        }

        public IDictionary<char, int> GetRomanToArabicMap()
        {
            IDictionary<char, int> numerals = new Dictionary<char, int>();

            numerals.Add('I', 1);
            numerals.Add('V', 5);
            numerals.Add('X', 10);
            numerals.Add('L', 50);
            numerals.Add('C', 100);
            numerals.Add('D', 500);
            numerals.Add('M', 1000);

            return numerals;
        }

        public IDictionary<int, string> GetArabicToRomanMap()
        {
            IDictionary<int, string> numerals = new Dictionary<int, string>();

            numerals.Add(1000, "M");
            numerals.Add(900, "CM");
            numerals.Add(500, "D");
            numerals.Add(400, "CD");
            numerals.Add(100, "C");
            numerals.Add(90, "XC");
            numerals.Add(50, "L");
            numerals.Add(40, "XL");
            numerals.Add(10, "X");
            numerals.Add(9, "IX");
            numerals.Add(5, "V");
            numerals.Add(4, "IV");
            numerals.Add(1, "I");

            return numerals;
        }
        
        public IEnumerable<string> GetIntergalacticProducts()
        {
            return this.reader.ReadLinesFromInputFile(this.intergalacticProductsPath);
        }

        public IEnumerable<string> GetEarthyProducts()
        {
            return this.reader.ReadLinesFromInputFile(this.earthyProductsPath);
        }

        public IEnumerable<string> GetInputFileContents()
        {
            return this.reader.ReadLinesFromInputFile(this.inputFilePath);
        }
    }
}