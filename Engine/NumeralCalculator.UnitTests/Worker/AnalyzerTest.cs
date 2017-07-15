using Moq;
using NumeralCalculator.Interpreter;
using NumeralCalculator.Worker;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NumeralCalculator.UnitTests.Worker
{
    [TestFixture]
    public class AnalyzerTest
    {
        private Mock<IInterpreter<int>> productDataInterpreter;
        private Mock<IInterpreter<string>> queryInterpreter;

        private IAnalyzer analyzer;

        [SetUp]
        public void Setup()
        {
            this.productDataInterpreter = new Mock<IInterpreter<int>>(MockBehavior.Strict);
            this.queryInterpreter = new Mock<IInterpreter<string>>(MockBehavior.Strict);

            this.analyzer = new Analyzer(this.productDataInterpreter.Object, this.queryInterpreter.Object);
        }

        [Test, RequiresThread]
        public void Analyze_NullContents()
        {
            var results = this.analyzer.Analyze(null);

            Assert.IsEmpty(results);
            this.productDataInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
            this.queryInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Analyze_EmptyContents()
        {
            var results = this.analyzer.Analyze(new Collection<Tuple<int, InputType, IEnumerable<string>, string>>());

            Assert.IsEmpty(results);
            this.productDataInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
            this.queryInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Analyze_ProductData_NullProductContents()
        {
            var results = this.analyzer.Analyze(new Collection<Tuple<int, InputType, IEnumerable<string>, string>>()
            {
                new Tuple<int, InputType, IEnumerable<string>, string>(1, InputType.ProductData, null, It.IsAny<string>())
            });

            Assert.IsEmpty(results);
            this.productDataInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
            this.queryInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Analyze_Query_NullProductContents()
        {
            this.queryInterpreter.Setup(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()))
                .Returns("response")
                .Verifiable();

            var results = this.analyzer.Analyze(new Collection<Tuple<int, InputType, IEnumerable<string>, string>>()
            {
                new Tuple<int, InputType, IEnumerable<string>, string>(1, InputType.Query, null, It.IsAny<string>())
            });

            Assert.IsNotEmpty(results);
            this.productDataInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
            this.queryInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Once);
        }

        [Test, RequiresThread]
        public void Analyze_ProductData_EmptyProductContents()
        {
            var results = this.analyzer.Analyze(new Collection<Tuple<int, InputType, IEnumerable<string>, string>>()
            {
                new Tuple<int, InputType, IEnumerable<string>, string>(1, InputType.ProductData, new string[]{ }, It.IsAny<string>())
            });

            Assert.IsEmpty(results);
            this.productDataInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
            this.queryInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Analyze_Query_EmptyProductContents()
        {
            this.queryInterpreter.Setup(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()))
                .Returns("response")
                .Verifiable();

            var results = this.analyzer.Analyze(new Collection<Tuple<int, InputType, IEnumerable<string>, string>>()
            {
                new Tuple<int, InputType, IEnumerable<string>, string>(1, InputType.Query, new string[]{ }, It.IsAny<string>())
            });

            Assert.IsNotEmpty(results);
            this.productDataInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
            this.queryInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Once);
        }

        [Test, RequiresThread]
        public void Analyze_ProductData()
        {
            this.productDataInterpreter.Setup(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()))
                .Returns(It.IsAny<int>())
                .Verifiable();

            var results = this.analyzer.Analyze(GetData(InputType.ProductData));

            Assert.IsEmpty(results);
            this.productDataInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.AtLeastOnce);
            this.queryInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Analyze_Query()
        {
            this.queryInterpreter.Setup(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()))
                .Returns(It.IsAny<string>())
                .Verifiable();

            var results = this.analyzer.Analyze(GetData(InputType.Query));

            Assert.IsNotEmpty(results);

            this.productDataInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
            this.queryInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.AtLeastOnce);
        }

        [Test, RequiresThread]
        public void Analyze_HandleException_WithMultipleContents()
        {
            this.productDataInterpreter.Setup(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()))
                .Throws<Exception>()
                .Verifiable();

            this.queryInterpreter.Setup(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()))
                .Returns(It.IsAny<string>())
                .Verifiable();

            var contents = GetData(InputType.ProductData).Union(GetData(InputType.Query));

            var results = this.analyzer.Analyze(contents);
            
            Assert.Greater(results.Count(), 0);
            this.productDataInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.AtLeastOnce);
            this.queryInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.AtLeastOnce);
        }

        [Test, RequiresThread]
        public void Analyze_HandleException_WithOneContent()
        {
            this.productDataInterpreter.Setup(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()))
                .Throws<Exception>()
                .Verifiable();

            var results = this.analyzer.Analyze(GetData(InputType.ProductData, 1));

            Assert.Zero(results.Count());
            this.productDataInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.AtLeastOnce);
            this.queryInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Analyze_HandleException_InvalidInputType()
        {
            this.productDataInterpreter.Setup(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()))
                .Throws<Exception>()
                .Verifiable();

            var results = this.analyzer.Analyze(GetData(InputType.Unassigned, 1));

            Assert.Zero(results.Count());
            this.productDataInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
            this.queryInterpreter.Verify(i => i.Interpret(It.IsAny<Tuple<int, IEnumerable<string>, string>>()), Times.Never);
        }

        private static IEnumerable<Tuple<int, InputType, IEnumerable<string>, string>> GetData(InputType typeOfInput, int count = 2)
        {
            var contents = new Collection<Tuple<int, InputType, IEnumerable<string>, string>>();

            for (int i = 1; i <= count; i++)
            {
                contents.Add(new Tuple<int, InputType, IEnumerable<string>, string>
                    (1, typeOfInput, new[] { "glob Silver"}, It.IsAny<string>()));
            }

            return contents;
        }
    }
}