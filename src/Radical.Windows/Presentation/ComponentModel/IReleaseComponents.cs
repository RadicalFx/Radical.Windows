using System;

namespace Radical.Windows.Presentation.ComponentModel
{
    /// <summary>
    /// Manage the release process of a component.
    /// </summary>
    public interface IReleaseComponents
	{
		/// <summary>
		/// Releases the given component.
		/// </summary>
		/// <param name="component">The component to release.</param>
		void Release(object component );
	}


    /// <summary>
    /// Signals that a DI container exposed implementing the <see cref="IReleaseComponents"/> 
    /// interface is capable of disposing components at release time, such as Castle Windsor
    /// </summary>
    /// <seealso cref="System.Attribute" />
    public sealed class SupportComponentDisposeAttribute : Attribute
    {

    }
}
