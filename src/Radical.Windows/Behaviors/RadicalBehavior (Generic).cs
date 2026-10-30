using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// Notifies when the object a behavior is attached to is loaded.
    /// </summary>
    public interface INotifyAttachedOjectLoaded
    {
        /// <summary>
        /// Occurs when the attached object is loaded.
        /// </summary>
        event EventHandler AttachedObjectLoaded;
        /// <summary>
        /// Gets the attached object.
        /// </summary>
        /// <typeparam name="T">The expected type of the attached object.</typeparam>
        /// <returns>The attached object, or <c>null</c> if it is not of the requested type.</returns>
        T GetAttachedObject<T>() where T : FrameworkElement;
    }

    /// <summary>
    /// Base class for Radical behaviors, notifies when the associated object is loaded.
    /// </summary>
    /// <typeparam name="T">The type of the element the behavior can be attached to.</typeparam>
    public abstract class RadicalBehavior<T> : Behavior<T>,
        INotifyAttachedOjectLoaded, IWeakEventListener
        where T : FrameworkElement
    {
        readonly RoutedEventHandler loaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadicalBehavior&lt;T&gt;"/> class.
        /// </summary>
        public RadicalBehavior()
        {
            loaded = (s, e) => OnAttachedObjectLoaded();
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            LoadedWeakEventManager.AddListener(AssociatedObject, this);
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            LoadedWeakEventManager.RemoveListener(AssociatedObject, this);
        }

        /// <inheritdoc/>
        public event EventHandler AttachedObjectLoaded;

        /// <summary>
        /// Raises the <see cref="AttachedObjectLoaded"/> event.
        /// </summary>
        protected virtual void OnAttachedObjectLoaded()
        {
            if (AttachedObjectLoaded != null)
            {
                AttachedObjectLoaded(this, EventArgs.Empty);
            }
        }

        /// <inheritdoc/>
        public T GetAttachedObject<T>() where T : FrameworkElement
        {
            return AssociatedObject as T;
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return OnReceiveWeakEvent(managerType, sender, e);
        }

        /// <summary>
        /// Called when a weak event is received.
        /// </summary>
        /// <param name="managerType">The type of the weak event manager.</param>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        /// <returns><c>true</c> if the event was handled; otherwise, <c>false</c>.</returns>
        protected virtual bool OnReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(LoadedWeakEventManager))
            {
                OnAttachedObjectLoaded();
            }
            else
            {
                // unrecognized event
                return false;
            }

            return true;
        }
    }
}