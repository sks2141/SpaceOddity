using System.Collections.Generic;

namespace NumeralCalculator
{
    public interface IProductHelper
    {
        decimal GetEarthyProductValue(
            string earthyProduct, string productValue, 
            IEnumerable<string> intergalacticProducts,
            IDictionary<string, string> intergalacticProductsCache);

        int GetMultiplierInArabicNumeralForm(IEnumerable<string> intergalacticProducts, IDictionary<string, string> intergalacticProductsCache);

        string GetMultiplierValue(IEnumerable<string> intergalacticProducts, IDictionary<string, string> intergalacticProductsCache);
    }
}