using System;
using System.Collections.Generic;

namespace NumeralCalculator.Worker
{
    public interface IAnalyzer
    {
        IEnumerable<string> Analyze(IEnumerable<Tuple<int, InputType, IEnumerable<string>, string>> contents);
    }
}