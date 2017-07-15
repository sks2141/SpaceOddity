using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using ValidationLib;

namespace NumeralCalculator.Validator
{
    public class ProductDataValidator : IProductDataValidator
    {
        private const string CREDITS_TEXT = "Credits";

        private readonly IRuleValidationEngine validator;
        private readonly IProductCategorizer productCategorizer;

        private readonly ICollection<string> productsList;

        public ProductDataValidator(
            IRuleValidationEngine validator,
            IProductCategorizer productCategorizer,
            ICollection<string> productsList)
        {
            if (productsList == null)
            {
                throw new ArgumentNullException("productsList");
            }

            this.validator = validator;
            this.productCategorizer = productCategorizer;
            this.productsList = productsList;
        }

        public IEnumerable<string> GetProducts(string content, int lineNumber)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
            {
                SLogger.LogWarnFormat("Line number {0} does not have a valid product. Skipping...", lineNumber);
                return null;
            }

            string[] products = content.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string product in products)
            {
                if (!this.UpdateProductCache(product, lineNumber))
                {
                    return null;
                }
            }

            return products;
        }

        public string GetProductValue(string content, int lineNumber)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
            {
                SLogger.LogWarnFormat("Line number {0} does not have a valid product value. Skipping...", lineNumber);
                return string.Empty;
            }

            int creditsIndex = -1;
            if (this.DoesContentContainArabicNumerals(content, out creditsIndex) &&
                creditsIndex > 0)
            {
                // Assuming that Credits is the last text in the content.
                if (!(content.Reverse().ToString().StartsWith(CREDITS_TEXT.Reverse().ToString())))
                {
                    SLogger.LogWarn("Credits text is not the last word of the content. Skipping...");

                    return string.Empty;
                }

                // Assuming that Credits are linked to Arabic Numerals
                int number = 0;
                string numberString = content.Substring(0, creditsIndex - 1).Trim();
                int.TryParse(numberString, out number);

                string error = this.validator.Validate(number);
                if (!string.IsNullOrEmpty(error))
                {
                    SLogger.LogWarnFormat("Validation issue found for arabic numeric value: {0} \nError: {1} on Line Number: {2}. Skipping...",
                        numberString, error, lineNumber);

                    return string.Empty;
                }

                return number.ToString();
            }
            else
            {
                // Else, the value is in Roman numeral representation
                string romanNumeral = content.Trim();
                string error = this.validator.Validate(content.Trim());
                if (!string.IsNullOrEmpty(error))
                {
                    SLogger.LogWarnFormat("Validation issue found for roman numberal: {0} \nError: {1} on Line Number: {2}. Skipping...",
                        romanNumeral, error, lineNumber);

                    return string.Empty;
                }

                return romanNumeral;
            }
        }

        private bool UpdateProductCache(string product, int lineNumber)
        {
            if (!this.productsList.Contains(product))
            {
                if (this.productCategorizer.IsProductValid(product))
                {
                    SLogger.LogInfoFormat("Line number {0} contains valid product:{1}.", lineNumber, product);
                    this.productsList.Add(product);
                    return true;
                }
                else
                {
                    SLogger.LogWarnFormat("Line number {0} contained an invalid product entry:{1}, Skipping...", lineNumber, product);
                    return false;
                }
            }

            SLogger.LogInfoFormat("Line number {0} contains valid product:{1}. Found in 'in-memory' list of products. Skipping adding to it.", lineNumber, product);
            return true;
        }

        /// <summary>
        /// Assuming that Credits are linked to ArabicNumerable
        /// </summary>
        private bool DoesContentContainArabicNumerals(string content, out int index)
        {
            index = content.IndexOf(CREDITS_TEXT, StringComparison.OrdinalIgnoreCase);
            return  index != -1;
        }
    }
}