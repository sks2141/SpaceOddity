using Moq;
using NumeralCalculator.Validator;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using ValidationLib;

namespace NumeralCalculator.UnitTests.Validator
{
    [TestFixture]
    public class CompoundValidatorTest
    {
        private Mock<IProductCategorizer> productCategorizer;
        private Mock<IProductHelper> productHelper;
        private Mock<IRuleValidationEngine> ruleValidator;

        private ICompoundValidator validator;

        [SetUp]
        public void Setup()
        {
            this.productCategorizer = new Mock<IProductCategorizer>(MockBehavior.Strict);
            this.productHelper = new Mock<IProductHelper>(MockBehavior.Strict);
            this.ruleValidator = new Mock<IRuleValidationEngine>(MockBehavior.Strict);

            this.validator = new CompoundValidator(this.productCategorizer.Object, this.productHelper.Object, this.ruleValidator.Object);
        }

        [TestCase(null, ExpectedResult = false)]
        [Test, RequiresThread]
        public bool IsProductRepresentationValid_NullProducts(IEnumerable<string> products)
        {
            return this.validator.IsProductRepresentationValid(products,
                It.IsAny<string>(), It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>());
        }

        [Test, RequiresThread]
        public void IsProductRepresentationValid_EmptyProducts()
        {
            var result = this.validator.IsProductRepresentationValid(new string[] { },
                It.IsAny<string>(), It.IsAny<InputType>(), It.IsAny<int>(), It.IsAny<IDictionary<string, string>>());

            Assert.IsFalse(result);
        }

        [TestCase(InputType.Query, null, ExpectedResult = true)] // Query does not supply the value of the product
        [TestCase(InputType.Query, "123", ExpectedResult = true)] // Negative Scenario
        [TestCase(InputType.ProductData, null, ExpectedResult = true)]
        [TestCase(InputType.ProductData, "", ExpectedResult = true)]
        [Test, RequiresThread]
        public bool IsProductRepresentationValid_Single_NullOrEmptyProductValue(InputType typeOfInput, string productValue)
        {
            return this.validator.IsProductRepresentationValid(new string[] { "glob" },
                productValue, typeOfInput, It.IsAny<int>(), It.IsAny<IDictionary<string, string>>());
        }

        [TestCase(ExpectedResult = true)]
        [Test, RequiresThread]
        public bool IsProductRepresentationValid_SingleProduct_UnknownType()
        {
            this.productCategorizer.Setup(i => i.IsProductIntergalactic(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();

            this.productCategorizer.Setup(i => i.IsProductEarthy(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();

            return this.validator.IsProductRepresentationValid(new string[] { "glob" },
                "123", InputType.ProductData, It.IsAny<int>(), It.IsAny<IDictionary<string, string>>());
        }

        [TestCase(ProductType.Intergalactic, ExpectedResult = false)]
        [TestCase(ProductType.Earthy, ExpectedResult = false)]
        [Test, RequiresThread]
        public bool IsProductRepresentationValid_SingleProduct_InvalidProductValue(ProductType typeOfProduct)
        {
            if (typeOfProduct == ProductType.Intergalactic)
            {
                this.productCategorizer.Setup(i => i.IsProductIntergalactic(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();

                this.ruleValidator.Setup(i => i.Validate(It.IsAny<string>()))
                    .Returns("Error")
                    .Verifiable();
            }
            else if (typeOfProduct == ProductType.Earthy)
            {
                this.productCategorizer.Setup(i => i.IsProductIntergalactic(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();

                this.productCategorizer.Setup(i => i.IsProductEarthy(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();

                this.ruleValidator.Setup(i => i.Validate(It.IsAny<int>()))
                    .Returns("Error")
                    .Verifiable();
            }

            return this.validator.IsProductRepresentationValid(new string[] { "glob" },
                "123", InputType.ProductData, It.IsAny<int>(), It.IsAny<IDictionary<string, string>>());
        }

        [TestCase(ProductType.Intergalactic, ExpectedResult = true)]
        [TestCase(ProductType.Earthy, ExpectedResult = true)]
        [Test, RequiresThread]
        public bool IsProductRepresentationValid_SingleProduct_ValidProductValue(ProductType typeOfProduct)
        {
            if (typeOfProduct == ProductType.Intergalactic)
            {
                this.productCategorizer.Setup(i => i.IsProductIntergalactic(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();

                this.ruleValidator.Setup(i => i.Validate(It.IsAny<string>()))
                    .Returns(string.Empty)
                    .Verifiable();
            }
            else if (typeOfProduct == ProductType.Earthy)
            {
                this.productCategorizer.Setup(i => i.IsProductIntergalactic(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();

                this.productCategorizer.Setup(i => i.IsProductEarthy(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();

                this.ruleValidator.Setup(i => i.Validate(It.IsAny<int>()))
                    .Returns(string.Empty)
                    .Verifiable();
            }

            return this.validator.IsProductRepresentationValid(new string[] { "glob" },
                "123", InputType.ProductData, It.IsAny<int>(), It.IsAny<IDictionary<string, string>>());
        }

        [TestCase(ExpectedResult = false)]
        [Test, RequiresThread]
        public bool IsProductRepresentationValid_MultipleProducts_IsInputValid_OnlyIntergalacticProducts()
        {
            this.productCategorizer.Setup(i => i.IsProductIntergalactic(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();

            this.productCategorizer.Setup(i => i.IsProductEarthy(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();

            return this.validator.IsProductRepresentationValid(new string[] { "pish", "glob" },
                It.IsAny<string>(), InputType.ProductData, It.IsAny<int>(), It.IsAny<IDictionary<string, string>>());
        }

        [TestCase(ExpectedResult = false)]
        [Test, RequiresThread]
        public bool IsProductRepresentationValid_MultipleProducts_IsInputValid_InvalidIntergalacticProductUnit()
        {
            this.productCategorizer.Setup(i => i.IsProductIntergalactic(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();

            this.productCategorizer.Setup(i => i.IsProductEarthy(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();

            this.ruleValidator.Setup(i => i.Validate(It.IsAny<string>()))
                .Returns("Error")
                .Verifiable();

            return this.validator.IsProductRepresentationValid(new string[] { "pish" },
                "123", InputType.ProductData, It.IsAny<int>(), It.IsAny<IDictionary<string, string>>());
        }

        [TestCase(ExpectedResult = true)]
        [Test, RequiresThread]
        public bool IsProductRepresentationValid_MultipleProducts_Query_IsInputValid()
        {
            this.productCategorizer.Setup(i => i.IsProductIntergalactic(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();

            this.productCategorizer.Setup(i => i.IsProductEarthy(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();

            return this.validator.IsProductRepresentationValid(new string[] { "pish" },
                "123", InputType.Query, It.IsAny<int>(), It.IsAny<IDictionary<string, string>>());
        }

        [TestCase(InputType.ProductData, ExpectedResult = false)]
        [TestCase(InputType.Query, ExpectedResult = false)]
        [Test, RequiresThread]
        public bool IsProductRepresentationValid_MultipleProducts_AllEarthy(InputType inputType)
        {
            if (inputType == InputType.ProductData)
            {
                this.productCategorizer.Setup(i => i.IsProductIntergalactic(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();
            }

            this.productCategorizer.Setup(i => i.IsProductEarthy(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();


            return this.validator.IsProductRepresentationValid(new string[] { "Gold", "Silver" },
                "123", inputType, It.IsAny<int>(), It.IsAny<IDictionary<string, string>>());
        }

        [TestCase(InputType.ProductData, ExpectedResult = false)]
        [TestCase(InputType.Query, ExpectedResult = false)]
        [Test, RequiresThread]
        public bool IsProductRepresentationValid_IsEarthyProduct_NotAtEndOfProductList(InputType typeOfInput)
        {
            this.productCategorizer.Setup(i => i.IsProductIntergalactic(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();

            this.productCategorizer.Setup(i => i.IsProductEarthy(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();

            return this.validator.IsProductRepresentationValid(new string[] { "glob", "Silver", "pish" },
                "123", typeOfInput, It.IsAny<int>(), It.IsAny<IDictionary<string, string>>());
        }

        //[TestCaseSource("GetLinesWithInvalidPrefixSet")]
        //[Test, RequiresThread]
        //public void IsProductRepresentationValid_IsEarthyProductWithIntergalacticOnes_InvalidPrefix(
        //    InputType typeOfInput, IDictionary<string, string> cache, IEnumerable<string> content, bool expectedResult)
        //{
        //    this.productCategorizer.Setup(i => i.IsProductIntergalactic(It.IsAny<string>()))
        //        .Returns(false)
        //        .Verifiable();

        //    this.productCategorizer.Setup(i => i.IsProductEarthy(It.IsAny<string>()))
        //        .Returns(true)
        //        .Verifiable();

        //    this.ruleValidator.Setup(i => i.Validate(It.IsAny<int>()))
        //        .Returns("")
        //        .Verifiable();

        //    this.ruleValidator.Setup(i => i.Validate(It.IsAny<string>()))
        //        .Returns("Error")
        //        .Verifiable();

        //    var result = this.validator.IsProductRepresentationValid(content, "123", typeOfInput, It.IsAny<int>(), cache);

        //    Assert.AreEqual(expectedResult, result);
        //}

        //[TestCaseSource("GetLinesWithInvalidPrefixSetAndNoCache")]
        //[Test, RequiresThread]
        //public void IsProductRepresentationValid_IsEarthyProductWithIntergalacticOnes_InvalidPrefix_EmptyIntergalacticCache(
        //    InputType typeOfInput, IEnumerable<string> content, bool expectedResult)
        //{
        //    this.productCategorizer.Setup(i => i.IsProductIntergalactic(It.IsAny<string>()))
        //        .Returns(false)
        //        .Verifiable();

        //    // Sequence is based on number of items in the products list X No. of times Moq is invoked
        //    //this.productCategorizer.SetupSequence(i => i.IsProductEarthy(It.IsAny<string>()))
        //    //    .Returns(false)
        //    //    .Returns(false)
        //    //    .Returns(false)
        //    //    .Returns(true)
        //    //    .Returns(true)
        //    //    .Returns(true);

        //    var firstTime = true;
        //    bool mockReturn = false;
        //    this.productCategorizer.Setup(i => i.IsProductEarthy(It.IsAny<string>()))
        //        .Returns(() =>
        //        {
        //            if (firstTime)
        //            {
        //                for (int i = 0; i < content.Count(); i++)
        //                {
        //                    mockReturn = false;
        //                }

        //                firstTime = false;
        //            }
        //            else
        //            {
        //                mockReturn = true;
        //            }
                    
        //            return mockReturn;
        //        });

        //    this.ruleValidator.Setup(i => i.Validate(It.IsAny<string>()))
        //        .Returns("Error")
        //        .Verifiable();

        //    this.ruleValidator.Setup(i => i.Validate(It.IsAny<int>()))
        //        .Returns("")
        //        .Verifiable();

        //    var result = this.validator.IsProductRepresentationValid(content, "123", typeOfInput, It.IsAny<int>(), It.IsAny<IDictionary<string, string>>());

        //    Assert.AreEqual(expectedResult, result);
        //}

        private static TestCaseData[] GetLinesWithInvalidPrefixSet()
        {
            return new[]
            {
                new TestCaseData(InputType.ProductData, new Dictionary<string, string> {{ "glob", "I"}, { "tegj", "L"}}, new[] { "glob", "tegj", "Silver" }, false),
                new TestCaseData(InputType.ProductData, new Dictionary<string, string> {{ "glob", "I"}, { "tegj", "L"}}, new[] { "tegj", "glob", "Gold"}, false),
                new TestCaseData(InputType.Query, new Dictionary<string, string> {{ "glob", "I"}, { "tegj", "L"}}, new[] { "glob", "tegj", "Silver" }, false),
                new TestCaseData(InputType.Query, new Dictionary<string, string> {{ "glob", "I"}, { "tegj", "L"}}, new[] { "tegj", "glob", "Gold" }, false),
            };
        }

        private static TestCaseData[] GetLinesWithInvalidPrefixSetAndNoCache()
        {
            return new[]
            {
                //new TestCaseData(InputType.ProductData, new[] { "glob", "tegj", "Silver" }, true),
                //new TestCaseData(InputType.ProductData, new[] { "tegj", "glob", "Gold" }, true),
                new TestCaseData(InputType.Query, new[] { "glob", "tegj", "Silver" }, true),
                new TestCaseData(InputType.Query, new[] { "tegj", "glob", "Gold" }, true),
            };
        }
    }
}