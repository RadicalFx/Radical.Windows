using System;

namespace Radical.Windows.Bootstrap
{
    /// <summary>
    /// Instructs the automatic registration process to ignore
    /// a type marked with this attribute.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class )]
    public class DisableAutomaticRegistrationAttribute : Attribute
    {
    }
}
