using System.Collections.Generic;

namespace NumeralCalculator.Worker
{
    public class Processor : IProcessor
    {
        private readonly IPreProcessor preProcessor;
        private readonly IAnalyzer analyzer;
        
        public Processor(
            IPreProcessor preProcessor,
            IAnalyzer analyzer)
        {
            this.preProcessor = preProcessor;
            this.analyzer = analyzer;
        }

        public IEnumerable<string> Process(IEnumerable<string> contents)
        {
            var processedContent = this.preProcessor.Process(contents);

            return this.analyzer.Analyze(processedContent);
        }
    }
}