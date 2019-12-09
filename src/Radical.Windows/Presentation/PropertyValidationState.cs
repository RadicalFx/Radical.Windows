using Radical.Validation;
using System;

namespace Radical.Windows.Presentation
{
    class PropertyValidationState : IDisposable
    {
        string actual = null;

        public IDisposable BeginPropertyValidation(string propertyName )
        {
            Ensure.That( actual )
                .WithMessage( "Cannot begin property validation for '{0}', there is already an ongoing property validation for '{1}'", propertyName, actual )
                .Is( null );

            actual = propertyName;

            return this;
        }

        public bool IsValidatingProperty(string propertyName )
        {
            return actual == propertyName;
        }

        public void Dispose()
        {
            actual = null;
        }
    }
}
