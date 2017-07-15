﻿using FluentValidation.Results;

namespace ValidationLib
{
    /// <summary>
    /// Provides logic to generate presentation data for validated data
    /// </summary>
    public interface IValidationMessageManager
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
        string GetValidationString(ValidationResult validationResult);
    }
}