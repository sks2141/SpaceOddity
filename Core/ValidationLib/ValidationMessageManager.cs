using FluentValidation.Results;
using System;
using System.Text;

namespace ValidationLib
{
    /// <summary>
    /// Provides logic to generate presentation data for validated data
    /// </summary>    
    public class ValidationMessageManager : IValidationMessageManager
    {
        /// <summary>
        /// Takes the results of a validation operations and concatenates 
        /// all the error messages into a newline delimited string
        /// </summary>
        /// <param name="validationResult">
        /// The results on the validation operation on an object
        /// </param>
        /// <returns>
        /// A newline delimited string containing all the error messages for 
        /// the object
        /// </returns>
        public string GetValidationString(ValidationResult validationResult)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if(validationResult != null)
            {
                foreach (var result in validationResult.Errors)
                {
                    stringBuilder.Append(result.ErrorMessage);
                    stringBuilder.Append(Environment.NewLine);
                }
            }
            
            return stringBuilder.ToString();
        }
    }
}