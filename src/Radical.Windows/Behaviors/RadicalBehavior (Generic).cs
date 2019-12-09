using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;

namespace Radical.Windows.Behaviors
{
    public interface INotifyAttachedOjectLoaded
    {
        event EventHandler AttachedObjectLoaded;
        T GetAttachedObject<T>() where T : FrameworkElement;
    }

    public abstract class RadicalBehavior<T> : Behavior<T>,
        INotifyAttachedOjectLoaded, IWeakEventListener
        where T : FrameworkElement
    {
        RoutedEventHandler loaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadicalBehavior&lt;T&gt;"/> class.
        /// </summary>
        public RadicalBehavior()
        {
            loaded = (s, e) => OnAttachedObjectLoaded();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            LoadedWeakEventManager.AddListener(AssociatedObject, this);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            LoadedWeakEventManager.RemoveListener(AssociatedObject, this);
        }

        public event EventHandler AttachedObjectLoaded;

        protected virtual void OnAttachedObjectLoaded()
        {
            if (AttachedObjectLoaded != null)
            {
                AttachedObjectLoaded(this, EventArgs.Empty);
            }
        }

        public T GetAttachedObject<T>() where T : FrameworkElement
        {
            return AssociatedObject as T;
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            return OnReceiveWeakEvent(managerType, sender, e);
        }

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