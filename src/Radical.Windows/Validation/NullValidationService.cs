using Radical.Validation;
using Radical.Windows.ComponentModel;
using System.Collections.Generic;

namespace Radical.Windows.Validation
{
    /// <summary>
    /// A default fake implementation of the <see cref="IValidationService"/> interface.
    /// </summary>
    public sealed class NullValidationService : AbstractValidationService
    {
        /// <summary>
        /// A default empty instance of a validation service.
        /// </summary>
        public static readonly IValidationService Instance = new NullValidationService();

        /// <summary>
        /// Prevents a default instance of the <see cref="NullValidationService"/> class from being created.
        /// </summary>
        private NullValidationService()
            : base()
        {
            SuspendValidation();
        }

        private static readonly ValidationError[] emptyErrors = System.Array.Empty<ValidationError>();

        /// <summary>
        /// Called in order to execute the concrete validation process.
        /// </summary>
        /// <returns>
        /// A list of <seealso cref="ValidationError"/>.
        /// </returns>
        protected override IEnumerable<ValidationError> OnValidate()
        {
            return emptyErrors;
        }
    }
}
