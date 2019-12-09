using System;
using System.Collections.Generic;
using System.Linq;
using Radical.Linq;
using Radical.Validation;
using Radical.Windows.Presentation.ComponentModel;

namespace Radical.Windows.Presentation.Services.Validation
{
	/// <summary>
	/// Provides a base implementation of the <see cref="IValidationService"/> interface.
	/// </summary>
	public abstract class AbstractValidationService : IValidationService
	{
		readonly List<ValidationError> _validationErrors = new List<ValidationError>();
		ValidationTools tools = new ValidationTools();

		/// <summary>
		/// Initializes a new instance of the <see cref="AbstractValidationService"/> class.
		/// </summary>
		protected AbstractValidationService()
		{
			IsValid = true;
			MergeValidationErrors = false;
		}

        //readonly HashSet<String> validationCalledOnce = new HashSet<String>();

        ///// <summary>
        ///// Called in order to understand if the validation for the 
        ///// supplied property has already been called at least one time.
        ///// </summary>
        ///// <param name="propertyName">Name of the property.</param>
        ///// <returns><c>True</c> if the supplied property has been validated at least once; otherwise <c>false</c>.</returns>
        //protected virtual Boolean ValidationCalledOnceFor( String propertyName )
        //{
        //    return this.validationCalledOnce.Contains( propertyName );
        //}

        ///// <summary>
        ///// Registers that the validation process has been called 
        ///// at least once for the supplied property.
        ///// </summary>
        ///// <param name="propertyName">Name of the property.</param>
        //protected virtual void RegisterValidationCalledOnceFor( String propertyName )
        //{
        //    this.validationCalledOnce.Add( propertyName );
        //}

		/// <summary>
		/// Validates the specified property.
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <returns>
		/// The validation error message if any; otherwise a null or empty string.
		/// </returns>
		public string Validate(string propertyName )
		{
			return ValidateRuleSet( null, propertyName );
		}

		/// <summary>
		/// Starts the validation process.
		/// </summary>
		/// <param name="ruleSet">The rule set.</param>
		/// <param name="propertyName">The name of the property to validate.</param>
		/// <returns>
		/// The validation error message if any; otherwise a null or empty string.
		/// </returns>
		public string ValidateRuleSet(string ruleSet, string propertyName )
		{
            //if( !this.ValidationCalledOnceFor( propertyName ) )
            //{
            //    /*
            //     * Se non abbiamo mai validato la proprietà significa che siamo 
            //     * allo startup della Window e il motore di validazione di WPF 
            //     * viene triggherato per ogni "set" di ogni binding. Dato che non
            //     * ci interessa visualizzare la Window come non valida sin da 
            //     * subito evitiamo la validazione al primo controllo.
            //     */
            //    this.RegisterValidationCalledOnceFor( propertyName );
            //    return null;
            //}

			if( IsValidationSuspended )
			{
				return null;
			}

			var isValidBeforeValidation = IsValid;

			var results = OnValidateProperty( ruleSet, propertyName );

			var toBeRemoved = _validationErrors
				.Where( e => e.Key == propertyName )
				.ToArray()
				.ForEach( e => _validationErrors.Remove( e ) );

			AddValidationErrors( results.ToArray() );

			var shouldTriggerStatusChanged = toBeRemoved.Any() || results.Any();
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
		public bool IsValid
		{
			get;
			private set;
		}

		/// <summary>
		/// Occurs when validation status changes.
		/// </summary>
		public event EventHandler StatusChanged;

		/// <summary>
		/// Occurs when this service is resetted.
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
					var actual = _validationErrors.SingleOrDefault( ve => ve.Key == error.Key );
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
		///   <c>True</c> if the validation process succedeed; otherwise <c>false</c>.
		/// </returns>
		public bool Validate()
		{
			return ValidateRuleSet( null );
		}

		/// <summary>
		/// Starts the validation process.
		/// </summary>
		/// <param name="ruleSet">The rule set.</param>
		/// <returns>
		///   <c>True</c> if the validation process succedeed; otherwise <c>false</c>.
		/// </returns>
		public virtual bool ValidateRuleSet(string ruleSet )
		{
			if( IsValidationSuspended )
			{
				return IsValid;
			}

			var isValidBeforeValidation = IsValid;
			var result = OnValidate( ruleSet );

			ClearErrors();
			AddValidationErrors( result.ToArray() );

			IsValid = result.None();

			if( IsValid != isValidBeforeValidation || !IsValid )
			{
				OnStatusChanged( EventArgs.Empty );
			}

			return IsValid;
		}

		/// <summary>
		/// Called in order to execute the concrete validation process.
		/// </summary>
		/// <param name="ruleSet">The rule set.</param>
		/// <returns>
		/// A list of <seealso cref="ValidationError"/>.
		/// </returns>
		protected abstract IEnumerable<ValidationError> OnValidate(string ruleSet );

		/// <summary>
		/// Called in order to execute the concrete validation process on the given property.
		/// </summary>
		/// <param name="ruleSet">The rule set.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>
		/// A list of <seealso cref="ValidationError" />.
		/// </returns>
		protected virtual IEnumerable<ValidationError> OnValidateProperty(string ruleSet, string propertyName )
		{
			if( !IsValidationSuspended && !ValidateRuleSet( ruleSet ) )
			{
				/*
				 * Se la validazione fallisce dobbiamo capire se è fallita
				 * per colpa della proprietà che ci è stato chiesto di validare.
				 * Cerchiamo quindi nella lista degli errori uno che abbia come Key
				 * il nome della proprietà
				 */
				return ValidationErrors
					.Where( err => err.Key == propertyName )
					.ToArray();
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
			return ValidationErrors.Select( ve => ve.Key )
                .Distinct()
                .AsReadOnly();
		}

		/// <summary>
		/// Clears the validation state resetting to its default valid value.
		/// </summary>
		public void Reset() 
		{
			Reset( ValidationResetBehavior.All );
		}

		/// <summary>
		/// Clears the validation state resetting to its default valid value.
		/// </summary>
		/// <param name="resetBehavior">The reset behavior.</param>
		public virtual void Reset( ValidationResetBehavior resetBehavior )
		{
			if( ( resetBehavior & ValidationResetBehavior.ErrorsOnly ) == ValidationResetBehavior.ErrorsOnly )
			{
				_validationErrors.Clear();
			}

			IsValid = true;

            //if( ( resetBehavior & ValidationResetBehavior.ValidationTracker ) == ValidationResetBehavior.ValidationTracker )
            //{
            //    this.validationCalledOnce.Clear();
            //}

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
			return tools.GetPropertyDisplayName( propertyName, entity );
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
				if( value != MergeValidationErrors )
				{
					_mergeValidationErrors = value;

					if( ValidationErrors.Any() )
					{
						//reset the errors if any so the have them gruoped
						var actual = ValidationErrors.ToArray();
						_validationErrors.Clear();
						AddValidationErrors( actual );

						OnValidationReset( EventArgs.Empty );
					}
				}
			}
		}
	}
}