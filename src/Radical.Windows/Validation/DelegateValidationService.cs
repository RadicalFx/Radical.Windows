﻿using Radical.Validation;
using System;
using System.Collections.Generic;

namespace Radical.Windows.Validation
{
    /// <summary>
    /// A validation service that delegates the validation process to a user defined handler.
    /// </summary>
    /// <remarks>
    /// This validation service implementation is the ideal bridge to link the Radical.Presentation
    /// validation process with an external validation system, e.g. the one supplied by the Validation 
    /// Application Block of the Enterprise Library.
    /// </remarks>
    public sealed class DelegateValidationService : AbstractValidationService
    {
        readonly Func<IEnumerable<ValidationError>> validationCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateValidationService"/> class.
        /// </summary>
        /// <param name="validationCallback">The validation callback.</param>
        public DelegateValidationService( Func<IEnumerable<ValidationError>> validationCallback )
        {
            Ensure.That( validationCallback ).Named( () => validationCallback ).IsNotNull();

            this.validationCallback = validationCallback;
        }

        /// <summary>
        /// Called in order to execute the concrete validation process.
        /// </summary>
        /// <returns>
        /// A list of <seealso cref="ValidationError"/>.
        /// </returns>
        protected override IEnumerable<ValidationError> OnValidate()
        {
            return validationCallback();
        }
    }
}
