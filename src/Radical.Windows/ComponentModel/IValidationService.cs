using Radical.Validation;
using System;
using System.Collections.Generic;

namespace Radical.Windows.ComponentModel
{
    /// <summary>
    /// Defines a validation service that can be used to validate an entity or a ViewModel.
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Starts the validation process.
        /// </summary>
        /// <returns><c>True</c> if the validation process succeeded; otherwise <c>false</c>.</returns>
        (bool IsValid, IEnumerable<ValidationError> Errors) Validate();

        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The validation error message if any; otherwise a null or empty string.</returns>
        (bool IsValid, IEnumerable<ValidationError> Errors) ValidateProperty(string propertyName);

        /// <summary>
        /// Gets a value indicating whether the validation process is suspended.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the validation process is suspended; otherwise, <c>false</c>.
        /// </value>
        bool IsValidationSuspended { get; }

        /// <summary>
        /// Suspends the validation.
        /// </summary>
        /// <returns>A disposable instance to automatically resume validation on dispose.</returns>
        IDisposable SuspendValidation();

        /// <summary>
        /// Resumes the validation.
        /// </summary>
        void ResumeValidation();

        /// <summary>
        /// Gets or sets if the service should merge validation errors related to the same property.
        /// </summary>
        /// <value>
        /// <c>True</c> if the service should merge validation errors related to the same property; otherwise <c>False</c>.
        /// </value>
        bool MergeValidationErrors { get; set; }
    }
}
