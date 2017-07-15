using System.Collections.Generic;

namespace NumeralCalculator.Worker
{
    public interface IProcessor
    {
        IEnumerable<string> Process(IEnumerable<string> contents);
    }
}