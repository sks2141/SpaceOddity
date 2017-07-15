using System;
using System.Collections.Generic;

namespace NumeralCalculator.Worker
{
    public interface IPreProcessor
    {
        IEnumerable<Tuple<int, InputType, IEnumerable<string>, string>> Process(IEnumerable<string> content);
    }
}