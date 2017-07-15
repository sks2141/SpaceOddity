using Moq;
using NumeralCalculator.Validator;
using NumeralCalculator.Worker;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NumeralCalculator.UnitTests.Worker
{
    [TestFixture]
    public class PreProcessorTest
    {
        private Mock<IProductDataValidator> productDataValidator;
        private Mock<IQueryValidator> queryValidator;

        private IPreProcessor preprocessor;

        [SetUp]
        public void Setup()
        {
            this.productDataValidator = new Mock<IProductDataValidator>(MockBehavior.Strict);
            this.queryValidator = new Mock<IQueryValidator>(MockBehavior.Strict);

            this.preprocessor = new PreProcessor(this.productDataValidator.Object, this.queryValidator.Object);
        }

        [Test, RequiresThread]
        public void Process_InvalidSet()
        {
            var result = this.preprocessor.Process(GetInvalidSet());

            result.Select(r => r.Item3).ToList().ForEach(i => Assert.IsEmpty(i));
            result.Select(r => r.Item4).ToList().ForEach(i => Assert.IsEmpty(i));

            this.productDataValidator.Verify(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            this.productDataValidator.Verify(i => i.GetProductValue(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            this.queryValidator.Verify(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Process_ValidProductSet()
        {
            this.productDataValidator.Setup(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new Collection<string> { "glob", "Silver" })
                .Verifiable();

            this.productDataValidator.Setup(i => i.GetProductValue(It.IsAny<string>(), It.IsAny<int>()))
                .Returns("34")
                .Verifiable();

            var result = this.preprocessor.Process(GetValidProductSet());

            result.Select(r => r.Item3).ToList().ForEach(i => Assert.IsNotEmpty(i));
            result.Select(r => r.Item4).ToList().ForEach(i => Assert.IsNotEmpty(i));

            this.productDataValidator.Verify(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce);
            this.productDataValidator.Verify(i => i.GetProductValue(It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce);
            this.queryValidator.Verify(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Process_ValidQuerySet()
        {
            this.queryValidator.Setup(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new Collection<string> { "glob", "Silver" })
                .Verifiable();

            var result = this.preprocessor.Process(GetValidQuerySet());

            result.Select(r => r.Item3).ToList().ForEach(i => Assert.IsNotEmpty(i));
            result.Select(r => r.Item4).ToList().ForEach(i => 
            {
                if (i == "Credits")
                {
                    Assert.IsNotEmpty(i);
                }
                else
                {
                    Assert.IsEmpty(i);
                }
            });

            this.productDataValidator.Verify(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            this.productDataValidator.Verify(i => i.GetProductValue(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            this.queryValidator.Verify(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce);
        }

        [Test, RequiresThread]
        public void Process_HandleException_WithOneContent()
        {
            this.productDataValidator.Setup(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()))
                .Throws<Exception>()
                .Verifiable();

            var result = this.preprocessor.Process(GetValidProductSet());

            result.Select(r => r.Item3).ToList().ForEach(i => Assert.IsEmpty(i));
            result.Select(r => r.Item4).ToList().ForEach(i => Assert.IsEmpty(i));

            this.productDataValidator.Verify(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce);
            this.productDataValidator.Verify(i => i.GetProductValue(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            this.queryValidator.Verify(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Test, RequiresThread]
        public void Process_HandleException_WithMultipleContents()
        {
            this.productDataValidator.Setup(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()))
                .Throws<Exception>()
                .Verifiable();

            this.queryValidator.Setup(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new Collection<string> { "glob", "Silver" })
                .Verifiable();


            var result = this.preprocessor.Process(GetValidProductSet().Union(GetValidQuerySet()));

            result.ToList().ForEach(i => 
            {
                if (i.Item2 == InputType.ProductData)
                {
                    Assert.IsEmpty(i.Item3);
                }
                else if (i.Item2 == InputType.Query)
                {
                    Assert.IsNotEmpty(i.Item3);
                }
            });

            result.ToList().ForEach(i =>
            {
                if (i.Item2 == InputType.ProductData)
                {
                    Assert.IsEmpty(i.Item4);
                }
                else if (i.Item2 == InputType.Query)
                {
                    if (i.Item4 == "Credits")
                    {
                        Assert.IsNotEmpty(i.Item4);
                    }
                    else
                    {
                        Assert.IsEmpty(i.Item4);
                    }
                }
            });

            this.productDataValidator.Verify(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce);
            this.productDataValidator.Verify(i => i.GetProductValue(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            this.queryValidator.Verify(i => i.GetProducts(It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce);
        }

        private static IEnumerable<string> GetInvalidSet()
        {
            return new []
            {
                "Hello Earth", // Does not contain " is "
                "Is Kepler-22b near to Kepler-69c?", // // Does not contain " is "
                "In the realms of merchant business, Gliese 667Cc is better than Kepler-229, but Kepler-229 is nearer to earth!", // Multiple " is " 
                "how much wood could a woodchuck chuck if a woodchuck could chuck wood ?" // No " is " found
            };
        }

        private static IEnumerable<string> GetValidProductSet()
        {
            return new []
            {
                "glob is I",
                "prok is V",
                "pish is X",
                "tegj is L",
                "glob glob Silver is 34 Credits",
                "glob prok Gold is 57800 Credits",
                "pish pish Iron is 3910 Credits",
            };
        }

        private static IEnumerable<string> GetValidQuerySet()
        {
            return new[]
            {
                "how much is pish tegj glob glob ?",
                "how many Credits is glob prok Silver ?",
                "how many Credits is glob prok Gold ?",
                "how many Credits is glob prok Iron ?",
            };
        }
    }
}