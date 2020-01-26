using System;

namespace Radical.Windows.ComponentModel
{
    /// <summary>
    /// Applied to a ViewModel issues automatically a ViewModelShown message.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
    public class NotifyShownAttribute : Attribute
    {
    }
}
