using System;

namespace Radical.Windows.ComponentModel
{
    /// <summary>
    /// Applied to properties prevents the IoC container to inject/intercept properties.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property )]
    public class IgnorePropertyInjectionAttribue : Attribute
    {
    }
}
