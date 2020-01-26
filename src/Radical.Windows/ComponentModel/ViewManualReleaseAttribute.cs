using System;

namespace Radical.Windows.ComponentModel
{
    /// <summary>
    /// When attached to a View prevents the View and the ViewModel associated to be automatically disposed.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class )]
    public class ViewManualReleaseAttribute : Attribute
    {
    }
}
