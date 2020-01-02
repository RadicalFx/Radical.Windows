using Radical.Linq;
using Radical.Model;
using Radical.Reflection;
using Radical.Validation;
using Radical.Windows.Presentation.ComponentModel;
using Radical.Windows.Presentation.Services.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Radical.Windows.Presentation
{
    /// <summary>
    /// A base abstract ViewModel with built-in support for validation, error notification.
    /// </summary>
    public abstract class AbstractViewModel :
        Entity,
        IViewModel,
        ISupportInitialize
    {
        /// <summary>
        /// Gets or sets the view. The view property is intended only for
        /// infrastructural purpose. It is required to hold the one-to-one
        /// relation between the view and the view model.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        [Bindable(false)]
        [SkipPropertyValidation]
        System.Windows.DependencyObject IViewModel.View { get; set; }

        readonly PropertyValidationState validationState = new PropertyValidationState();

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (IsValidationEnabled
                && RunValidationOnPropertyChanged
                && !IsResettingValidation
                && !IsTriggeringValidation
                && !SkipPropertyValidation(e.PropertyName)
                && !validationState.IsValidatingProperty(e.PropertyName))
            {
                ValidateProperty(e.PropertyName);
            }

            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// Determines if property validation should be skipped for the given property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected virtual bool SkipPropertyValidation(string propertyName)
        {
            var pi = GetType().GetProperty(propertyName);
            if (pi == null)
            {
                return true;
            }

            return pi.IsAttributeDefined<SkipPropertyValidationAttribute>();
        }

        /// <summary>
        /// Gets a value indication if validation is enabled or not.
        /// </summary>
        [SkipPropertyValidation]
        protected virtual bool IsValidationEnabled
        {
            get
            {
                return this is INotifyDataErrorInfo
                    || this is IRequireValidation;
            }
        }

        IValidationService _validationService;

        /// <summary>
        /// Gets the validation service.
        /// </summary>
        /// <value>The validation service.</value>
        [SkipPropertyValidation]
        protected IValidationService ValidationService
        {
            get
            {
                if (_validationService == null)
                {
                    _validationService = GetValidationService();
                }

                return _validationService;
            }
        }

        protected ValidationBehavior DefaultValidationBehavior { get; set; } = ValidationBehavior.TriggerValidationErrorsOnFailure;

        /// <summary>
        /// Gets the validation service, this method is called once the first time
        /// the validation service is accessed, inheritors should override this method
        /// in order to provide a <see cref="IValidationService"/> implementation.
        /// </summary>
        /// <returns>The validation service to use to validate this view model.</returns>
        protected virtual IValidationService GetValidationService()
        {
            return NullValidationService.Instance;
        }

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        void ISupportInitialize.BeginInit()
        {

        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        void ISupportInitialize.EndInit()
        {

        }

        protected (bool IsValid, IEnumerable<ValidationError> Errors) ValidateProperty(string propertyName)
        {
            return ValidateProperty(propertyName, DefaultValidationBehavior);
        }

        /// <summary>
        /// Validates the given property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns>
        /// The first validation error, if any; Otherwise <c>null</c>.
        /// </returns>
        protected virtual (bool IsValid, IEnumerable<ValidationError> Errors) ValidateProperty(string propertyName, ValidationBehavior behavior)
        {
            (bool IsValid, IEnumerable<ValidationError> Errors) validationResult = (true, new ValidationError[0]);
            if (ValidationService.IsValidationSuspended)
            {
                return validationResult;
            }

            using (validationState.BeginPropertyValidation(propertyName))
            {
                var wasValid = IsValid;

                validationResult = ValidationService.ValidateProperty(propertyName);
                IsValid = validationResult.IsValid;

                var validationStatusChanged = ValidationErrors.IsValidationStatusChanged(validationResult.Errors, propertyName);
                if (validationStatusChanged)
                {
                    ValidationErrors.SyncValidationErrorsFrom(validationResult.Errors, propertyName);
                }

                if (validationStatusChanged && behavior == ValidationBehavior.TriggerValidationErrorsOnFailure)
                {
                    OnPropertyChanged(propertyName);
                    OnErrorsChanged(propertyName);
                }

                if (IsValid != wasValid)
                {
                    OnPropertyChanged(nameof(IsValid));
                    OnPropertyChanged(nameof(HasErrors));
                }
            }

            OnValidated();

            return validationResult;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        [Bindable(false), SkipPropertyValidation]
        public virtual bool IsValid { get; private set; } = true;

        /// <summary>
        /// Gets the validation errors if any.
        /// </summary>
        /// <value>The validation errors.</value>
        [Bindable(false), SkipPropertyValidation]
        public virtual ObservableCollection<ValidationError> ValidationErrors { get; } = new ObservableCollection<ValidationError>();

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns><c>True</c> if this instance is valid; otherwise <c>false</c>.</returns>
        public (bool IsValid, IEnumerable<ValidationError> Errors) Validate()
        {
            return Validate(DefaultValidationBehavior);
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        /// <returns>
        ///   <c>True</c> if this instance is valid; otherwise <c>false</c>.
        /// </returns>
        public virtual (bool IsValid, IEnumerable<ValidationError> Errors) Validate(ValidationBehavior behavior)
        {
            if (ValidationService.IsValidationSuspended)
            {
                return (true, new ValidationError[0]);
            }

            var wasValid = IsValid;

            var (isValid, errors) = ValidationService.Validate();
            IsValid = isValid;

            var isValidationStatusChanged = ValidationErrors.IsValidationStatusChanged(errors);
            if (isValidationStatusChanged) 
            {
                ValidationErrors.SyncValidationErrorsFrom(errors);
            }
            
            if (behavior == ValidationBehavior.TriggerValidationErrorsOnFailure && !IsValid)
            {
                TriggerValidation();
            }

            if (IsValid != wasValid)
            {
                OnPropertyChanged(() => IsValid);
                OnPropertyChanged(() => HasErrors);
            }

            OnValidated();
            return (isValid, errors);
        }

        /// <summary>
        /// Occurs when the validation process terminates.
        /// </summary>
        public event EventHandler Validated;

        /// <summary>
        /// Raises the Validated event.
        /// </summary>
        protected virtual void OnValidated()
        {
            Validated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Triggers the validation.
        /// </summary>
        public virtual void TriggerValidation()
        {
            if (!IsTriggeringValidation)
            {
                IsTriggeringValidation = true;

                foreach (var invalid in ValidationErrors.Select(ve => ve.PropertyName).Distinct())
                {
                    OnPropertyChanged(invalid);
                }

                IsTriggeringValidation = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is triggering validation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is triggering validation; otherwise, <c>false</c>.
        /// </value>
        [SkipPropertyValidation]
        protected virtual bool IsTriggeringValidation
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the focused element key.
        /// </summary>
        /// <value>
        /// The focused element key.
        /// </value>
        [Bindable(false), SkipPropertyValidation]
        [MementoPropertyMetadata(TrackChanges = false)]
        public string FocusedElementKey
        {
            get { return GetPropertyValue(() => FocusedElementKey); }
            set { SetPropertyValue(() => FocusedElementKey, value); }
        }

        /// <summary>
        /// Moves the focus to.
        /// </summary>
        /// <param name="property">The property.</param>
        protected virtual void MoveFocusTo<T>(Expression<Func<T>> property)
        {
            EnsureNotDisposed();

            var propertyName = property.GetMemberName();
            MoveFocusTo(propertyName);
        }

        /// <summary>
        /// Moves the focus to.
        /// </summary>
        /// <param name="focusedElementKey">The focused element key.</param>
        protected virtual void MoveFocusTo(string focusedElementKey)
        {
            EnsureNotDisposed();

            FocusedElementKey = focusedElementKey;
        }

        /// <summary>
        /// Determines if each time a property changes the validation process should be run. The default value is <c>true</c>.
        /// </summary>
        [SkipPropertyValidation]
        protected bool RunValidationOnPropertyChanged { get; set; } = true;

        /// <summary>
        /// <c>True</c> if the current ValidationService is resetting the validation status; Otherwise <c>false</c>.
        /// </summary>
        [SkipPropertyValidation]
        protected bool IsResettingValidation { get; private set; }

        /// <summary>
        /// Resets the validation status.
        /// </summary>
        public virtual void ResetValidation()
        {
            IsResettingValidation = true;
            ValidationErrors.Clear();
            IsValid = true;
            OnPropertyChanged(nameof(IsValid));
            OnErrorsChanged(null);
            IsResettingValidation = false;
        }

        /// <summary>
        /// Occurs when errors change.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Raises the ErrorsChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                return ValidationErrors.ToArray();
            }

            var temp = ValidationErrors.Where(e => e.PropertyName == propertyName).ToArray();
            return temp;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        [Bindable(false)]
        [SkipPropertyValidation]
        public bool HasErrors
        {
            get { return !IsValid; }
        }
    }
}