namespace ValidationLib
{
    public interface IRuleValidationEngine
    {
        string Validate(string romanNumeral);

        string Validate(int arabicNumeral);
    }
}