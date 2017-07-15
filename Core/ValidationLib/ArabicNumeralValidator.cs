using FluentValidation;

namespace ValidationLib
{
    public class ArabicNumeralValidator : AbstractValidator<int>, IValidator<int>
    {
        public ArabicNumeralValidator()
        {
            this.RuleFor(numeral => numeral)
                .GreaterThan(0)
                .WithMessage("Arabic numeral should be greater than 0.");
            
            // Commenting out: Fails for glob prok Gold is 57800 Credits
            //.InclusiveBetween(1, 3999)
            //.WithMessage("Arabic numeral should be in the range 1 to 3999 only.");
        }
    }
}