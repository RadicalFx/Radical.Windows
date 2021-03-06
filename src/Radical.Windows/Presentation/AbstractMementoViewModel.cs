﻿using Radical.ComponentModel.ChangeTracking;
using Radical.Linq;
using Radical.Model;
using Radical.Reflection;
using Radical.Validation;
using Radical.Windows.ComponentModel;
using Radical.Windows.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Radical.Windows.Presentation
{
    /// <summary>
    /// A base abstract ViewModel with built-in support for validation, error notification and memento.
    /// </summary>
    public abstract class AbstractMementoViewModel :
        MementoEntity,
        IViewModel
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
        /// The validation service to use to validate this view model.
        /// </summary>
        [SkipPropertyValidation]
        protected internal IValidationService ValidationService{ get; set; } = NullValidationService.Instance;

        protected ValidationBehavior DefaultValidationBehavior { get; set; } = ValidationBehavior.TriggerValidationErrorsOnFailure;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMementoViewModel"/> class.
        /// </summary>
        protected AbstractMementoViewModel()
            : base(ChangeTrackingRegistration.AsTransient)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMementoViewModel" /> class.
        /// </summary>
        /// <param name="registration">The registration.</param>
        protected AbstractMementoViewModel(ChangeTrackingRegistration registration)
            : base(registration)
        {

        }

        protected string GetPropertyDisplayName(string propertyName)
        {
            return this.GetProperty(propertyName).GetDisplayName();
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
        [Bindable(false)]
        [SkipPropertyValidation]
        public virtual bool IsValid { get; private set; } = true;

        /// <summary>
        /// Gets the validation errors if any.
        /// </summary>
        /// <value>The validation errors.</value>
        [Bindable(false)]
        [SkipPropertyValidation]
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
        public (bool IsValid, IEnumerable<ValidationError> Errors) Validate(ValidationBehavior behavior)
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

                foreach (var invalid in ValidationErrors.GetInvalidProperties())
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
        ///     <c>true</c> if this instance is triggering validation; otherwise, <c>false</c>.
        /// </value>
        [SkipPropertyValidation]
        protected virtual bool IsTriggeringValidation
        {
            get;
            private set;
        }

        /// <summary>
        /// Called when the <see cref="IChangeTrackingService"/> changes.
        /// </summary>
        /// <param name="newMemento">The new memento service.</param>
        /// <param name="oldMemento">The old memento service.</param>
        protected override void OnMementoChanged(IChangeTrackingService newMemento, IChangeTrackingService oldMemento)
        {
            base.OnMementoChanged(newMemento, oldMemento);

            if (oldMemento != null && !oldMemento.IsDisposed)
            {
                oldMemento.AcceptingChanges -= new EventHandler<CancelEventArgs>(OnAcceptingChanges);
                oldMemento.RejectingChanges -= new EventHandler<CancelEventArgs>(OnRejectingChanges);

                oldMemento.ChangesAccepted -= new EventHandler(OnChangesAccepted);
                oldMemento.ChangesRejected -= new EventHandler(OnChangesRejected);
            }

            if (newMemento != null && !newMemento.IsDisposed)
            {
                newMemento.AcceptingChanges += new EventHandler<CancelEventArgs>(OnAcceptingChanges);
                newMemento.RejectingChanges += new EventHandler<CancelEventArgs>(OnRejectingChanges);

                newMemento.ChangesAccepted += new EventHandler(OnChangesAccepted);
                newMemento.ChangesRejected += new EventHandler(OnChangesRejected);
            }
        }

        void OnAcceptingChanges(object sender, CancelEventArgs e)
        {
            OnAcceptingChanges(e);
        }

        void OnRejectingChanges(object sender, CancelEventArgs e)
        {
            OnRejectingChanges(e);
        }

        void OnChangesAccepted(object sender, EventArgs e)
        {
            OnChangesAccepted();
        }

        void OnChangesRejected(object sender, EventArgs e)
        {
            OnChangesRejected();
        }

        /// <summary>
        /// Called when the changes are ready to be accepted.
        /// </summary>
        /// <param name="e">The <see cref="CancelEventArgs" /> instance containing the event data.</param>
        protected virtual void OnAcceptingChanges(CancelEventArgs e)
        {

        }

        /// <summary>
        /// Called when the changes are ready to be rejected.
        /// </summary>
        /// <param name="e">The <see cref="CancelEventArgs" /> instance containing the event data.</param>
        protected virtual void OnRejectingChanges(CancelEventArgs e)
        {

        }

        /// <summary>
        /// Called when the changes have been accepted.
        /// </summary>
        protected virtual void OnChangesAccepted()
        {

        }

        /// <summary>
        /// Called when have been rejected.
        /// </summary>
        protected virtual void OnChangesRejected()
        {

        }

        /// <summary>
        /// Gets or sets the focused element key.
        /// </summary>
        /// <value>
        /// The focused element key.
        /// </value>
        [MementoPropertyMetadata(TrackChanges = false)]
        [Bindable(false)]
        [SkipPropertyValidation]
        public string FocusedElementKey
        {
            get { return GetPropertyValue(() => FocusedElementKey); }
            set { SetPropertyValue(() => FocusedElementKey, value); }
        }

        /// <summary>
        /// Moves the focus to.
        /// </summary>
        /// <param name="property">The property.</param>
        [System.Obsolete("This function has been obsoleted and will be removed in v3.0.0. Use MoveFocusTo(nameof(property)) instead")]
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
            this.IsResettingValidation = true;
            ValidationErrors.Clear();
            IsValid = true;
            OnPropertyChanged(nameof(IsValid));
            OnErrorsChanged(null);
            this.IsResettingValidation = false;
        }

        /// <summary>
        /// Occurs when errors change.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Raises the ErrorsChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void OnErrorsChanged(String propertyName)
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
                return this.ValidationErrors.ToArray();
            }

            var temp = this.ValidationErrors.Where(e => e.PropertyName == propertyName).ToArray();
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
            get
            {
                var hasErrors = !this.IsValid;
                return hasErrors;
            }
        }
    }
}
