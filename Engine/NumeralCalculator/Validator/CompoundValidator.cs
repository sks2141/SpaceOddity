using System.Collections.Generic;
using System.Linq;
using Utilities;
using ValidationLib;

namespace NumeralCalculator.Validator
{
    public class CompoundValidator : ICompoundValidator
    {
        private readonly IProductCategorizer productCategorizer;
        private readonly IProductHelper productHelper;
        private readonly IRuleValidationEngine validator;
        
        public CompoundValidator(
            IProductCategorizer productCategorizer,
            IProductHelper productHelper,
            IRuleValidationEngine validator)
        {
            this.productCategorizer = productCategorizer;
            this.productHelper = productHelper;
            this.validator = validator;
        }

        public bool IsProductRepresentationValid(IEnumerable<string> products, string productValue, 
            InputType typeOfInput, int lineNumber, IDictionary<string, string> intergalacticProductsCache = null)
        {
            if (products == null || !products.Any())
            {
                SLogger.LogWarnFormat("At Line number: {0}, there were no products found. Skipping.", lineNumber);
                return false;
            }

            if (products.Count() == 1)
            {
                string product = products.First();
                return this.IsProductValueValid(product, productValue, typeOfInput, lineNumber);
            }
            else
            {
                if (!IsInputValid(products, productValue, typeOfInput, lineNumber))
                {
                    return false;
                }
                
                // Check if multiple Earthy products are mentioned in a single line.
                if (products.Where(p => this.productCategorizer.IsProductEarthy(p)).Count() > 1)
                {
                    SLogger.LogWarnFormat("At Line number: {0}, there are multiple Earthy products. Invalid case.", lineNumber);
                    return false;
                }

                bool isEarthyProductValid = IsEarthyProductValid(products, productValue, lineNumber, 
                                                                 typeOfInput, intergalacticProductsCache);
                if (!isEarthyProductValid)
                {
                    return false;
                }
            }
            
            return true;
        }

        private bool IsInputValid(IEnumerable<string> products, string productValue, InputType typeOfInput, int lineNumber)
        {
            if (typeOfInput == InputType.ProductData)
            {
                // For Input Data (ProductData), Check if multiple Intergalactic products, without Earthy products, are mentioned in a single assignment line.
                if (products.Where(p => this.productCategorizer.IsProductIntergalactic(p)).Count() > 1 &&
                    !products.Any(p => this.productCategorizer.IsProductEarthy(p)))
                {
                    SLogger.LogWarnFormat("At Line number: {0}, there are multiple Intergalactic Products and no Earthy products in this assignment line (Product Data Input Type). Invalid case.", lineNumber);
                    return false;
                }

                // Check if the unit associated with the productValue for intergalactic products is Arabic numeral
                if (!products.Any(p => this.productCategorizer.IsProductEarthy(p)))
                {
                    foreach (string product in products.Where(p => this.productCategorizer.IsProductIntergalactic(p)))
                    {
                        bool isValid = this.IsProductValueValid(product, productValue, typeOfInput, lineNumber);
                        if (!isValid)
                        {
                            return isValid;
                        }
                    }
                }
            }

            return true;
        }

        private bool IsProductValueValid(string product, string productValue, InputType typeOfInput, int lineNumber)
        {
            if (typeOfInput == InputType.ProductData && !string.IsNullOrEmpty(productValue))
            {
                string error = null;

                if (this.productCategorizer.IsProductIntergalactic(product))
                {
                    // Check if intergalactic product has arabic numeral unit
                    error = this.validator.Validate(productValue);

                    if(!string.IsNullOrEmpty(error))
                    {
                        SLogger.LogWarnFormat("At Line number: {0}, Intergalactic product:{1} does not have proper roman numeral value. Value:{2}. Error:{3}",
                            lineNumber, product, productValue, error);

                        return false;
                    }
                }
                else if (this.productCategorizer.IsProductEarthy(product))
                {
                    // Check if earthy product has roman numeral unit
                    int value = 0;
                    int.TryParse(productValue, out value);

                    error = this.validator.Validate(value);
                    if (!string.IsNullOrEmpty(error))
                    {
                        SLogger.LogWarnFormat("At Line number: {0}, Earthy product:{1} does not have proper arabic numeral value. Value:{2}. Error:{3}",
                            lineNumber, product, productValue, error);

                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsEarthyProductValid(IEnumerable<string> products, string productValue, int lineNumber,
            InputType typeOfInput, IDictionary<string, string> intergalacticProductsCache = null)
        {
            if (products.Any(p => this.productCategorizer.IsProductEarthy(p)))
            {
                string earthyProduct = products.Last();

                // Check if Earthy product, if any, are not mentioned at the end of the list
                if (earthyProduct != products.SingleOrDefault(p => this.productCategorizer.IsProductEarthy(p)))
                {
                    SLogger.LogWarnFormat("At Line number: {0}, Earthy product should be last referenced.");
                    return false;
                }

                // Check if the intergalactic representation 'tagged' with earthy product is proper
                if (intergalacticProductsCache != null && intergalacticProductsCache.Any())
                {
                    IEnumerable<string> intergalacticProducts = products.Where(p => this.productCategorizer.IsProductIntergalactic(p));
                    if (intergalacticProducts.Any())
                    {
                        string romanNumeral = this.productHelper.GetMultiplierValue(intergalacticProducts, intergalacticProductsCache);
                        string error = this.validator.Validate(romanNumeral.ToString());

                        if (!string.IsNullOrEmpty(error))
                        {
                            // An error scenario - due to Logic Flaw in processing one of the special input! - This should not crash the app - no need to throw this error.
                            SLogger.LogErrorFormat("At Line number: {0}, error : {1}, while validating roman numberal : {2}.", lineNumber, error, romanNumeral);
                            return false;
                        }
                    }
                }

                // Followup check if the unit association to valid Earthy product is Roman Numeral
                return this.IsProductValueValid(earthyProduct, productValue, typeOfInput, lineNumber);
            }

            return true;
        }
    }
}