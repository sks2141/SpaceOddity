using System.Collections.Generic;

namespace NumeralCalculator.Validator
{
    public interface IProductDataValidator
    {
        IEnumerable<string> GetProducts(string content, int lineNumber);

        string GetProductValue(string content, int lineNumber);
    }
}