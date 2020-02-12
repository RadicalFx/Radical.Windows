using System;

namespace Radical.Windows.ComponentModel
{
    /// <summary>
    /// Applied to a ViewModel issues automatically a ViewModelClosed message.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
    public class NotifyClosedAttribute : Attribute
    {
    }
}
