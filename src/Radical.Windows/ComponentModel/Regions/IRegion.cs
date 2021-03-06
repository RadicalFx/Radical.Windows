﻿using System.Windows;

namespace Radical.Windows.ComponentModel
{
    /// <summary>
    /// A region identifies a pluggable area in the user interface, 
    /// where shell and modules can plug their own UI.
    /// </summary>
    public interface IRegion
    {
        /// <summary>
        /// Gets the name of the region.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the view that hosts this region.
        /// </summary>
        DependencyObject HostingView { get; }

        /// <summary>
        /// The number of milliseconds to wait before loading the region content.
        /// </summary>
        int AsyncLoadDelay { get; set; }

        /// <summary>
        /// Shutdowns this region, the shutdown process is invoked by the hosting region manager at close time.
        /// </summary>
        void Shutdown();
    }
}
