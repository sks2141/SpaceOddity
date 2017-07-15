using Moq;
using NumeralCalculator.Worker;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NumeralCalculator.UnitTests.Worker
{
    [TestFixture]
    public class ProcessorTest
    {
        private Mock<IPreProcessor> preprocessor;
        private Mock<IAnalyzer> analyzer;
        
        [SetUp]
        public void Setup()
        {
            this.preprocessor = new Mock<IPreProcessor>(MockBehavior.Strict);
            this.analyzer = new Mock<IAnalyzer>(MockBehavior.Strict);
        }

        [Test, RequiresThread]
        public void Process_Test()
        {
            this.preprocessor.Setup(i => i.Process(It.IsAny<IEnumerable<string>>()))
                .Returns(It.IsAny<IEnumerable<Tuple<int, InputType, IEnumerable<string>, string>>>())
                .Verifiable();

            this.analyzer.Setup(i => i.Analyze(It.IsAny<IEnumerable<Tuple<int, InputType, IEnumerable<string>, string>>>()))
                .Returns(It.IsAny<IEnumerable<string>>())
                .Verifiable();

            IProcessor processor = new Processor(this.preprocessor.Object, this.analyzer.Object);

            var results = processor.Process(It.IsAny<IEnumerable<string>>());

            Assert.AreEqual(It.IsAny<IEnumerable<string>>(), results);

            this.preprocessor.Verify(i => i.Process(It.IsAny<IEnumerable<string>>()), Times.Once);
            this.analyzer.Verify(i => i.Analyze(It.IsAny<IEnumerable<Tuple<int, InputType, IEnumerable<string>, string>>>()), Times.Once);
        }
    }
}