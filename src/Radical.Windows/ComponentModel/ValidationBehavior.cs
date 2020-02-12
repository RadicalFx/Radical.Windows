
namespace Radical.Windows.ComponentModel
{
    /// <summary>
    /// Determines the behavior of the validation process.
    /// </summary>
    public enum ValidationBehavior
    {
        /// <summary>
        /// Automatically trigger controls error view if the validation fails.
        /// </summary>
        TriggerValidationErrorsOnFailure,

        /// <summary>
        /// Validation is run without triggering errors on controls in case of validation failure.
        /// </summary>
        RunSilentValidation
    }
}
