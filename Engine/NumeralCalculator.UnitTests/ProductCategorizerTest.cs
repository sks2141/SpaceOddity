using NUnit.Framework;
using System;

namespace NumeralCalculator.UnitTests
{
    [TestFixture]
    public class ProductCategorizerTest
    {
        public static readonly string[] intergalacticProducts = { "glob", "prok", "pish", "tegj" };
        public static readonly string[] earthyProducts = { "Silver", "Gold", "Iron" };
        
        [TestCaseSource("GetNullTestCases")]
        [Test, RequiresThread]
        public void Categorizer_NullEntries(string[] intergalacticalProducts, string[] earthyProducts)
        {
            Assert.That(() => new ProductCategorizer(intergalacticalProducts, earthyProducts), Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase("", ExpectedResult = false)]
        [TestCase("Silver", ExpectedResult = false)]
        [TestCase("glob1", ExpectedResult = false)]
        [TestCase("glob", ExpectedResult = true)]
        [Test, RequiresThread]
        public bool IsProductIntergalactic_TestSet(string product)
        {
            var categorizer = new ProductCategorizer(intergalacticProducts, earthyProducts);

            return categorizer.IsProductIntergalactic(product);
        }

        [TestCase("", ExpectedResult = false)]
        [TestCase("GoldSilver", ExpectedResult = false)]
        [TestCase("glob1", ExpectedResult = false)]
        [TestCase("glob", ExpectedResult = false)]
        [TestCase("Silver", ExpectedResult = true)]
        [Test, RequiresThread]
        public bool IsProductEarthy_TestSet(string product)
        {
            var categorizer = new ProductCategorizer(intergalacticProducts, earthyProducts);

            return categorizer.IsProductEarthy(product);
        }

        [TestCase("", ExpectedResult = false)]
        [TestCase("IronSilver", ExpectedResult = false)]
        [TestCase("glob1", ExpectedResult = false)]
        [TestCase("glob", ExpectedResult = true)]
        [TestCase("Silver", ExpectedResult = true)]
        [Test, RequiresThread]
        public bool IsProductValid_TestSet(string product)
        {
            var categorizer = new ProductCategorizer(intergalacticProducts, earthyProducts);

            return categorizer.IsProductValid(product);
        }

        [TestCase("", ExpectedResult = ProductType.Unassigned)]
        [TestCase("IronSilver", ExpectedResult = ProductType.Unassigned)]
        [TestCase("glob1", ExpectedResult = ProductType.Unassigned)]
        [TestCase("glob", ExpectedResult = ProductType.Intergalactic)]
        [TestCase("Silver", ExpectedResult = ProductType.Earthy)]
        [Test, RequiresThread]
        public ProductType GetProductType_TestSet(string product)
        {
            var categorizer = new ProductCategorizer(intergalacticProducts, earthyProducts);

            return categorizer.GetProductType(product);
        }

        private static TestCaseData[] GetNullTestCases()
        {
            return new[]
            {
                new TestCaseData(null, null),
                new TestCaseData(null, earthyProducts),
                new TestCaseData(intergalacticProducts, null)
            };
        }
    }
}