using Moq;
using NumeralCalculator.Converter;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NumeralCalculator.UnitTests
{
    [TestFixture]
    public class ProductHelperTest
    {
        private Mock<IConverterStrategy> converter;
        private IProductHelper productHelper;

        [SetUp]
        public void Setup()
        {
            this.converter = new Mock<IConverterStrategy>(MockBehavior.Strict);
            this.productHelper = new ProductHelper(this.converter.Object);
        }

        [TestCaseSource("GetNullTestCases")]
        [Test, RequiresThread]
        public void GetGetEarthyProductValue_NullTestCases(IEnumerable<string> products, IDictionary<string, string> productsCache)
        {
            this.converter.Setup(i => i.ConvertToArabicNumeral(null))
                          .Returns(It.IsAny<int>())
                          .Verifiable();

            var result = this.productHelper.GetEarthyProductValue("Silver", "34", products, productsCache);

            if (products == null || !products.Any())
            {
                Assert.IsTrue(result == 34);
                this.converter.Verify(i => i.ConvertToArabicNumeral(It.IsAny<string>()), Times.Never);
            }
            else
            {
                Assert.Zero(result);
                this.converter.Verify(i => i.ConvertToArabicNumeral(It.IsAny<string>()), Times.Once);
            }
        }

        [Test, RequiresThread]
        public void GetGetEarthyProductValue_ZeroMultiplier()
        {
            this.converter.Setup(i => i.ConvertToArabicNumeral(It.IsAny<string>()))
                          .Returns(0)
                          .Verifiable();

            var result = this.productHelper.GetEarthyProductValue("Silver", "34", GetProducts(), GetProductsCache());
            
            Assert.Zero(result);
            this.converter.Verify(i => i.ConvertToArabicNumeral(It.IsAny<string>()), Times.Once);
        }

        [Test, RequiresThread]
        public void GetGetEarthyProductValue_ValidCase()
        {
            this.converter.Setup(i => i.ConvertToArabicNumeral(It.IsAny<string>()))
                          .Returns(2)
                          .Verifiable();

            var result = this.productHelper.GetEarthyProductValue("Silver", "34", GetProducts(), GetProductsCache());

            Assert.IsTrue(result == 17);
            this.converter.Verify(i => i.ConvertToArabicNumeral(It.IsAny<string>()), Times.Once);
        }

        [TestCaseSource("GetNullTestCases")]
        [Test, RequiresThread]
        public void GetMultiplierInArabicNumeralForm_NullTestCases(IEnumerable<string> products, IDictionary<string, string> productsCache)
        {
            this.converter.Setup(i => i.ConvertToArabicNumeral(null))
                          .Returns(It.IsAny<int>())
                          .Verifiable();

            var result = this.productHelper.GetMultiplierInArabicNumeralForm(products, productsCache);

            Assert.Zero(result);
            this.converter.Verify(i => i.ConvertToArabicNumeral(It.IsAny<string>()), Times.Once);
        }

        [Test, RequiresThread]
        public void GetMultiplierInArabicNumeralForm_ValidCases()
        {
            this.converter.Setup(i => i.ConvertToArabicNumeral(It.IsAny<string>()))
                          .Returns(34)
                          .Verifiable();

            var result = this.productHelper.GetMultiplierInArabicNumeralForm(GetProducts(), GetProductsCache());

            Assert.IsTrue(result != 0);
            this.converter.Verify(i => i.ConvertToArabicNumeral(It.IsAny<string>()), Times.Once);
        }
        
        [TestCaseSource("GetNullTestCases")]
        [Test, RequiresThread]
        public void GetMultiplierValue_NullTestCases(IEnumerable<string> products, IDictionary<string, string> productsCache)
        {
            var result = this.productHelper.GetMultiplierValue(products, productsCache);

            Assert.IsNull(result);
            this.converter.Verify(i => i.ConvertToArabicNumeral(It.IsAny<string>()), Times.Never);
        }

        [Test, RequiresThread]
        public void GetMultiplierValue_ProductNotInCache()
        {
            var products = new[] { "glob", "prok", "phish", "tegj" };
            var productsCache = new Dictionary<string, string> { { "glob", "I" }, { "prok", "V" } };

            var result = this.productHelper.GetMultiplierValue(products, productsCache);

            Assert.IsNull(result);
            this.converter.Verify(i => i.ConvertToArabicNumeral(It.IsAny<string>()), Times.Never);
        }

        [Test, RequiresThread]
        public void GetMultiplierValue_ProductCacheValueNullOrEmpty()
        {
            var products = new[] { "glob", "prok", "phish", "tegj" };
            var productsCache = new Dictionary<string, string> { { "glob", "I" }, { "prok", "" } };

            var result = this.productHelper.GetMultiplierValue(products, productsCache);

            Assert.IsNull(result);
            this.converter.Verify(i => i.ConvertToArabicNumeral(It.IsAny<string>()), Times.Never);
        }

        [Test, RequiresThread]
        public void GetMultiplierValue_Valid()
        {
            var result = this.productHelper.GetMultiplierValue(GetProducts(), GetProductsCache());

            Assert.IsNotNull(result);
            this.converter.Verify(i => i.ConvertToArabicNumeral(It.IsAny<string>()), Times.Never);
        }

        private static TestCaseData[] GetNullTestCases()
        {
            return new[]
            {
                new TestCaseData(null, null),
                new TestCaseData(null, GetProductsCache()),
                new TestCaseData(null, new Dictionary<string, string>()),
                new TestCaseData(GetProducts(), null),
                new TestCaseData(new Collection<string>(), null)
            };
        }

        private static IEnumerable<string> GetProducts()
        {
            return new[] { "glob", "prok", "phish", "tegj" };
        }


        private static IDictionary<string, string> GetProductsCache()
        {
            return new Dictionary<string, string> { { "glob", "I" }, { "prok", "V" }, { "phish", "X" }, { "tegj", "L" } };
        }
    }
}