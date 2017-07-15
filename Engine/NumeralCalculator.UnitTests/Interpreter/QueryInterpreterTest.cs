using Moq;
using NumeralCalculator.Interpreter;
using NumeralCalculator.Validator;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NumeralCalculator.UnitTests.Interpreter
{
    [TestFixture]
    public class QueryInterpreterTest
    {
        private const string INVALID_RESPONSE = "I have no idea what you are talking about";
        private const string CREDITS_TEXT = "Credits";

        private Mock<IProductCategorizer> productCategorizer;
        private Mock<IProductHelper> productHelper;
        private Mock<ICompoundValidator> validator;
        
        private IInterpreter<string> interpreter;

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
                new QueryInterpreter(this.productCategorizer.Object,
                                     this.productHelper.Object,
                                     this.validator.Object,
                                     intergalacticProductsCache,
                                     earthyProductsCache), 
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test, RequiresThread]
        public void Interpret_NullProduct()
        {
            this.SetInterpreter(GetIntergalacticProductsCache(), GetEarthyProductsCache());

            var content = new Tuple<int, IEnumerable<string>, string>(1, null, It.IsAny<string>());
            var result = this.interpreter.Interpret(content);

            StringAssert.AreEqualIgnoringCase(INVALID_RESPONSE, result);
        }

        [Test, RequiresThread]
        public void Interpret_EmptyProducts()
        {
            this.SetInterpreter(GetIntergalacticProductsCache(), GetEarthyProductsCache());

            var content = new Tuple<int, IEnumerable<string>, string>(1, new Collection<string>{ }, It.IsAny<string>());
            var result = this.interpreter.Interpret(content);

            StringAssert.AreEqualIgnoringCase(INVALID_RESPONSE, result);
        }

        [Test, RequiresThread]
        public void Interpret_InvalidProducts()
        {
            this.SetInterpreter(GetIntergalacticProductsCache(), GetEarthyProductsCache());
            this.validator.Setup(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(false)
                .Verifiable();

            var content = new Tuple<int, IEnumerable<string>, string>(
                1, new Collection<string> { "yaba", "daba", "doo" }, It.IsAny<string>());

            var result = this.interpreter.Interpret(content);

            StringAssert.AreEqualIgnoringCase(INVALID_RESPONSE, result);

            this.validator.Verify(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
        }

        [Test, RequiresThread]
        public void Interpret_InvalidResponse()
        {
            this.SetInterpreter(GetIntergalacticProductsCache(), GetEarthyProductsCache());
            this.validator.Setup(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(true)
                .Verifiable();

            this.productCategorizer.Setup(i => i.GetProductType(It.IsAny<string>()))
                .Returns(ProductType.Unassigned)
                .Verifiable();

            var content = new Tuple<int, IEnumerable<string>, string>(
                1, new Collection<string> { "alpha", "beta", "tango" }, It.IsAny<string>());

            var result = this.interpreter.Interpret(content);

            StringAssert.AreEqualIgnoringCase(INVALID_RESPONSE, result);

            this.validator.Verify(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()), Times.Once);

            this.productCategorizer.Verify(i => i.GetProductType(It.IsAny<string>()), Times.AtLeastOnce);

        }

        [Test, RequiresThread]
        public void Interpret_AllIntergalacticProducts_ValidResponseWithoutCredit()
        {
            this.SetInterpreter(GetIntergalacticProductsCache(), GetEarthyProductsCache());
            this.validator.Setup(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(true)
                .Verifiable();

            this.productCategorizer.Setup(i => i.GetProductType(It.IsAny<string>()))
                .Returns(ProductType.Intergalactic)
                .Verifiable();

            this.productHelper.Setup(i => i.GetMultiplierInArabicNumeralForm(It.IsAny<IEnumerable<string>>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(2)
                .Verifiable();

            var content = new Tuple<int, IEnumerable<string>, string>(
                1, new Collection<string> { "prok", "phish" }, It.IsAny<string>());

            var result = this.interpreter.Interpret(content);

            Assert.IsNotEmpty(result);
            StringAssert.AreNotEqualIgnoringCase(INVALID_RESPONSE, result);
            StringAssert.DoesNotContain(CREDITS_TEXT, result);

            this.validator.Verify(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()), Times.Once);

            this.productCategorizer.Verify(i => i.GetProductType(It.IsAny<string>()), Times.AtLeastOnce);

            this.productHelper.Verify(i => i.GetMultiplierInArabicNumeralForm(It.IsAny<IEnumerable<string>>(), It.IsAny<IDictionary<string, string>>()), Times.Once);
        }

        [Test, RequiresThread]
        public void Interpret_OnlyEarthyProduct_DecimalRepresentation_ValidResponseWithCredit()
        {
            this.SetInterpreter(GetIntergalacticProductsCache(), GetEarthyProductsCache());
            this.validator.Setup(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(true)
                .Verifiable();

            this.productCategorizer.Setup(i => i.GetProductType(It.IsAny<string>()))
                .Returns(ProductType.Earthy)
                .Verifiable();

            var content = new Tuple<int, IEnumerable<string>, string>(
                1, new Collection<string> { "Iron" }, CREDITS_TEXT); // Iron has product value: 19.5M 

            var result = this.interpreter.Interpret(content);

            Assert.IsNotEmpty(result);
            StringAssert.AreNotEqualIgnoringCase(INVALID_RESPONSE, result);
            StringAssert.Contains(CREDITS_TEXT, result);
            StringAssert.Contains(".", result);

            this.validator.Verify(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()), Times.Once);

            this.productCategorizer.Verify(i => i.GetProductType(It.IsAny<string>()), Times.AtLeastOnce);

            this.productHelper.Verify(i => i.GetMultiplierInArabicNumeralForm(It.IsAny<IEnumerable<string>>(), It.IsAny<IDictionary<string, string>>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Interpret_OnlyEarthyProduct_DecimalWithZeroFraction_ValidResponseWithCredit()
        {
            this.SetInterpreter(GetIntergalacticProductsCache(), GetEarthyProductsCache());
            this.validator.Setup(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(true)
                .Verifiable();

            this.productCategorizer.Setup(i => i.GetProductType(It.IsAny<string>()))
                .Returns(ProductType.Earthy)
                .Verifiable();

            var content = new Tuple<int, IEnumerable<string>, string>(
                1, new Collection<string> { "Gold" }, CREDITS_TEXT); // Gold has product value: 100M 

            var result = this.interpreter.Interpret(content);

            Assert.IsNotEmpty(result);
            StringAssert.AreNotEqualIgnoringCase(INVALID_RESPONSE, result);
            StringAssert.Contains(CREDITS_TEXT, result);
            StringAssert.DoesNotContain(".", result);

            this.validator.Verify(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()), Times.Once);

            this.productCategorizer.Verify(i => i.GetProductType(It.IsAny<string>()), Times.AtLeastOnce);

            this.productHelper.Verify(i => i.GetMultiplierInArabicNumeralForm(It.IsAny<IEnumerable<string>>(), It.IsAny<IDictionary<string, string>>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Interpret_EarthyProductTaggedWithIntergalactic_ValidResponseWithCredit()
        {
            this.SetInterpreter(GetIntergalacticProductsCache(), GetEarthyProductsCache());
            this.validator.Setup(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(true)
                .Verifiable();

            this.productCategorizer.SetupSequence(i => i.GetProductType(It.IsAny<string>()))
                .Returns(ProductType.Earthy)
                .Returns(ProductType.Earthy)
                .Returns(ProductType.Earthy)
                .Returns(ProductType.Intergalactic)
                .Returns(ProductType.Intergalactic);

            this.productHelper.Setup(i => i.GetMultiplierInArabicNumeralForm(It.IsAny<IEnumerable<string>>(), It.IsAny<IDictionary<string, string>>()))
                .Returns(2)
                .Verifiable();

            var content = new Tuple<int, IEnumerable<string>, string>(
                1, new Collection<string> { "prok", "Gold" }, CREDITS_TEXT); // Gold has product value: 100M 

            var result = this.interpreter.Interpret(content);

            Assert.IsNotEmpty(result);
            StringAssert.AreNotEqualIgnoringCase(INVALID_RESPONSE, result);
            StringAssert.Contains(CREDITS_TEXT, result);
            StringAssert.DoesNotContain(".", result);

            this.validator.Verify(i => i.IsProductRepresentationValid(It.IsAny<IEnumerable<string>>(), It.IsAny<string>(),
                        It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>()), Times.Once);

            this.productCategorizer.Verify(i => i.GetProductType(It.IsAny<string>()), Times.AtLeastOnce);

            this.productHelper.Verify(i => i.GetMultiplierInArabicNumeralForm(It.IsAny<IEnumerable<string>>(), It.IsAny<IDictionary<string, string>>()), Times.AtLeastOnce);
        }

        private void SetInterpreter(IDictionary<string, string> intergalacticProductsCache, IDictionary<string, decimal> earthyProductsCache)
        {
            this.interpreter = new QueryInterpreter(
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