﻿using Radical.Windows.ComponentModel;
using System;
using System.Windows;

namespace Radical.Windows.Regions
{
    /// <summary>
    /// A base abstract implementation for the <see cref="ISwitchingElementsRegion"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SwitchingElementsRegion<T> :
        ElementsRegion<T>,
        ISwitchingElementsRegion
        where T : FrameworkElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchingElementsRegion&lt;T&gt;"/> class.
        /// </summary>
        protected SwitchingElementsRegion()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchingElementsRegion&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected SwitchingElementsRegion(string name )
            : base( name )
        {

        }

        /// <summary>
        /// Gets the content of the active.
        /// </summary>
        /// <value>
        /// The content of the active.
        /// </value>
        public abstract DependencyObject ActiveContent { get; }

        /// <summary>
        /// Occurs when the active content changes.
        /// </summary>
        public event EventHandler<ActiveContentChangedEventArgs> ActiveContentChanged;

        /// <summary>
        /// Called when the active content is changed.
        /// </summary>
        protected virtual void OnActiveContentChanged()
        {
            var h = ActiveContentChanged;
            if( h != null )
            {
                h( this, new ActiveContentChangedEventArgs( ActiveContent, PreviousActiveContent ) );
            }

            if (TryGetViewModel(ActiveContent) is IExpectViewActivatedCallback vm)
            {
                vm.OnViewActivated();
            }

            if ( ActiveContent != PreviousActiveContent )
            {
                PreviousActiveContent = ActiveContent;
            }
        }

        /// <summary>
        /// Tries the get view model.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        protected virtual object TryGetViewModel( DependencyObject view )
        {
            if( RegionService.Conventions != null )
            {
                var vm = RegionService.Conventions.GetViewDataContext( view, ViewDataContextSearchBehavior.LocalOnly );

                return vm;
            }

            return null;
        }

        /// <summary>
        /// Gets the content of the previous active.
        /// </summary>
        /// <value>
        /// The content of the previous active.
        /// </value>
        public DependencyObject PreviousActiveContent
        {
            get;
            private set;
        }

        /// <summary>
        /// Activates the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        public abstract void Activate( DependencyObject content );
    }
}