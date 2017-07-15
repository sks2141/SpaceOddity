using NumeralCalculator.Validator;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NumeralCalculator.UnitTests.Validator
{
    [TestFixture]
    public class QueryValidatorTest
    {
        [Test, RequiresThread]
        public void Validate_NullProductsList()
        {
            Assert.That(() => new QueryValidator(null), Throws.TypeOf<ArgumentNullException>());
        }
        
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [Test, RequiresThread]
        public void GetProducts_NullOrEmptyOrWhitespaceContent(string content)
        {
            IQueryValidator validator = new QueryValidator(new string[] { });

            var results = validator.GetProducts(content, 1);

            Assert.IsNull(results);
        }

        [Test, RequiresThread]
        public void GetProducts_ProductNotInProductsList()
        {
            IQueryValidator validator = new QueryValidator(new string[] { "prok", "tegj" });

            var results = validator.GetProducts("glob pish Silver", 1);

            Assert.IsNull(results);
        }

        [Test, RequiresThread]
        public void GetProducts_ValidProducts()
        {
            IQueryValidator validator = new QueryValidator(new string[] { "glob", "Silver", "pish", "Gold" });

            var results = validator.GetProducts("glob pish Silver", 1);

            Assert.IsNotNull(results);

            IEnumerable<string> output = new[] { "glob", "pish", "Silver" };
            CollectionAssert.AreEqual(output, results);
        }
    }
}