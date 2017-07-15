namespace NumeralCalculator.Converter
{
    public interface IConverterStrategy
    {
        int ConvertToArabicNumeral(string romanNumeral);

        string ConvertToRomanNumeral(int arabicNumeral);
    }
}