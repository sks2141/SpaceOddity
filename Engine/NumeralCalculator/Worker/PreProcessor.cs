using NumeralCalculator.Validator;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Utilities;

namespace NumeralCalculator.Worker
{
    public class PreProcessor : IPreProcessor
    {
        private const string CREDITS_TEXT = "Credits";

        private readonly IProductDataValidator productDataValidator;
        private readonly IQueryValidator queryValidator;
        
        public PreProcessor(
            IProductDataValidator productDataValidator, 
            IQueryValidator queryValidator)
        {
            this.productDataValidator = productDataValidator;
            this.queryValidator = queryValidator;
        }

        public IEnumerable<Tuple<int, InputType, IEnumerable<string>, string>> Process(IEnumerable<string> contents)
        {
            ICollection<Tuple<int, InputType, IEnumerable<string>, string>> result = new Collection<Tuple<int, InputType, IEnumerable<string>, string>>();

            int lineNumber = 0;
            
            foreach (string content in contents)
            {
                try
                {
                    InputType inputType = this.GetInputType(content);
                    IEnumerable<string> products = new Collection<string>(); ;
                    string productValue = string.Empty;

                    this.ProcessContent(content, ++lineNumber, inputType, out products, out productValue);
                    
                    result.Add(new Tuple<int, InputType, IEnumerable<string>, string>
                                (lineNumber, inputType, products, productValue));
                }
                catch (Exception ex)
                {
                    SLogger.LogErrorFormat(ex, "Exception found when Preprocessing line: {0}.", lineNumber);
                }
            }

            return result;
        }

        private void ProcessContent(string content, int lineNumber, 
            InputType typeOfInput, out IEnumerable<string> products, out string productValue)
        {
            SLogger.LogInfoFormat("Preprocessing line number {0} with content {1}", lineNumber, content);

            products = new Collection<string>();
            productValue = string.Empty;

            if (content.IndexOf(" is ", StringComparison.OrdinalIgnoreCase) == -1)
            {
                SLogger.LogWarnFormat("{0} at line number {1} does not contain 'is' keyword. Skipping...", content, lineNumber);
                return;
            }

            string[] words = content.Split(new string[] { " is " }, StringSplitOptions.None);

            if (words.Length != 2)
            {
                SLogger.LogWarnFormat("{0} at line number {1} is not properly formatted. Skipping...", content, lineNumber);
                return;
            }

            switch (typeOfInput)
            {
                case InputType.ProductData:
                {
                    products = this.productDataValidator.GetProducts(words[0].Trim(), lineNumber);
                    if(products == null || !products.Any())
                    {
                        return;
                    }

                    productValue = this.productDataValidator.GetProductValue(words[1].Trim(), lineNumber);
                }
                break;

                case InputType.Query:
                {
                    products = this.queryValidator.GetProducts(words[1].Replace("?",string.Empty).Trim(), lineNumber);

                    productValue = words[0].IndexOf(CREDITS_TEXT) != -1 ? CREDITS_TEXT : string.Empty;
                }
                break;

                default: // This won't hit, since GetInputType will always return either of ProductData or Query as InputType
                    throw new InvalidOperationException("Invalid InputType Enum.");
            }
        }

        private InputType GetInputType(string content)
        {
            return content.IndexOf("?", StringComparison.OrdinalIgnoreCase) != -1 ? InputType.Query : InputType.ProductData;
        }
    }
}