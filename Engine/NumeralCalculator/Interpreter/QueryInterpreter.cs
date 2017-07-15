using NumeralCalculator.Validator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace NumeralCalculator.Interpreter
{
    public class QueryInterpreter : IInterpreter<string>
    {
        private const string INVALID_RESPONSE = "I have no idea what you are talking about";
        private const string CREDITS_TEXT = "Credits";

        private readonly IProductCategorizer productCategorizer;
        private readonly IProductHelper productHelper;
        private readonly ICompoundValidator validator;
        private readonly IDictionary<string, string> intergalacticProductsCache;
        private readonly IDictionary<string, decimal> earthyProductsCache;

        public QueryInterpreter(
            IProductCategorizer productCategorizer,
            IProductHelper productHelper,
            ICompoundValidator validator,
            IDictionary<string, string> intergalacticProductsCache,
            IDictionary<string, decimal> earthyProductsCache)
        {
            if (intergalacticProductsCache == null)
            {
                throw new ArgumentNullException("intergalacticProductsCache");
            }

            if (earthyProductsCache == null)
            {
                throw new ArgumentNullException("earthyProductsCache");
            }

            this.productCategorizer = productCategorizer;
            this.productHelper = productHelper;
            this.validator = validator;
            this.intergalacticProductsCache = intergalacticProductsCache;
            this.earthyProductsCache = earthyProductsCache;
        }

        public string Interpret(Tuple<int, IEnumerable<string>, string> content)
        {
            int lineNumber = content.Item1;
            IEnumerable<string> products = content.Item2;
            string responseFormat = content.Item3;

            SLogger.LogInfoFormat("Interpreting Query at line number {0}.", lineNumber);

            if (products == null || !products.Any())
            {
                return INVALID_RESPONSE;
            }
            
            if (!this.validator.IsProductRepresentationValid(products, null, InputType.Query, lineNumber, intergalacticProductsCache))
            {
                return INVALID_RESPONSE;
            }

            OutputUnitType outputUnit = this.GetOutputUnitType(responseFormat);

            return this.InterpretProducts(products, outputUnit);
        }

        private string InterpretProducts(IEnumerable<string> products, OutputUnitType unitType)
        {
            bool validResponse = false;

            StringBuilder response = new StringBuilder(string.Format("{0} is ", string.Join(" ", products)));

            if (products.All(p => this.productCategorizer.GetProductType(p) == ProductType.Intergalactic))
            {
                response.Append(this.productHelper.GetMultiplierInArabicNumeralForm(products, this.intergalacticProductsCache));
                validResponse = true;
            }

            if (this.productCategorizer.GetProductType(products.Last()) == ProductType.Earthy)
            {
                if (products.Any(p => this.productCategorizer.GetProductType(p) == ProductType.Intergalactic))
                {
                    int multiplier = this.productHelper.GetMultiplierInArabicNumeralForm(
                        products.Where(p => this.productCategorizer.GetProductType(p) == ProductType.Intergalactic),
                        this.intergalacticProductsCache);

                    decimal value = multiplier * this.earthyProductsCache[products.Last()];

                    response.Append(this.GetDecimalString(value));
                    validResponse = true;
                }
                else
                {
                    response.Append(this.GetDecimalString(this.earthyProductsCache[products.Last()]));
                    validResponse = true;
                }
            }

            if (validResponse)
            {
                if (unitType == OutputUnitType.ArabicNumeralWithCredits)
                {
                    response.Append(" ");
                    response.Append(CREDITS_TEXT);
                }

                return response.ToString();
            }

            SLogger.LogWarnFormat("Failed to get a response. Returning empty response. Product Details:{0} for unitType:{1}", 
                string.Join(", ", products), unitType);

            return INVALID_RESPONSE;
        }

        private string GetDecimalString(decimal value)
        {
            string valueString = value.ToString("0.##"); // format to 2 decimal places
            return valueString.Replace(".0", string.Empty);
        }

        private OutputUnitType GetOutputUnitType(string responseFormat)
        {
            if (responseFormat == CREDITS_TEXT)
            {
                return OutputUnitType.ArabicNumeralWithCredits;
            }

            return OutputUnitType.ArabicNumeralWithoutCredits;
        }
    }
}