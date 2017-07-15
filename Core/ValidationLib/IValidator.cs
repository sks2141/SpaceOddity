using FluentValidation.Results;

namespace ValidationLib
{
    public interface IValidator<in T>
    {
        ValidationResult Validate(T input);
    }
}