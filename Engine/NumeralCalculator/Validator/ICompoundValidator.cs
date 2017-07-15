using System.Collections.Generic;

namespace NumeralCalculator.Validator
{
    public interface ICompoundValidator
    {
        bool IsProductRepresentationValid(IEnumerable<string> products, string productValue, InputType typeOfInput, int lineNumber,
            IDictionary<string, string> intergalacticProductsCache = null);
    }
}