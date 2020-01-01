using Radical.Validation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Radical.Windows.Presentation.ComponentModel
{
    /// <summary>
    /// Determines the reset behavior.
    /// </summary>
    [Flags]
    public enum ValidationResetBehavior
    {
        /// <summary>
        /// Resets only the errors collection.
        /// </summary>
        ErrorsOnly,

        /// <summary>
        /// Resets only the validation tracker that tracks if validation for properties has been called at least once.
        /// </summary>
        ValidationTracker,

        /// <summary>
        /// Resets both the validation tracker and the errors collection.
        /// </summary>
        All = ErrorsOnly | ValidationTracker
    }

    /// <summary>
    /// Defines a validation service that can be used to validate an entity or a ViewModel.
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Gets a value indicating whether the validation process
        /// returns a valid response or not.
        /// </summary>
        /// <value><c>true</c> if the validation process has successfully passed the validation process.; otherwise, <c>false</c>.</value>
        bool IsValid { get; }

        /// <summary>
        /// Occurs when validation status changes.
        /// </summary>
        event EventHandler StatusChanged;

        /// <summary>
        /// Occurs when this service is reset.
        /// </summary>
        event EventHandler ValidationReset;

        /// <summary>
        /// Gets the invalid properties.
        /// </summary>
        /// <returns>A list of property names that identifies the invalid properties.</returns>
        IEnumerable<string> GetInvalidProperties();

        /// <summary>
        /// Starts the validation process.
        /// </summary>
        /// <returns><c>True</c> if the validation process succeeded; otherwise <c>false</c>.</returns>
        bool Validate();

        /// <summary>
        /// Starts the validation process.
        /// </summary>
        /// <param name="ruleSet">The rule set.</param>
        /// <returns>
        ///   <c>True</c> if the validation process succeeded; otherwise <c>false</c>.
        /// </returns>
        bool ValidateRuleSet(string ruleSet );

        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The validation error message if any; otherwise a null or empty string.</returns>
        string Validate(string propertyName );

        /// <summary>
        /// Starts the validation process.
        /// </summary>
        /// <param name="ruleSet">The rule set.</param>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <returns>The validation error message if any; otherwise a null or empty string.</returns>
        string ValidateRuleSet(string ruleSet, string propertyName );

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <value>All the validation errors.</value>
        IEnumerable<ValidationError> ValidationErrors { get; }

        /// <summary>
        /// Clears the validation state resetting to it its default valid value.
        /// </summary>
        void Reset();

        /// <summary>
        /// Clears the validation state resetting to it its default valid value.
        /// </summary>
        /// <param name="resetBehavior">The reset behavior.</param>
        void Reset(ValidationResetBehavior resetBehavior);

        /// <summary>
        /// Gets a value indicating whether the validation process is suspended.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the validation process is suspended; otherwise, <c>false</c>.
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
        /// Gets the display name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        string GetPropertyDisplayName<T>( T entity, Expression<Func<T, object>> property );

        /// <summary>
        /// Gets the display name of the property.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        string GetPropertyDisplayName(object entity, string propertyName );

        /// <summary>
        /// Gets or sets if the service should merge validation errors related to the same property.
        /// </summary>
        /// <value>
        /// <c>True</c> if the service should merge validation errors related to the same property; otherwise <c>False</c>.
        /// </value>
        bool MergeValidationErrors { get; set; }
    }
}
