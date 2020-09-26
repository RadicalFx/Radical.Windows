using System;

namespace Radical.Windows.ComponentModel
{
    /// <summary>
    /// Applied to a ViewModel to expose it as a static resource in the bound View.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, Inherited = false )]
    public class ExposeViewModelAsStaticResourceAttribute : Attribute
    {
    }
}