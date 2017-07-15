using NumeralCalculator.Interpreter;
using System.Collections.Generic;
using Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NumeralCalculator.Worker
{
    /// <summary>
    /// Acts as a factory to fan out the interpretation of input
    /// </summary>
    public class Analyzer : IAnalyzer
    {
        private readonly IInterpreter<int> productDataInterpreter;
        private readonly IInterpreter<string> queryInterpreter;

        public Analyzer(
            IInterpreter<int> productDataInterpreter,
            IInterpreter<string> queryInterpreter)
        {
            this.productDataInterpreter = productDataInterpreter;
            this.queryInterpreter = queryInterpreter;
        }

        public IEnumerable<string> Analyze(IEnumerable<Tuple<int, InputType, IEnumerable<string>, string>> contents)
        {
            ICollection<string> analyzedResponses = new Collection<string>();

            if (contents != null && contents.Any())
            {
                foreach (Tuple<int, InputType, IEnumerable<string>, string> content in contents)
                {
                    int lineNumber = content.Item1;
                    InputType typeOfInput = content.Item2;
                    IEnumerable<string> products = content.Item3;
                    string productValue = content.Item4;

                    SLogger.LogInfoFormat("Analyzing line number {0}.", lineNumber);

                    try
                    {
                        Tuple<int, IEnumerable<string>, string> contentToInterpret =
                            new Tuple<int, IEnumerable<string>, string>(lineNumber, products, productValue);

                        switch (typeOfInput)
                        {
                            case InputType.ProductData:
                            {
                                if (products == null || !products.Any())
                                {
                                    SLogger.LogWarn("No products found to analyze. Skipping product data intrepretation...");
                                    continue;
                                }

                                this.productDataInterpreter.Interpret(contentToInterpret);
                            }
                            break;

                            case InputType.Query:
                            {
                                string queryResponse = this.queryInterpreter.Interpret(contentToInterpret);
                                analyzedResponses.Add(queryResponse);
                            }
                            break;

                            default:
                                throw new InvalidOperationException("Invalid InputType Enum.");
                        }
                    }
                    catch (Exception ex)
                    {
                        SLogger.LogErrorFormat(ex, "Exception found when Analyzing line: {0}.", lineNumber);
                    }
                }
            }

            return analyzedResponses;
        }
    }
}