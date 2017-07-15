using System.Collections.Generic;

namespace DataAccessLib
{
    public interface INumeralRepository
    {
        IDictionary<char, int> GetRomanToArabicMap();

        IDictionary<int, string> GetArabicToRomanMap();

        IEnumerable<string> GetIntergalacticProducts();

        IEnumerable<string> GetEarthyProducts();

        IEnumerable<string> GetInputFileContents();
    }
}