using System;
using System.Collections.Generic;
using Utilities;

namespace NumeralCalculator.Validator
{
    public class QueryValidator : IQueryValidator
    {
        private readonly ICollection<string> productsList;

        public QueryValidator(ICollection<string> productsList)
        {
            if (productsList == null)
            {
                throw new ArgumentNullException("productsList");
            }

            this.productsList = productsList;
        }

        public IEnumerable<string> GetProducts(string content, int lineNumber)
        {
            SLogger.LogDebugFormat("Line number {0} is a question.", lineNumber);

            if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
            {
                SLogger.LogWarnFormat("Line number {0} does not have valid product(s). Skipping...", lineNumber);
                return null;
            }

            string[] products = content.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string product in products)
            {
                if (!this.productsList.Contains(product))
                {
                    SLogger.LogInfoFormat("Product List (derived by reading the input lines) does not contain product uptil now:{0} at line number {1}.\nSkipping...",
                                          product, lineNumber);

                    return null;
                }
            }

            return products;
        }
    }
}