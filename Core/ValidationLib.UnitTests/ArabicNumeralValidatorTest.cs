using FluentValidation.Results;
using NUnit.Framework;
using System.Linq;

namespace ValidationLib.UnitTests
{
    [TestFixture]
    public class ArabicNumeralValidatorTest
    {
        private IValidator<int> validator;

        [SetUp]
        public void Setup()
        {
            this.validator = new ArabicNumeralValidator();
        }

        [Test, RequiresThread]
        public void Validate_InvalidSet(
            [Values(-1, 0)]
            int number)
        {
            ValidationResult result = this.validator.Validate(number);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count == 1);
            StringAssert.Contains("Arabic numeral should be greater than 0.", result.Errors.First().ErrorMessage);
        }

        [Test, RequiresThread]
        public void Validate_ValidSet(
            [Values(1, 100, 3999)]
            int number)
        {
            ValidationResult result = this.validator.Validate(number);

            Assert.IsTrue(result.IsValid);
            Assert.Zero(result.Errors.Count);
        }
    }
}