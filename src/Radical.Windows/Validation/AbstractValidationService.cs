using Radical.Linq;
using Radical.Validation;
using Radical.Windows.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Radical.Windows.Validation
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IValidationService"/> interface.
    /// </summary>
    public abstract class AbstractValidationService : IValidationService
    {
        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>
        /// The validation error message if any; otherwise a null or empty string.
        /// </returns>
        public (bool IsValid, IEnumerable<ValidationError> Errors) ValidateProperty(string propertyName)
        {
            if (IsValidationSuspended)
            {
                return (true, Array.Empty<ValidationError>());
            }

            var results = OnValidateProperty(propertyName);
            return (results.None(), results);
        }

        /// <summary>
        /// Called in order to execute the concrete validation process on the given property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// A list of <seealso cref="ValidationError" />.
        /// </returns>
        protected virtual IEnumerable<ValidationError> OnValidateProperty(string propertyName)
        {
            if (IsValidationSuspended) 
            {
                return Array.Empty<ValidationError>();
            }

            /*
             * The default implementation of property validation is
             * very basic and relies on running a full validation and
             * then on looking for errors related to the validated
             * property. 
             */
            var (isValid, errors) = Validate();
            return errors.Where(err => err.PropertyName == propertyName);
        }

        /// <summary>
        /// Starts the validation process.
        /// </summary>
        /// <returns>
        ///   <c>True</c> if the validation process succeeded; otherwise <c>false</c>.
        /// </returns>
        public (bool IsValid, IEnumerable<ValidationError> Errors) Validate()
        {
            if (IsValidationSuspended)
            {
                return (true, Array.Empty<ValidationError>());
            }

            //var wasValidBeforeValidation = IsValid;
            var errors = OnValidate();
            if (!MergeValidationErrors)
            {
                return (errors.None(), errors);
            }

            Dictionary<string, ValidationError> merged = new Dictionary<string, ValidationError>();
            foreach (var error in errors)
            {
                if (merged.TryGetValue(error.PropertyName, out ValidationError current))
                {
                    current.AddProblems(error.DetectedProblems);
                }
                else 
                {
                    merged.Add(error.PropertyName, error);
                }
            }

            return (merged.Values.None(), merged.Values);
        }

        /// <summary>
        /// Called in order to execute the concrete validation process.
        /// </summary>
        /// <returns>
        /// A list of <seealso cref="ValidationError"/>.
        /// </returns>
        protected abstract IEnumerable<ValidationError> OnValidate();

        class ValidationSuspender : IDisposable
        {
            public void Dispose()
            {
                onDisposed();
            }

            readonly Action onDisposed;

            public ValidationSuspender(Action onDisposed)
            {
                this.onDisposed = onDisposed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the validation process is suspended.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the validation process is suspended; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidationSuspended { get; private set; }

        ValidationSuspender suspender = null;

        /// <summary>
        /// Suspends the validation.
        /// </summary>
        /// <returns>A disposable instance to automatically resume validation on dispose.</returns>
        public IDisposable SuspendValidation()
        {
            if (!IsValidationSuspended)
            {
                IsValidationSuspended = true;
                suspender = new ValidationSuspender(() => ResumeValidation());
            }

            return suspender;
        }

        /// <summary>
        /// Resumes the validation.
        /// </summary>
        public void ResumeValidation()
        {
            if (IsValidationSuspended)
            {
                IsValidationSuspended = false;
                suspender = null;
            }
        }

        public bool MergeValidationErrors { get; set; }
    }
}