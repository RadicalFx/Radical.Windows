﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.ComponentModel.Validation;
using Radical.Validation;
using Radical.Windows.ComponentModel;
using Radical.Windows.Presentation;
using Radical.Windows.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace Test.Radical.Windows.Presentation
{
    [TestClass]
    public class AbstractMementoViewModelTests
    {
        abstract class TestMementoViewModel : AbstractMementoViewModel
        {
            bool? _forceIsValidationEnabledTo;
            internal void ValidateUsing(IValidationService validationService, bool? forceIsValidationEnabledTo = null)
            {
                ValidationService = validationService;
                _forceIsValidationEnabledTo = forceIsValidationEnabledTo;
            }

            protected override bool IsValidationEnabled
            {
                get
                {
                    if (_forceIsValidationEnabledTo.HasValue)
                    {
                        return _forceIsValidationEnabledTo.Value;
                    }

                    return base.IsValidationEnabled;
                }
            }

            public bool Test_IsValidationEnabled { get { return IsValidationEnabled; } }

            public (bool IsValid, IEnumerable<ValidationError> Errors) Test_ValidateProperty(string propertyName)
            {
                return Test_ValidateProperty(propertyName, ValidationBehavior.TriggerValidationErrorsOnFailure);
            }

            public (bool IsValid, IEnumerable<ValidationError> Errors) Test_ValidateProperty(string propertyName, ValidationBehavior behavior)
            {
                return ValidateProperty(propertyName, behavior);
            }

            public void Test_RaisePropertyChanged<T>(Expression<Func<T>> property)
            {
                OnPropertyChanged(property);
            }

            public void Test_SetInitialPropertyValue<T>(Expression<Func<T>> property, T value)
            {
                SetInitialPropertyValue(property, value);
            }
        }

        class SampleTestMementoViewModel : TestMementoViewModel
        {
            [Required(AllowEmptyStrings = false)]
            [StringLength(10, MinimumLength = 5)]
            public string NotNullNotEmpty
            {
                get { return GetPropertyValue(() => NotNullNotEmpty); }
                set { SetPropertyValue(() => NotNullNotEmpty, value); }
            }

            public string Another
            {
                get { return GetPropertyValue(() => Another); }
                set { SetPropertyValue(() => Another, value); }
            }

            public string AnotherOne
            {
                get { return GetPropertyValue(() => AnotherOne); }
                set { SetPropertyValue(() => AnotherOne, value); }
            }

            public string OnceMore
            {
                get { return GetPropertyValue(() => OnceMore); }
                set { SetPropertyValue(() => OnceMore, value); }
            }
        }

        class SampleTestMementoViewModelWithValidationCallback : SampleTestMementoViewModel, IRequireValidationCallback<SampleTestMementoViewModelWithValidationCallback>
        {
            public Action<ValidationContext<SampleTestMementoViewModelWithValidationCallback>> Test_OnValidate { get; set; }

            public void OnValidate(ValidationContext<SampleTestMementoViewModelWithValidationCallback> context)
            {
                Test_OnValidate(context);
            }
        }


        //class ImplementsIDataErrorInfo : TestViewModel, IDataErrorInfo
        //{

        //}

        //class ImplementsICanBeValidated : TestViewModel, ICanBeValidated
        //{

        //}

        class ImplementsINotifyDataErrorInfo : TestMementoViewModel, INotifyDataErrorInfo
        {

        }

        class ImplementsIRequireValidation : TestMementoViewModel, IRequireValidation
        {

        }
        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_with_no_validation_service_always_validates_to_true()
        {
            var sut = new SampleTestMementoViewModel();
            var result = sut.Validate();

            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_with_no_validation_service_is_always_validat()
        {
            var sut = new SampleTestMementoViewModel();
            var isValid = sut.IsValid;

            Assert.IsTrue(isValid);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_with_no_validation_service_has_no_errors()
        {
            var sut = new SampleTestMementoViewModel();
            sut.Validate();
            var errors = sut.ValidationErrors;

            Assert.IsTrue(errors.Count == 0);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_with_validation_service_should_generate_expected_errors()
        {
            var sut = new SampleTestMementoViewModel();
            sut.ValidateUsing(
                new DataAnnotationValidationService<SampleTestMementoViewModel>(sut));
            sut.Validate();
            var errors = sut.ValidationErrors;

            Assert.IsTrue(errors.Count == 1);
        }

        //[TestMethod]
        //[TestCategory( "AbstractMementoViewModel" ), TestCategory( "Validation" )]
        //public void AbstractMementoViewModel_as_IDataErrorInfo_with_validation_service_invalid_property_not_validated_is_valid()
        //{
        //    var sut = new SampleTestViewModel();
        //    sut.ValidateUsing(
        //        new DataAnnotationValidationService<SampleTestViewModel>( sut ) );

        //    var error = sut[ "NotNullNotEmpty" ];

        //    Assert.IsNull( error );
        //}

        //[TestMethod]
        //[TestCategory( "AbstractMementoViewModel" ), TestCategory( "Validation" )]
        //public void AbstractMementoViewModel_as_IDataErrorInfo_with_validation_service_invalid_property_not_validated_is_valid_even_if_called_multiple_times()
        //{
        //    var sut = new SampleTestViewModel();
        //    sut.ValidateUsing(
        //        new DataAnnotationValidationService<SampleTestViewModel>( sut ) );

        //    var error = sut[ "NotNullNotEmpty" ];
        //    error = sut[ "NotNullNotEmpty" ];
        //    error = sut[ "NotNullNotEmpty" ];

        //    Assert.IsNull( error );
        //}

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_as_INotifyDataErrorInfo_with_validation_service_invalid_property_not_validated_is_valid()
        {
            var sut = new SampleTestMementoViewModel();
            sut.ValidateUsing(
                new DataAnnotationValidationService<SampleTestMementoViewModel>(sut));

            var errors = sut.GetErrors("NotNullNotEmpty").OfType<object>();

            Assert.AreEqual(0, errors.Count());
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_as_INotifyDataErrorInfo_with_validation_service_invalid_property_not_validated_is_valid_even_if_called_multiple_times()
        {
            var sut = new SampleTestMementoViewModel();
            sut.ValidateUsing(
                new DataAnnotationValidationService<SampleTestMementoViewModel>(sut));

            var errors = sut.GetErrors("NotNullNotEmpty").OfType<object>();
            errors = sut.GetErrors("NotNullNotEmpty").OfType<object>();
            errors = sut.GetErrors("NotNullNotEmpty").OfType<object>();

            Assert.AreEqual(0, errors.Count());
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_INotifyDataErrorInfo_IsValidationEnabled_should_be_true()
        {
            var sut = new ImplementsINotifyDataErrorInfo();
            Assert.IsTrue(sut.Test_IsValidationEnabled);
        }

        //[TestMethod]
        //[TestCategory( "AbstractMementoViewModel" ), TestCategory( "Validation" )]
        //public void AbstractMementoViewModel_IDataErrorInfo_IsValidationEnabled_should_be_true()
        //{
        //    var sut = new ImplementsIDataErrorInfo();
        //    Assert.IsTrue( sut.Test_IsValidationEnabled );
        //}

        //[TestMethod]
        //[TestCategory( "AbstractMementoViewModel" ), TestCategory( "Validation" )]
        //public void AbstractMementoViewModel_ICanBeValidated_IsValidationEnabled_should_be_true()
        //{
        //    var sut = new ImplementsICanBeValidated();
        //    Assert.IsTrue( sut.Test_IsValidationEnabled );
        //}

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_IRequireValidation_IsValidationEnabled_should_be_true()
        {
            var sut = new ImplementsIRequireValidation();
            Assert.IsTrue(sut.Test_IsValidationEnabled);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_PropertyChanged_is_raised_GetErrors_should_contain_expected_errors()
        {
            IEnumerable<object> errors = null;

            var sut = new SampleTestMementoViewModel();
            sut.PropertyChanged += (s, e) =>
            {
                errors = sut.GetErrors("NotNullNotEmpty").OfType<object>();
            };

            sut.ValidateUsing(
                new DataAnnotationValidationService<SampleTestMementoViewModel>(sut));
            sut.NotNullNotEmpty = "";

            Assert.IsNotNull(errors);
            Assert.AreEqual(0, errors.Count());
        }

        //[TestMethod]
        //[TestCategory( "AbstractMementoViewModel" ), TestCategory( "Validation" )]
        //public void AbstractMementoViewModel_PropertyChanged_is_raised_Error_indexer_should_contain_expected_errors()
        //{
        //    string errors = null;

        //    var sut = new SampleTestViewModel();
        //    sut.PropertyChanged += ( s, e ) =>
        //    {
        //        errors = sut[ "NotNullNotEmpty" ];
        //    };

        //    sut.ValidateUsing(
        //        new DataAnnotationValidationService<SampleTestViewModel>( sut ),
        //        forceIsValidationEnabledTo: true );
        //    sut.NotNullNotEmpty = "";

        //    Assert.IsFalse(string.IsNullOrWhiteSpace( errors ) );
        //}

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_PropertyChanged_is_raised_IsValid_should_be_false()
        {
            bool isValid = true;

            var sut = new SampleTestMementoViewModel();
            sut.PropertyChanged += (s, e) =>
            {
                isValid = sut.IsValid;
            };

            sut.ValidateUsing(
                new DataAnnotationValidationService<SampleTestMementoViewModel>(sut),
                forceIsValidationEnabledTo: true);
            sut.NotNullNotEmpty = "";

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_PropertyChanged_is_raised_ValidationErrors_should_contain_expected_errors()
        {
            ObservableCollection<ValidationError> errors = null;

            var sut = new SampleTestMementoViewModel();
            sut.PropertyChanged += (s, e) =>
            {
                errors = sut.ValidationErrors;
            };

            sut.ValidateUsing(
                new DataAnnotationValidationService<SampleTestMementoViewModel>(sut),
                forceIsValidationEnabledTo: true);
            sut.NotNullNotEmpty = "";

            Assert.IsNotNull(errors);
            Assert.AreEqual(1, errors.Count);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_when_validation_status_changes_ErrorsChanged_should_be_raised()
        {
            bool raised = false;

            var sut = new SampleTestMementoViewModel();
            sut.ErrorsChanged += (s, e) =>
            {
                raised = true;
            };

            sut.ValidateUsing(
                new DataAnnotationValidationService<SampleTestMementoViewModel>(sut),
                forceIsValidationEnabledTo: true);
            sut.NotNullNotEmpty = "";

            Assert.IsTrue(raised);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_when_validation_status_changes_Validated_should_be_raised()
        {
            bool raised = false;

            var sut = new SampleTestMementoViewModel();
            sut.Validated += (s, e) =>
            {
                raised = true;
            };

            sut.ValidateUsing(
                new DataAnnotationValidationService<SampleTestMementoViewModel>(sut),
                forceIsValidationEnabledTo: true);
            sut.NotNullNotEmpty = "";

            Assert.IsTrue(raised);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_when_validation_is_reset_ErrorsChanged_should_be_raised()
        {
            bool raised = false;

            var sut = new SampleTestMementoViewModel();
            sut.ErrorsChanged += (s, e) =>
            {
                raised = true;
            };

            sut.ValidateUsing(
                new DataAnnotationValidationService<SampleTestMementoViewModel>(sut),
                forceIsValidationEnabledTo: true);
            sut.ResetValidation();

            Assert.IsTrue(raised);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_when_validation_status_changes_PropertyChanged_for_IsValid_property_should_be_raised()
        {
            bool raised = false;

            var sut = new SampleTestMementoViewModel();
            sut.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "IsValid")
                {
                    raised = true;
                }
            };

            sut.ValidateUsing(
                new DataAnnotationValidationService<SampleTestMementoViewModel>(sut),
                forceIsValidationEnabledTo: true);
            sut.NotNullNotEmpty = "";

            Assert.IsTrue(raised);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_when_validation_status_changes_PropertyChanged_for_HasErrors_property_should_be_raised()
        {
            bool raised = false;

            var sut = new SampleTestMementoViewModel();
            sut.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "HasErrors")
                {
                    raised = true;
                }
            };

            sut.ValidateUsing(
                new DataAnnotationValidationService<SampleTestMementoViewModel>(sut),
                forceIsValidationEnabledTo: true);
            sut.NotNullNotEmpty = "";

            Assert.IsTrue(raised);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_when_validating_entire_entity_PropertyChanged_for_HasErrors_property_should_be_raised()
        {
            bool raised = false;

            var sut = new SampleTestMementoViewModel();
            sut.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "HasErrors")
                {
                    raised = true;
                }
            };

            sut.ValidateUsing(
                new DataAnnotationValidationService<SampleTestMementoViewModel>(sut),
                forceIsValidationEnabledTo: true);

            var result = sut.Validate();

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(raised);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_when_validating_entire_entity_PropertyChanged_for_IsValid_property_should_be_raised()
        {
            bool raised = false;

            var sut = new SampleTestMementoViewModel();
            sut.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(sut.IsValid))
                {
                    raised = true;
                }
            };

            sut.ValidateUsing(
                new DataAnnotationValidationService<SampleTestMementoViewModel>(sut),
                forceIsValidationEnabledTo: true);

            var result = sut.Validate();

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(raised);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractViewModel_When_merge_errors_changes_it_should_not_fail()
        {
            var sut = new SampleTestMementoViewModel();
            var svc = new DataAnnotationValidationService<SampleTestMementoViewModel>(sut);
            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);

            sut.Validate();
            svc.MergeValidationErrors = !svc.MergeValidationErrors;
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_When_ValidateProperty_with_Silent_Validation_PropertyChanged_event_should_not_be_raised()
        {
            List<string> raised = new List<string>();
            var propName = "NotNullNotEmpty";

            var sut = new SampleTestMementoViewModel();
            var svc = new DataAnnotationValidationService<SampleTestMementoViewModel>(sut);
            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);
            sut.PropertyChanged += (s, e) => raised.Add(e.PropertyName);

            sut.Test_ValidateProperty(propName, ValidationBehavior.RunSilentValidation);

            Assert.IsFalse(raised.Contains(propName));
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_When_ValidateProperty_PropertyChanged_event_should_be_raised()
        {
            List<string> raised = new List<string>();
            var propName = "NotNullNotEmpty";

            var sut = new SampleTestMementoViewModel();
            var svc = new DataAnnotationValidationService<SampleTestMementoViewModel>(sut);
            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);
            sut.PropertyChanged += (s, e) => raised.Add(e.PropertyName);

            sut.Test_ValidateProperty(propName);

            Assert.IsTrue(raised.Contains(propName));
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_When_ValidateProperty_with_Trigger_berhavior_PropertyChanged_event_should_be_raised()
        {
            List<string> raised = new List<string>();
            var propName = "NotNullNotEmpty";

            var sut = new SampleTestMementoViewModel();
            var svc = new DataAnnotationValidationService<SampleTestMementoViewModel>(sut);
            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);
            sut.PropertyChanged += (s, e) => raised.Add(e.PropertyName);

            sut.Test_ValidateProperty(propName, ValidationBehavior.TriggerValidationErrorsOnFailure);

            Assert.IsTrue(raised.Contains(propName));
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_When_ValidateProperty_with_run_silent_validation_ErrorsChanged_event_should_not_be_raised()
        {
            List<string> raised = new List<string>();
            var propName = "NotNullNotEmpty";

            var sut = new SampleTestMementoViewModel();
            var svc = new DataAnnotationValidationService<SampleTestMementoViewModel>(sut);
            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);
            sut.ErrorsChanged += (s, e) => raised.Add(e.PropertyName);

            sut.Test_ValidateProperty(propName, ValidationBehavior.RunSilentValidation);

            Assert.IsFalse(raised.Contains(propName));
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_When_ValidateProperty_with_Trigger_berhavior_ErrorsChanged_event_should_be_raised()
        {
            List<string> raised = new List<string>();
            var propName = "NotNullNotEmpty";

            var sut = new SampleTestMementoViewModel();
            var svc = new DataAnnotationValidationService<SampleTestMementoViewModel>(sut);
            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);
            sut.ErrorsChanged += (s, e) => raised.Add(e.PropertyName);

            sut.Test_ValidateProperty(propName, ValidationBehavior.TriggerValidationErrorsOnFailure);

            Assert.IsTrue(raised.Contains(propName));
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_When_ValidateProperty_with_Trigger_berhavior_ErrorsChanged_event_should_be_raised_if_the_status_of_errors_changes()
        {
            List<string> raised = new List<string>();
            var propName = "NotNullNotEmpty";

            var sut = new SampleTestMementoViewModel();
            var svc = new DataAnnotationValidationService<SampleTestMementoViewModel>(sut);
            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);
            sut.ErrorsChanged += (s, e) => raised.Add(e.PropertyName);

            sut.Test_ValidateProperty(propName, ValidationBehavior.TriggerValidationErrorsOnFailure);

            using (svc.SuspendValidation()) //so that we can change a property without triggering the validation process
            {
                sut.NotNullNotEmpty = "qwertyqwerty";
            }

            sut.Test_ValidateProperty(propName, ValidationBehavior.TriggerValidationErrorsOnFailure);

            Assert.IsTrue(raised.Count(p => p == propName) == 2);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_When_ValidateProperty_with_Trigger_berhavior_PropertyChanged_event_should_be_raised_if_the_status_of_errors_changes()
        {
            List<string> raised = new List<string>();
            var propName = "NotNullNotEmpty";

            var sut = new SampleTestMementoViewModel();
            var svc = new DataAnnotationValidationService<SampleTestMementoViewModel>(sut);
            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);
            sut.PropertyChanged += (s, e) => raised.Add(e.PropertyName);

            sut.Test_ValidateProperty(propName, ValidationBehavior.TriggerValidationErrorsOnFailure);

            using (svc.SuspendValidation()) //so that we can change a property without triggering the validation process
            {
                sut.NotNullNotEmpty = "qwertyqwerty"; //this raises 1 PropertyChanged
            }

            sut.Test_ValidateProperty(propName, ValidationBehavior.TriggerValidationErrorsOnFailure);

            Assert.IsTrue(raised.Count(p => p == propName) == 3);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_When_ValidateProperty_and_Validation_is_suspended_Validated_event_should_not_be_raised()
        {
            bool raised = false;
            var propName = "NotNullNotEmpty";

            var sut = new SampleTestMementoViewModel();
            var svc = new DataAnnotationValidationService<SampleTestMementoViewModel>(sut);
            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);
            sut.Validated += (s, e) => raised = true;

            using (svc.SuspendValidation()) //so that we can change a property without triggering the validation process
            {
                sut.Test_ValidateProperty(propName, ValidationBehavior.TriggerValidationErrorsOnFailure);
            }

            Assert.IsFalse(raised);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation")]
        public void AbstractMementoViewModel_When_Validate_and_Validation_is_suspended_Validated_event_should_not_be_raised()
        {
            bool raised = false;

            var sut = new SampleTestMementoViewModel();
            var svc = new DataAnnotationValidationService<SampleTestMementoViewModel>(sut);
            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);
            sut.Validated += (s, e) => raised = true;

            using (svc.SuspendValidation()) //so that we can change a property without triggering the validation process
            {
                sut.Validate();
            }

            Assert.IsFalse(raised);
        }

        [TestMethod]
        [TestCategory("AbstractViewModel"), TestCategory("Validation"), TestCategory("Issue#176")]
        public void AbstractViewModel_with_custom_validation_validating_multiple_times_should_report_custom_validation_errors_only_once()
        {
            var sut = new SampleTestMementoViewModelWithValidationCallback()
            {
                NotNullNotEmpty = "something"
            };
            var svc = DataAnnotationValidationService.CreateFor(sut);

            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);
            sut.Test_OnValidate = ctx =>
            {
                if (ctx.PropertyName == "AProperty" || ctx.PropertyName == null)
                {
                    ctx.Results.AddError(new ValidationError("AProperty", null, new[] { "This is fully custom." }));
                }
            };

            sut.Validate();
            sut.Validate();
            sut.Validate();

            Assert.AreEqual(1, sut.ValidationErrors.Count);
        }

        [TestMethod]
        [TestCategory("AbstractViewModel"), TestCategory("Validation"), TestCategory("Issue#176")]
        public void AbstractViewModel_with_custom_validation_changing_properties_multiple_times_should_report_custom_validation_errors_only_once()
        {
            var sut = new SampleTestMementoViewModelWithValidationCallback()
            {
                NotNullNotEmpty = "something"
            };
            var svc = DataAnnotationValidationService.CreateFor(sut);

            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);
            sut.Test_OnValidate = ctx =>
            {
                if (ctx.PropertyName == "AProperty" || ctx.PropertyName == null)
                {
                    ctx.Results.AddError(new ValidationError("AProperty", null, new[] { "This is fully custom." }));
                }
            };

            sut.NotNullNotEmpty = "";
            sut.NotNullNotEmpty = "longer";
            sut.NotNullNotEmpty = "+longer";

            Assert.AreEqual(0, sut.ValidationErrors.Count);
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation"), TestCategory("Issue#176")]
        [Ignore]
        public void AbstractMementoViewModel_it_should_be_possible_to_change_a_validatable_property_at_custom_validation_time()
        {
            var sut = new SampleTestMementoViewModelWithValidationCallback();
            var svc = DataAnnotationValidationService.CreateFor(sut);

            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);
            sut.Test_OnValidate = ctx =>
            {
                sut.AnotherOne = "fail";
            };

            sut.NotNullNotEmpty = "a value";
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation"), TestCategory("Issue#176")]
        [Ignore]
        public void AbstractMementoViewModel_it_should_be_possible_to_change_a_validatable_property_in_a_custom_validation_rule()
        {
            var sut = new SampleTestMementoViewModel();
            var svc = DataAnnotationValidationService.CreateFor(sut);
            svc.AddRule(
                property: o => o.NotNullNotEmpty,
                rule: ctx =>
                {
                    sut.AnotherOne = "fail";

                    return ctx.Succeeded();
                });

            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);

            sut.NotNullNotEmpty = "a value";
        }

        [TestMethod]
        [TestCategory("AbstractMementoViewModel"), TestCategory("Validation"), TestCategory("Issue#177")]
        public void AbstractMementoViewModel_it_should_be_possible_to_change_a_validatable_property_in_the_validated_event()
        {
            var sut = new SampleTestMementoViewModel();
            var svc = DataAnnotationValidationService.CreateFor(sut);

            sut.ValidateUsing(svc, forceIsValidationEnabledTo: true);
            sut.Validated += (s, e) =>
            {
                sut.AnotherOne = "fail";
            };

            sut.NotNullNotEmpty = "a value";
        }
    }
}