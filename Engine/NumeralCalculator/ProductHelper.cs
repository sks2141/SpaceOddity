using NumeralCalculator.Converter;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace NumeralCalculator
{
    public class ProductHelper : IProductHelper
    {
        private readonly IConverterStrategy converter;

        public ProductHelper(IConverterStrategy converter)
        {
            this.converter = converter;
        }

        public decimal GetEarthyProductValue(
            string earthyProduct, string productValue,
            IEnumerable<string> intergalacticProducts, 
            IDictionary<string, string> intergalacticProductsCache)
        {
            decimal value = 0;
            decimal.TryParse(productValue, out value);

            if (intergalacticProducts == null || !intergalacticProducts.Any())
            {
                SLogger.LogWarnFormat("No intergalacticProducts tagged with earthyProduct:{0}. Returning value:{1} for string value of:{2}",
                    earthyProduct, value, productValue);

                return value;
            }

            int multiplier = this.GetMultiplierInArabicNumeralForm(intergalacticProducts, intergalacticProductsCache);

            if (multiplier > 0)
            {
                return value / multiplier;
            }

            SLogger.LogErrorFormat("Invalid Multiplier: {0} returned for earthyProduct:{1} with value:{2} - Tagged IntergalacticProducts:{3}",
                multiplier, earthyProduct, productValue, string.Join(", ", intergalacticProducts));

            return 0;
        }

        public int GetMultiplierInArabicNumeralForm(IEnumerable<string> intergalacticProducts, IDictionary<string, string> intergalacticProductsCache)
        {
            string multiplierValue = this.GetMultiplierValue(intergalacticProducts, intergalacticProductsCache);
            int multiplier = this.converter.ConvertToArabicNumeral(multiplierValue);

            return multiplier;
        }

        public string GetMultiplierValue(IEnumerable<string> intergalacticProducts, IDictionary<string, string> intergalacticProductsCache)
        {
            if (intergalacticProducts != null && intergalacticProducts.Any() && 
                intergalacticProductsCache != null && intergalacticProductsCache.Any())
            {
                StringBuilder romanNumeral = new StringBuilder();

                foreach (string intergalacticProduct in intergalacticProducts)
                {
                    if (!intergalacticProductsCache.ContainsKey(intergalacticProduct))
                    {
                        // An error scenario, which should not crash the app - no need to throw this error.
                        SLogger.LogErrorFormat("Intergalactic product {0} is not found in the cache.", intergalacticProduct);
                        return null;
                    }

                    if (string.IsNullOrEmpty(intergalacticProductsCache[intergalacticProduct]))
                    {
                        // An error scenario, which should not crash the app - no need to throw this error.
                        SLogger.LogErrorFormat("Null value found for Intergalactic product {0} in the cache.", intergalacticProduct);
                        return null;
                    }

                    romanNumeral.Append(intergalacticProductsCache[intergalacticProduct]);
                }

                return romanNumeral.ToString();
            }

            return null;
        }
    }
}