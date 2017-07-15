using FluentValidation.Results;
using Moq;
using NUnit.Framework;

namespace ValidationLib.UnitTests
{
    [TestFixture]
    public class RuleValidationEngineTest
    {
        private Mock<IValidator<int>> arabicNumeralValidator;
        private Mock<IValidator<string>> romanNumeralValidator;
        private Mock<IValidationMessageManager> validationMessageManager;
        private IRuleValidationEngine validator;

        [SetUp]
        public void Setup()
        {
            this.arabicNumeralValidator = new Mock<IValidator<int>>(MockBehavior.Strict);
            this.romanNumeralValidator = new Mock<IValidator<string>>(MockBehavior.Strict);
            this.validationMessageManager = new Mock<IValidationMessageManager>(MockBehavior.Strict);

            this.validator = new RuleValidationEngine(this.arabicNumeralValidator.Object, 
                                this.romanNumeralValidator.Object, this.validationMessageManager.Object);
        }

        [TearDown]
        public void Teardown()
        {
            this.arabicNumeralValidator.Verify();
            this.romanNumeralValidator.Verify();
            this.validationMessageManager.Verify();
        }

        [Test, RequiresThread]
        public void Validate_InvalidArabicNumeral()
        {
            string errorMessage = "fakeError";
            ValidationResult errorResult = new ValidationResult();
            errorResult.Errors.Add(new ValidationFailure("fakePropertyName", errorMessage));

            this.arabicNumeralValidator.Setup(i => i.Validate(It.IsAny<int>()))
                .Returns(errorResult)
                .Verifiable();

            this.validationMessageManager.Setup(i => i.GetValidationString(It.IsAny<ValidationResult>()))
                .Returns(errorMessage)
                .Verifiable();

            string result = this.validator.Validate(0);

            Assert.IsNotEmpty(result);
            this.arabicNumeralValidator.Verify(i => i.Validate(It.IsAny<int>()), Times.Once);
            this.validationMessageManager.Verify(i => i.GetValidationString(It.IsAny<ValidationResult>()), Times.Once);
            this.romanNumeralValidator.Verify(i => i.Validate(It.IsAny<string>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Validate_InvalidRomanNumeral()
        {
            string errorMessage = "fakeError";
            ValidationResult errorResult = new ValidationResult();
            errorResult.Errors.Add(new ValidationFailure("fakePropertyName", errorMessage));

            this.romanNumeralValidator.Setup(i => i.Validate(It.IsAny<string>()))
                .Returns(errorResult)
                .Verifiable();

            this.validationMessageManager.Setup(i => i.GetValidationString(It.IsAny<ValidationResult>()))
                .Returns(errorMessage)
                .Verifiable();

            string result = this.validator.Validate("ABC");

            Assert.IsNotEmpty(result);
            this.romanNumeralValidator.Verify(i => i.Validate(It.IsAny<string>()), Times.Once);
            this.validationMessageManager.Verify(i => i.GetValidationString(It.IsAny<ValidationResult>()), Times.Once);
            this.arabicNumeralValidator.Verify(i => i.Validate(It.IsAny<int>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Validate_ValidArabicNumeral()
        {
            this.arabicNumeralValidator.Setup(i => i.Validate(It.IsAny<int>()))
                .Returns(new ValidationResult())
                .Verifiable();
            
            string result = this.validator.Validate(10);

            Assert.IsEmpty(result);
            this.arabicNumeralValidator.Verify(i => i.Validate(It.IsAny<int>()), Times.Once);
            this.validationMessageManager.Verify(i => i.GetValidationString(It.IsAny<ValidationResult>()), Times.Never);
            this.romanNumeralValidator.Verify(i => i.Validate(It.IsAny<string>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Validate_ValidRomanNumeral()
        {
            this.romanNumeralValidator.Setup(i => i.Validate(It.IsAny<string>()))
                .Returns(new ValidationResult())
                .Verifiable();
            
            string result = this.validator.Validate("X");

            Assert.IsEmpty(result);
            this.romanNumeralValidator.Verify(i => i.Validate(It.IsAny<string>()), Times.Once);
            this.validationMessageManager.Verify(i => i.GetValidationString(It.IsAny<ValidationResult>()), Times.Never);
            this.arabicNumeralValidator.Verify(i => i.Validate(It.IsAny<int>()), Times.Never);
        }
    }
}