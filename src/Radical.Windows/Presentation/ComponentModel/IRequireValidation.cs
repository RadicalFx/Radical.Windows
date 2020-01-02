using Radical.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Radical.Windows.Presentation.ComponentModel
{
    /// <summary>
    /// Defines a ViewModel, or any other type, as something that supports 
    /// a validation process.
    /// </summary>
    public interface IRequireValidation : INotifyDataErrorInfo
    {
        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        bool IsValid { get; }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <value>The validation errors.</value>
        ObservableCollection<ValidationError> ValidationErrors { get; }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns><c>true</c> if this instance is valid; otherwise, <c>false</c>.</returns>
        (bool IsValid, IEnumerable<ValidationError> Errors) Validate();

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        /// <returns>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        (bool IsValid, IEnumerable<ValidationError> Errors) Validate(ValidationBehavior behavior);

        /// <summary>
        /// Occurs when the validation process is completed.
        /// </summary>
        event EventHandler Validated;

        /// <summary>
        /// Triggers the validation process on this instances forcing all the invalid
        /// fields to notify their invalid status.
        /// </summary>
        void TriggerValidation();

        /// <summary>
        /// Resets the validation status.
        /// </summary>
        void ResetValidation();
    }
}
