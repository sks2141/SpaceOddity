using Moq;
using NumeralCalculator.Interpreter;
using NumeralCalculator.Validator;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NumeralCalculator.UnitTests.Interpreter
{
    [TestFixture]
    public class ProductDataInterpreterTest
    {
        private Mock<IProductCategorizer> productCategorizer;
        private Mock<IProductHelper> productHelper;
        private Mock<ICompoundValidator> validator;
        
        private IInterpreter<int> interpreter;

        [SetUp]
        public void Setup()
        {
            this.productCategorizer = new Mock<IProductCategorizer>(MockBehavior.Strict);
            this.productHelper = new Mock<IProductHelper>(MockBehavior.Strict);
            this.validator = new Mock<ICompoundValidator>(MockBehavior.Strict);
        }

        [TestCaseSource("GetNullDependencies")]
        [Test, RequiresThread]
        public void Interpret_NullDependencies(
            IDictionary<string, string> intergalacticProductsCache,
            IDictionary<string, decimal> earthyProductsCache)
        {
            Assert.That(() =>
                new ProductDataInterpreter(this.productCategorizer.Object,
                                     this.productHelper.Object,
                                     this.validator.Object,
                                     intergalacticProductsCache,
                                     earthyProductsCache),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test, RequiresThread]
        public void Interpret_NullProduct()
        {
            this.SetInterpreter();

            var content = new Tuple<int, IEnumerable<string>, string>(1, null, It.IsAny<string>());
            var result = this.interpreter.Interpret(content);

            Assert.Zero(result);
        }

        [Test, RequiresThread]
        public void Interpret_EmptyProducts()
        {
            this.SetInterpreter();

            var content = new Tuple<int, IEnumerable<string>, string>(1, new Collection<string> { }, It.IsAny<string>());
            var result = this.interpreter.Interpret(content);

            Assert.Zero(result);
        }

        [Test, RequiresThread]
        public void Interpret_InvalidProducts()
        {
            this.SetInterpreter();
            this.validator.Setup(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(false)
                .Verifiable();

            var content = new Tuple<int, IEnumerable<string>, string>(
                1, new Collection<string> { "yaba", "daba", "doo" }, It.IsAny<string>());

            var result = this.interpreter.Interpret(content);

            Assert.Zero(result);

            this.validator.Verify(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
        }

        [Test, RequiresThread]
        public void Interpret_InvalidProductType()
        {
            this.SetInterpreter();
            this.validator.Setup(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(true)
                .Verifiable();

            this.productCategorizer.Setup(i => i.GetProductType(It.IsAny<string>()))
                .Returns(ProductType.Unassigned)
                .Verifiable();

            var content = new Tuple<int, IEnumerable<string>, string>(
                1, new Collection<string> { "brrr" }, It.IsAny<string>());

            Assert.That(() => this.interpreter.Interpret(content), Throws.TypeOf<InvalidOperationException>());
            
            this.validator.Verify(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
        }

        [Test, RequiresThread]
        public void Interpret_IntergalacticProduct()
        {
            const string product = "pish";
            const string productValue = "V";

            var intergalacticProductsCache = new Dictionary<string, string>();
            var earthyProductsCache = new Dictionary<string, decimal>();

            this.SetInterpreter(intergalacticProductsCache, earthyProductsCache);

            this.validator.Setup(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(true)
                .Verifiable();

            this.productCategorizer.Setup(i => i.GetProductType(It.IsAny<string>()))
                .Returns(ProductType.Intergalactic)
                .Verifiable();

            var content = new Tuple<int, IEnumerable<string>, string>(
                1, new Collection<string> { product }, productValue);

            var result = this.interpreter.Interpret(content);

            Assert.IsTrue(result == 1);
            Assert.IsTrue(intergalacticProductsCache.Keys.Count() == 1);
            Assert.AreEqual(product, intergalacticProductsCache.Keys.Last());
            Assert.AreEqual(productValue, intergalacticProductsCache[product]);

            Assert.IsTrue(earthyProductsCache.Keys.Count() == 0);

            this.validator.Verify(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()), Times.Once);

            this.productCategorizer.Verify(i => i.GetProductType(It.IsAny<string>()), Times.AtLeastOnce);

            this.productHelper.Verify(i => i.GetMultiplierInArabicNumeralForm(It.IsAny<IEnumerable<string>>(), It.IsAny<IDictionary<string, string>>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Interpret_EarthyProduct()
        {
            const string product = "Silver";
            const string productValue = "60";
            const decimal value = 60M; // product value in decimal 

            var intergalacticProductsCache = new Dictionary<string, string>();
            var earthyProductsCache = new Dictionary<string, decimal>();

            this.SetInterpreter(intergalacticProductsCache, earthyProductsCache);

            this.validator.Setup(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(true)
                .Verifiable();

            this.productCategorizer.Setup(i => i.GetProductType(It.IsAny<string>()))
                .Returns(ProductType.Earthy)
                .Verifiable();

            this.productHelper.Setup(i => i.GetEarthyProductValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(value)
                .Verifiable();

            var content = new Tuple<int, IEnumerable<string>, string>(
                1, new Collection<string> { product }, productValue);

            var result = this.interpreter.Interpret(content);

            Assert.IsTrue(result == 1);
            Assert.IsTrue(earthyProductsCache.Keys.Count() == 1);
            Assert.AreEqual(product, earthyProductsCache.Keys.Last());
            Assert.AreEqual(value, earthyProductsCache[product]);

            Assert.IsTrue(intergalacticProductsCache.Keys.Count() == 0);

            this.validator.Verify(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()), Times.Once);

            this.productCategorizer.Verify(i => i.GetProductType(It.IsAny<string>()), Times.AtLeastOnce);

            this.productHelper.Verify(i => i.GetEarthyProductValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
        }

        [Test, RequiresThread]
        public void Interpret_EarthyProductTaggedWithIntergalacticProduct()
        {
            const string product = "Silver";
            const string productValue = "60";
            const decimal value = 60M; // product value in decimal 

            var intergalacticProductsCache = new Dictionary<string, string>() { { "phish", "V" } };
            var earthyProductsCache = new Dictionary<string, decimal>();

            this.SetInterpreter(intergalacticProductsCache, earthyProductsCache);

            this.validator.Setup(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(true)
                .Verifiable();

            this.productCategorizer.Setup(i => i.GetProductType(It.IsAny<string>()))
                .Returns(ProductType.Earthy)
                .Verifiable();

            this.productHelper.Setup(i => i.GetEarthyProductValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(value)
                .Verifiable();

            var content = new Tuple<int, IEnumerable<string>, string>(
                1, new Collection<string> { product }, productValue);

            var result = this.interpreter.Interpret(content);

            Assert.IsTrue(result == 1);
            Assert.IsTrue(earthyProductsCache.Keys.Count() == 1);
            Assert.AreEqual(product, earthyProductsCache.Keys.Last());
            Assert.AreEqual(value, earthyProductsCache[product]);

            // Nothing was added to this cache.
            Assert.IsTrue(intergalacticProductsCache.Keys.Count() == 1);

            this.validator.Verify(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()), Times.Once);

            this.productCategorizer.Verify(i => i.GetProductType(It.IsAny<string>()), Times.AtLeastOnce);

            this.productHelper.Verify(i => i.GetEarthyProductValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
        }

        private void SetInterpreter()
        {
            var intergalacticProductsCache = new Dictionary<string, string>();
            var earthyProductsCache = new Dictionary<string, decimal>();

            this.interpreter = new ProductDataInterpreter(
                                        this.productCategorizer.Object,
                                        this.productHelper.Object,
                                        this.validator.Object,
                                        intergalacticProductsCache,
                                        earthyProductsCache);
        }

        private void SetInterpreter(IDictionary<string, string> intergalacticProductsCache, IDictionary<string, decimal> earthyProductsCache)
        {
            this.interpreter = new ProductDataInterpreter(
                                        this.productCategorizer.Object,
                                        this.productHelper.Object,
                                        this.validator.Object,
                                        intergalacticProductsCache,
                                        earthyProductsCache);
        }

        private static TestCaseData[] GetNullDependencies()
        {
            return new[]
            {
                new TestCaseData(null, null),
                new TestCaseData(null, GetEarthyProductsCache()),
                new TestCaseData(GetIntergalacticProductsCache(), null),
            };
        }

        private static IDictionary<string, string> GetIntergalacticProductsCache()
        {
            return new Dictionary<string, string>()
            {
                { "glob", "I" },
                { "prok", "V" },
                { "pish", "X" },
                { "tegj", "L" },
                { "kryptonite", "MMMCMXCIX" }
            };
        }

        private static IDictionary<string, decimal> GetEarthyProductsCache()
        {
            return new Dictionary<string, decimal>()
            {
                { "Silver", 60M },
                { "Gold", 100M },
                { "Iron", 19.5M },
                { "Bronze", 25M }
            };
        }
    }
}