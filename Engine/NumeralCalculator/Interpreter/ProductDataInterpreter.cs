using NumeralCalculator.Validator;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace NumeralCalculator.Interpreter
{
    /// <summary>
    /// For product data input, 
    ///     only one intergalactic product assignment is valid.
    ///     earthy product's value is derived via ProductHelper's func.
    /// </summary>
    public class ProductDataInterpreter : IInterpreter<int>
    {
        private readonly IProductCategorizer productCategorizer;
        private readonly ICompoundValidator validator;
        private readonly IProductHelper productHelper;
        private readonly IDictionary<string, string> intergalacticProductsCache;
        private readonly IDictionary<string, decimal> earthyProductsCache;
        
        public ProductDataInterpreter(
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

        public int Interpret(Tuple<int, IEnumerable<string>, string> content)
        {
            int lineNumber = content.Item1;
            IEnumerable<string> products = content.Item2;
            string productValue = content.Item3;

            SLogger.LogInfoFormat("Interpreting Product(s) at line number {0}.", lineNumber);

            if (products == null || !products.Any())
            {
                return 0;
            }

            if (!this.validator.IsProductRepresentationValid(products, productValue, InputType.ProductData, lineNumber, intergalacticProductsCache))
            {
                return 0;
            }

            this.InterpretProducts(products, productValue);
            return 1;
        }

        private void InterpretProducts(IEnumerable<string> products, string productValue)
        {
            foreach (string product in products)
            {
                ProductType productType = this.productCategorizer.GetProductType(product);

                switch (productType)
                {
                    case ProductType.Intergalactic:
                    {
                        this.InterpretProduct(this.intergalacticProductsCache, product, productValue);
                    }
                    break;

                    case ProductType.Earthy:
                    {
                        this.InterpretProduct(this.earthyProductsCache, product, 
                            this.productHelper.GetEarthyProductValue(
                                product, productValue,
                                products.Where(p => this.productCategorizer.IsProductIntergalactic(p)),
                                this.intergalacticProductsCache));
                    }
                    break;

                    default:
                        throw new InvalidOperationException("Invalid ProductType Enum.");
                }
            }
        }

        private void InterpretProduct<T>(IDictionary<string, T> productCache, string product, T productValue)
        {
            if (!productCache.ContainsKey(product))
            {
                productCache.Add(product, productValue);
            }
            else
            {
                SLogger.LogDebugFormat("ProductCache:{0} already contains the product:{1} with value:{2}. New value that missed update:{3}" +
                    "\n This can possibly be a false positive due to Intergalactic Products being 'Tagged' with Earthy Products.",
                    productCache.GetType(), product, productCache[product], productValue);
            }
        }
    }
}