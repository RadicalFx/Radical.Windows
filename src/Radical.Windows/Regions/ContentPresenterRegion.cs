﻿using Radical.Conversions;
using Radical.Windows.ComponentModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Radical.Windows.Regions
{
    /// <summary>
    /// An content region hosted by a ContentPresenter.
    /// </summary>
    public sealed class ContentPresenterRegion : ContentRegion<ContentPresenter>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPresenterRegion"/> class.
        /// </summary>
        public ContentPresenterRegion()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPresenterRegion"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ContentPresenterRegion(string name )
            : base( name )
        {

        }

        /// <summary>
        /// Called to get content.
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject OnGetContent()
        {
            return Element.Content as DependencyObject;
        }

        /// <summary>
        /// Called to set new content.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="args">The cancel even arguments.</param>
        protected override void OnSetContent( DependencyObject view, CancelEventArgs args )
        {
            if (Element.Content is DependencyObject previous)
            {
                RegionService.Conventions
                    .GetViewDataContext(previous, RegionService.Conventions.DefaultViewDataContextSearchBehavior)
                    .As<IExpectViewClosingCallback>(i => i.OnViewClosing(args));
            }

            if ( !args.Cancel )
            {
                Element.Content = view;
            }
        }
    }
}
