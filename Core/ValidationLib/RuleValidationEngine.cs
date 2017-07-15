using FluentValidation.Results;
using Utilities;

namespace ValidationLib
{
    public class RuleValidationEngine : IRuleValidationEngine
    {
        private readonly IValidator<int> arabicNumeralValidator;
        private readonly IValidator<string> romanNumeralValidator;
        private readonly IValidationMessageManager validationMessageManager;

        public RuleValidationEngine(
            IValidator<int> arabicNumeralValidator,
            IValidator<string> romanNumeralValidator, 
            IValidationMessageManager validationMessageManager)
        {
            this.arabicNumeralValidator = arabicNumeralValidator;
            this.romanNumeralValidator = romanNumeralValidator;
            this.validationMessageManager = validationMessageManager;
        }

        public string Validate(string numeral)
        {
            SLogger.LogInfoFormat("Validating {0}", numeral);

            ValidationResult result = this.romanNumeralValidator.Validate(numeral);

            return this.Validate(result);
        }

        public string Validate(int numeral)
        {
            SLogger.LogInfoFormat("Validating {0}", numeral);

            ValidationResult result = this.arabicNumeralValidator.Validate(numeral);

            return this.Validate(result);
        }

        private string Validate(ValidationResult result)
        {
            if (!result.IsValid)
            {
                string validationExceptions = this.validationMessageManager.GetValidationString(result);

                SLogger.LogWarnFormat("Found validationExceptions {0}", validationExceptions);

                return validationExceptions;
            }

            SLogger.LogDebug("Validation succeeded. No issues noticed.");

            return string.Empty;
        }
    }
}