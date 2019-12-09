using System;
using System.ComponentModel;
using System.Windows;

namespace Radical.Windows.Behaviors
{
    public static class DesignTimeHelper
    {
        /// <summary>
        /// Gets a value indicating whether we are in design mode.
        /// </summary>
        /// <returns><c>True</c> if we are in design mode, otherwise <c>false</c>.</returns>
        public static bool GetIsInDesignMode()
        {
            var descriptor = DependencyPropertyDescriptor.FromProperty(
                DesignerProperties.IsInDesignModeProperty,
                typeof(DependencyObject));

            var isInDesignMode = (bool)descriptor.Metadata.DefaultValue;

            return isInDesignMode;
        }
    }
}
