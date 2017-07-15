using System;
using System.Collections.Generic;
using System.Linq;

namespace NumeralCalculator
{
    public class ProductCategorizer : IProductCategorizer
    {
        private readonly IEnumerable<string> intergalacticProducts;
        private readonly IEnumerable<string> earthyProducts;

        public ProductCategorizer(IEnumerable<string> intergalacticProducts, IEnumerable<string> earthyProducts)
        {
            if (intergalacticProducts == null || !intergalacticProducts.Any())
            {
                throw new ArgumentNullException("intergalacticProducts");
            }

            if (earthyProducts == null || !earthyProducts.Any())
            {
                throw new ArgumentNullException("earthyProducts");
            }

            this.intergalacticProducts = intergalacticProducts;
            this.earthyProducts = earthyProducts;
        }

        public bool IsProductIntergalactic(string product)
        {
            return this.intergalacticProducts.Contains(product);
        }

        public bool IsProductEarthy(string product)
        {
            return this.earthyProducts.Contains(product);
        }

        public bool IsProductValid(string product)
        {
            return this.IsProductIntergalactic(product) || this.IsProductEarthy(product);
        }

        public ProductType GetProductType(string product)
        {
            if (this.IsProductIntergalactic(product))
            {
                return ProductType.Intergalactic;
            }

            if (this.IsProductEarthy(product))
            {
                return ProductType.Earthy;
            }

            return ProductType.Unassigned;
        }
    }
}