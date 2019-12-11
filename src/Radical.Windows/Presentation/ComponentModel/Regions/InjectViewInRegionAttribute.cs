using System;

namespace Radical.Windows.Presentation.ComponentModel.Regions
{
    /// <summary>
    /// Defines that a view want to be injected in the region identified by the region name.
    /// </summary>
    [AttributeUsage( AttributeTargets.Class )]
	public class InjectViewInRegionAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the name of the region.
		/// </summary>
		/// <value>
		/// The name of the region.
		/// </value>
		public string Named { get; set; }
	}
}
