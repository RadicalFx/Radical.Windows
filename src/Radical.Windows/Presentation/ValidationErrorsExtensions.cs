using Radical.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Radical.Windows.Presentation
{
    public static class ValidationErrorsExtensions
    {
        public static IEnumerable<string> GetInvalidProperties(this ObservableCollection<ValidationError> validationErrors)
        {
            return validationErrors.Select(ve => ve.PropertyName).Distinct();
        }

        internal static bool IsValidationStatusChanged(this ObservableCollection<ValidationError> validationErrors, IEnumerable<ValidationError> errors, string propertyName)
        {
            var beforeDetectedProblems = validationErrors
                    .Where(ve => ve.PropertyName == propertyName)
                    .SelectMany(ve => ve.DetectedProblems)
                    .OrderBy(dp => dp)
                    .ToArray();

            var afterDetectedProblems = errors
                    .SelectMany(ve => ve.DetectedProblems)
                    .OrderBy(dp => dp)
                    .ToArray();

            return !beforeDetectedProblems.SequenceEqual(afterDetectedProblems);
        }

        internal static bool IsValidationStatusChanged(this ObservableCollection<ValidationError> validationErrors, IEnumerable<ValidationError> errors)
        {
            var errorsArray = errors.ToArray();
            if (errorsArray.Length != validationErrors.Count) 
            {
                return true;
            }

            var beforeDetectedProblems = validationErrors
                    .SelectMany(ve => ve.DetectedProblems)
                    .OrderBy(dp => dp)
                    .ToArray();

            var afterDetectedProblems = errors
                    .SelectMany(ve => ve.DetectedProblems)
                    .OrderBy(dp => dp)
                    .ToArray();

            return !beforeDetectedProblems.SequenceEqual(afterDetectedProblems);
        }

        internal static void SyncValidationErrorsFrom(this ObservableCollection<ValidationError> validationErrors, IEnumerable<ValidationError> errors, string propertyName) 
        {
            for (int i = validationErrors.Count - 1; i >= 0; i--)
            {
                if (validationErrors[i].PropertyName == propertyName)
                {
                    validationErrors.RemoveAt(i);
                }
            }

            foreach (var error in errors)
            {
                validationErrors.Add(error);
            }
        }

        internal static void SyncValidationErrorsFrom(this ObservableCollection<ValidationError> validationErrors, IEnumerable<ValidationError> errors)
        {
            validationErrors.Clear();
            foreach (var error in errors)
            {
                validationErrors.Add(error);
            }
        }
    }
}
