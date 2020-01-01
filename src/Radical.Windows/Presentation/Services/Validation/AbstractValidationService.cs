using Radical.Linq;
using Radical.Reflection;
using Radical.Validation;
using Radical.Windows.Presentation.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Radical.Windows.Presentation.Services.Validation
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IValidationService"/> interface.
    /// </summary>
    public abstract class AbstractValidationService : IValidationService
    {
        readonly List<ValidationError> _validationErrors = new List<ValidationError>();

        /// <summary>
        /// Validates the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>
        /// The validation error message if any; otherwise a null or empty string.
        /// </returns>
        public string ValidateProperty(string propertyName)
        {
            if( IsValidationSuspended )
            {
                return null;
            }

            var isValidBeforeValidation = IsValid;

            var results = OnValidateProperty(propertyName);

            var removedErrors = _validationErrors
                .Where( e => e.PropertyName != propertyName )
                .ToArray()
                .ForEach( e => _validationErrors.Remove( e ) );

            AddValidationErrors( results.ToArray() );

            var shouldTriggerStatusChanged = removedErrors.Any() || results.Any();
            IsValid = ValidationErrors.None();

            if( IsValid != isValidBeforeValidation || shouldTriggerStatusChanged )
            {
                OnStatusChanged( EventArgs.Empty );
            }

            if( results.Any() )
            {
                var error = results.Select( err => err.ToString() )
                    .First();

                return error;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the validation process
        /// returns a valid response or not.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the validation process has successfully passed the validation process.; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid{ get; private set; } = true;

        /// <summary>
        /// Occurs when validation status changes.
        /// </summary>
        public event EventHandler StatusChanged;

        /// <summary>
        /// Occurs when this service is reset.
        /// </summary>
        public event EventHandler ValidationReset;

        /// <summary>
        /// Raises the <see cref="E:StatusChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnStatusChanged( EventArgs e )
        {
            var h = StatusChanged;
            if( h != null )
            {
                h( this, e );
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ValidationReset"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnValidationReset( EventArgs e )
        {
            var h = ValidationReset;
            if( h != null )
            {
                h( this, e );
            }
        }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <value>
        /// All the validation errors.
        /// </value>
        public IEnumerable<ValidationError> ValidationErrors
        {
            get { return _validationErrors; }
        }

        /// <summary>
        /// Adds the validation errors.
        /// </summary>
        /// <param name="errors">The errors.</param>
        protected void AddValidationErrors( params ValidationError[] errors )
        {
            Ensure.That( errors ).Named( () => errors ).IsNotNull();

            if( MergeValidationErrors )
            {
                foreach( var error in errors )
                {
                    var actual = _validationErrors.SingleOrDefault( ve => ve.PropertyName == error.PropertyName);
                    if( actual != null )
                    {
                        actual.AddProblems( error.DetectedProblems.ToArray() );
                    }
                    else
                    {
                        _validationErrors.Add( error );
                    }
                }
            }
            else
            {
                _validationErrors.AddRange( errors );
            }
        }

        /// <summary>
        /// Clears all the current validation errors.
        /// </summary>
        protected void ClearErrors()
        {
            _validationErrors.Clear();
        }

        /// <summary>
        /// Starts the validation process.
        /// </summary>
        /// <returns>
        ///   <c>True</c> if the validation process succeeded; otherwise <c>false</c>.
        /// </returns>
        public virtual bool Validate()
        {
            if( IsValidationSuspended )
            {
                return IsValid;
            }

            var isValidBeforeValidation = IsValid;
            var results = OnValidate();

            ClearErrors();
            AddValidationErrors( results.ToArray() );

            IsValid = results.None();

            if( IsValid != isValidBeforeValidation || !IsValid )
            {
                OnStatusChanged( EventArgs.Empty );
            }

            return IsValid;
        }

        /// <summary>
        /// Called in order to execute the concrete validation process.
        /// </summary>
        /// <returns>
        /// A list of <seealso cref="ValidationError"/>.
        /// </returns>
        protected abstract IEnumerable<ValidationError> OnValidate();

        /// <summary>
        /// Called in order to execute the concrete validation process on the given property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// A list of <seealso cref="ValidationError" />.
        /// </returns>
        protected virtual IEnumerable<ValidationError> OnValidateProperty( string propertyName )
        {
            if( !IsValidationSuspended && !Validate() )
            {
                /*
                 * The default implementation of property validation is
                 * very basic and relies on running a full validation and
                 * then on looking for errors related to the validated
                 * property. 
                 */
                return ValidationErrors
                    .Where( err => err.PropertyName == propertyName );
            }

            return new ValidationError[ 0 ];
        }

        /// <summary>
        /// Gets the invalid properties.
        /// </summary>
        /// <returns>
        /// A list of property names that identifies the invalid properties.
        /// </returns>
        public virtual IEnumerable<string> GetInvalidProperties()
        {
            return ValidationErrors.Select( ve => ve.PropertyName)
                .Distinct()
                .AsReadOnly();
        }

        /// <summary>
        /// Clears the validation state resetting to its default valid value.
        /// </summary>
        public void Reset() 
        {
            _validationErrors.Clear();
            IsValid = true;
            OnValidationReset( EventArgs.Empty );
        }

        class ValidationSuspender : IDisposable
        {
            public void Dispose()
            {
                onDisposed();
            }

            readonly Action onDisposed;

            public ValidationSuspender( Action onDisposed )
            {
                this.onDisposed = onDisposed;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the validation process is suspended.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the validation process is suspended; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidationSuspended { get; private set; }

        ValidationSuspender suspender = null;

        /// <summary>
        /// Suspends the validation.
        /// </summary>
        /// <returns>A disposable instance to automatically resume validation on dispose.</returns>
        public IDisposable SuspendValidation()
        {
            if( !IsValidationSuspended )
            {
                IsValidationSuspended = true;
                suspender = new ValidationSuspender( () => ResumeValidation() );
            }

            return suspender;
        }

        /// <summary>
        /// Resumes the validation.
        /// </summary>
        public void ResumeValidation()
        {
            if( IsValidationSuspended )
            {
                IsValidationSuspended = false;
                suspender = null;
            }
        }


        /// <summary>
        /// Gets the display name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public string GetPropertyDisplayName<T>( T entity, System.Linq.Expressions.Expression<Func<T, object>> property )
        {
            var propertyname = property.GetMemberName();
            return GetPropertyDisplayName( entity, propertyname );
        }

        /// <summary>
        /// Gets the display name of the property.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public virtual string GetPropertyDisplayName( object entity, string propertyName )
        {
            var pi = entity.GetType().GetProperty(propertyName);
            if (pi != null && pi.IsAttributeDefined<DisplayNameAttribute>())
            {
                return pi.GetAttribute<DisplayNameAttribute>().DisplayName;
            }
            return null;
        }

        bool _mergeValidationErrors;

        /// <summary>
        /// Gets or sets if the service should merge validation errors related to the same property.
        /// </summary>
        /// <value>
        /// <c>True</c> if the service should merge validation errors related to the same property; otherwise <c>False</c>.
        /// </value>
        /// <exception cref="System.NotImplementedException">
        /// </exception>
        public bool MergeValidationErrors
        {
            get { return _mergeValidationErrors; }
            set
            {
                if (value != MergeValidationErrors)
                {
                    _mergeValidationErrors = value;

                    if (ValidationErrors.Any())
                    {
                        //reset the errors if any so the have them grouped
                        var actual = ValidationErrors.ToArray();
                        _validationErrors.Clear();
                        AddValidationErrors(actual);

                        OnValidationReset(EventArgs.Empty);
                    }
                }
            }
        }
    }
}