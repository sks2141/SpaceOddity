using System.Collections.Generic;

namespace NumeralCalculator.Validator
{
    public interface IQueryValidator
    {
        IEnumerable<string> GetProducts(string content, int lineNumber);
    }
}