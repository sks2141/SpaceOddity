using Autofac;
using DataAccessLib;
using NUnit.Framework;
using NumeralCalculator.Worker;
using System.Linq;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using Utilities;
using ValidationLib;

namespace NumeralCalculator.Specs
{
    [Binding]
    public class NumeralCalculatorSteps
    {
        private readonly ContainerBuilder builder;
        private IProcessor processor;
        private IFileReader reader;

        private IEnumerable<string> inputContents;
        private IList<string> expectedOutput;

        private IList<string> processedOutput;

        public NumeralCalculatorSteps()
        {
            builder = new ContainerBuilder();
            this.LoadDependencyGraph();
        }

        [Given]
        public void GivenBootstrapDependencies()
        {
            var container = this.builder.Build();
            this.processor = container.Resolve<IProcessor>();
            this.reader = container.Resolve<IFileReader>();
        }

        [Given(@"Input From ""(.*)""")]
        public void GivenInputFrom(string filePath)
        {
            this.inputContents = this.reader.ReadLinesFromInputFile(filePath);
        }

        [Given(@"Expected Output From ""(.*)""")]
        public void GivenExpectedOutputFrom(string filePath)
        {
            this.expectedOutput = this.reader.ReadLinesFromInputFile(filePath).ToList();
        }
        
        [When]
        public void WhenProcessedByCalculator()
        {
            this.processedOutput = this.processor.Process(this.inputContents).ToList();
        }
        
        [Then]
        public void ThenCompareOutputToExpectedOutput()
        {
            Assert.AreEqual(this.expectedOutput.Count(), this.processedOutput.Count());

            for (int i = 0; i < this.expectedOutput.Count(); i++)
            {
                //StringAssert.AreEqualIgnoringCase(this.expectedOutput[i], this.processedOutput[i]);
                Assert.AreEqual(this.expectedOutput[i], this.processedOutput[i]);
            }
        }

        public void LoadDependencyGraph()
        {
            builder.RegisterModule<DataAccessModule>();
            builder.RegisterModule<UtilitiesModule>();
            builder.RegisterModule<ValidationModule>();
            builder.RegisterModule<NumeralCalculatorModule>();
        }
    }
}
