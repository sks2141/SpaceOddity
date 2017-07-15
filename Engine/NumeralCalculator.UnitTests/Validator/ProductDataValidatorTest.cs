using Moq;
using NumeralCalculator.Validator;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ValidationLib;

namespace NumeralCalculator.UnitTests.Validator
{
    [TestFixture]
    public class ProductDataValidatorTest
    {
        private const string CREDITS_TEXT = "Credits";

        private Mock<IRuleValidationEngine> ruleValidator;
        private Mock<IProductCategorizer> productCategorizer;

        private IProductDataValidator validator;

        [SetUp]
        public void Setup()
        {
            this.ruleValidator = new Mock<IRuleValidationEngine>(MockBehavior.Strict);
            this.productCategorizer = new Mock<IProductCategorizer>(MockBehavior.Strict);
        }

        [Test, RequiresThread]
        public void Validate_NullProductsList()
        {
            Assert.That(() => new ProductDataValidator(
                this.ruleValidator.Object,
                this.productCategorizer.Object,
                null), Throws.TypeOf<ArgumentNullException>());
        }
        
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        [Test, RequiresThread]
        public void GetProducts_NullOrEmptyOrWhitespaceContent(string content)
        {
            this.validator = new ProductDataValidator(this.ruleValidator.Object,
                                                      this.productCategorizer.Object, 
                                                      new string[] { });

            var results = validator.GetProducts(content, 1);

            Assert.IsNull(results);
        }

        [Test, RequiresThread]
        public void GetProducts_InvalidProductType()
        {
            this.productCategorizer.Setup(i => i.IsProductValid(It.IsAny<string>()))
                .Returns(false)
                .Verifiable();
            
            this.validator = new ProductDataValidator(this.ruleValidator.Object,
                                                      this.productCategorizer.Object,
                                                      new string[] { });

            string content = "kryptonite gummyl Diamond";

            var results = validator.GetProducts(content, 1);

            Assert.IsNull(results);
            this.productCategorizer.Verify(i => i.IsProductValid(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test, RequiresThread]
        public void GetProducts_ValidProductAlreadyInList()
        {
            ICollection<string> productsList = new Collection<string> { "glob" , "drd" };
            int cacheCount = productsList.Count();

            this.productCategorizer.Setup(i => i.IsProductValid(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();

            this.validator = new ProductDataValidator(this.ruleValidator.Object,
                                                      this.productCategorizer.Object,
                                                      productsList);

            string content = "glob pish prok";

            var results = validator.GetProducts(content, 1);

            Assert.IsNotEmpty(results);
            Assert.Greater(productsList.Count, cacheCount);
            Assert.Less(results.Count(), productsList.Count);

            this.productCategorizer.Verify(i => i.IsProductValid(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test, RequiresThread]
        public void GetProducts_ValidProductNotInCache()
        {
            ICollection<string> productsList = new Collection<string> { "glob", "drd" };
            int cacheCount = productsList.Count();

            this.productCategorizer.Setup(i => i.IsProductValid(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();

            this.validator = new ProductDataValidator(this.ruleValidator.Object,
                                                      this.productCategorizer.Object,
                                                      productsList);

            string content = "tegj pish prok";

            var results = validator.GetProducts(content, 1);

            Assert.IsNotEmpty(results);
            Assert.Greater(productsList.Count, cacheCount);
            Assert.AreEqual(cacheCount + results.Count(), productsList.Count);

            this.productCategorizer.Verify(i => i.IsProductValid(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        [Test, RequiresThread]
        public void GetProductValue_NullOrEmptyOrWhitespaceValue(string content)
        {
            this.validator = new ProductDataValidator(this.ruleValidator.Object,
                                                      this.productCategorizer.Object,
                                                      new string[] { });

            var results = validator.GetProductValue(content, 1);

            Assert.IsEmpty(results);
        }

        [TestCase("100")]
        [TestCase("Credits 100")]
        [TestCase("100 Credits Ahoy")]
        [Test, RequiresThread]
        public void GetProductValue_InvalidArabicNumeralContent(string content)
        {
            this.ruleValidator.Setup(i => i.Validate(It.IsAny<int>()))
                .Returns("Error")
                .Verifiable();

            this.ruleValidator.Setup(i => i.Validate(It.IsAny<string>()))
                .Returns("Error")
                .Verifiable();

            this.validator = new ProductDataValidator(this.ruleValidator.Object,
                                                      this.productCategorizer.Object,
                                                      new string[] { });

            var result = this.validator.GetProductValue(content, 1);

            Assert.IsEmpty(result);

            int index = content.IndexOf(CREDITS_TEXT, StringComparison.OrdinalIgnoreCase);
            if (index < 1) // -1 for string, 0 if Credits is the first word in the split.
            {
                this.ruleValidator.Verify(i => i.Validate(It.IsAny<string>()), Times.AtLeastOnce);
            }
            else
            {
                this.ruleValidator.Verify(i => i.Validate(It.IsAny<int>()), Times.AtLeastOnce);
            }
        }

        [TestCase("100 Credits")]
        [Test, RequiresThread]
        public void GetProductValue_ValidArabicNumeralContent(string content)
        {
            this.ruleValidator.Setup(i => i.Validate(It.IsAny<int>()))
                .Returns(string.Empty)
                .Verifiable();

            this.validator = new ProductDataValidator(this.ruleValidator.Object,
                                                      this.productCategorizer.Object,
                                                      new string[] { });

            var result = this.validator.GetProductValue(content, 1);

            Assert.IsNotEmpty(result);

            int number = 0;
            string numberString = content.Substring(0, content.IndexOf(CREDITS_TEXT, StringComparison.OrdinalIgnoreCase) - 1).Trim();
            int.TryParse(numberString, out number);

            Assert.AreEqual(numberString.ToString(), result);
        }

        [TestCase("IXABC")]
        [TestCase("IX Credits")]
        [Test, RequiresThread]
        public void GetProductValue_InvalidRomanNumeralContent(string content)
        {
            this.ruleValidator.Setup(i => i.Validate(It.IsAny<int>()))
                .Returns("Error")
                .Verifiable();

            this.ruleValidator.Setup(i => i.Validate(It.IsAny<string>()))
                .Returns("Error")
                .Verifiable();

            this.validator = new ProductDataValidator(this.ruleValidator.Object,
                                                      this.productCategorizer.Object,
                                                      new string[] { });
            
            var result = this.validator.GetProductValue(content, 1);

            Assert.IsEmpty(result);


            if (content.IndexOf(CREDITS_TEXT) == -1)
            {
                this.ruleValidator.Verify(i => i.Validate(It.IsAny<string>()), Times.AtLeastOnce);
            }
            else
            {
                this.ruleValidator.Verify(i => i.Validate(It.IsAny<int>()), Times.AtLeastOnce);
            }
        }

        [TestCase("IX")]
        [Test, RequiresThread]
        public void GetProductValue_RomanNumeralContentWithoutCredits(string content)
        {
            this.ruleValidator.Setup(i => i.Validate(It.IsAny<string>()))
                .Returns(string.Empty)
                .Verifiable();

            this.validator = new ProductDataValidator(this.ruleValidator.Object,
                                                      this.productCategorizer.Object,
                                                      new string[] { });
            
            var result = this.validator.GetProductValue(content, 1);

            Assert.IsNotEmpty(result);
            Assert.AreEqual(content, result);
        }
    }
}