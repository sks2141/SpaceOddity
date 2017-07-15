using System;
using System.Collections.Generic;

namespace NumeralCalculator.Interpreter
{
    public interface IInterpreter<out T>
    {
        T Interpret(Tuple<int, IEnumerable<string>, string> content);
    }
}