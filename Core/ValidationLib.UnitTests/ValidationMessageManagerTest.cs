using FluentValidation.Results;
using NUnit.Framework;

namespace ValidationLib.UnitTests
{
    [TestFixture]
    public class ValidationMessageManagerTest
    {
        private IValidationMessageManager messageManager;

        [SetUp]
        public void Setup()
        {
            this.messageManager = new ValidationMessageManager();
        }

        [Test, RequiresThread]
        public void GetValidationString_NullValidationResultInput()
        {
            string output = this.messageManager.GetValidationString(null);

            Assert.IsEmpty(output);
        }

        [Test, RequiresThread]
        public void GetValidationString_ValidInput()
        {
            string errorMessage = "fakeError";
            ValidationResult result = new ValidationResult();
            result.Errors.Add(new ValidationFailure("fakeProperty", errorMessage));

            string output = this.messageManager.GetValidationString(result);

            Assert.IsNotNull(output);
            Assert.IsNotEmpty(output);

            StringAssert.Contains(errorMessage, output);
        }
    }
}