using System.Windows;

namespace Radical.Windows
{
    public sealed class LoadedWeakEventManager : WeakEventManager
    {
        static LoadedWeakEventManager GetCurrentManager()
        {
            var mt = typeof(LoadedWeakEventManager);

            var manager = (LoadedWeakEventManager)GetCurrentManager(mt);
            if (manager == null)
            {
                manager = new LoadedWeakEventManager();
                SetCurrentManager(mt, manager);
            }

            return manager;
        }

        public static void AddListener(FrameworkElement source, IWeakEventListener listener)
        {

            GetCurrentManager()
                .ProtectedAddListener(source, listener);
        }

        public static void RemoveListener(FrameworkElement source, IWeakEventListener listener)
        {

            GetCurrentManager()
                .ProtectedRemoveListener(source, listener);
        }

        private LoadedWeakEventManager()
        {

        }

        void OnLoaded(object sender, RoutedEventArgs args)
        {
            DeliverEvent(sender, args);
        }

        /// <summary>
        /// When overridden in a derived class, starts listening for the event being managed. After <see cref="M:System.Windows.WeakEventManager.StartListening(System.Object)"/>  is first called, the manager should be in the state of calling <see cref="M:System.Windows.WeakEventManager.DeliverEvent(System.Object,System.EventArgs)"/> or <see cref="M:System.Windows.WeakEventManager.DeliverEventToList(System.Object,System.EventArgs,System.Windows.WeakEventManager.ListenerList)"/> whenever the relevant event from the provided source is handled.
        /// </summary>
        /// <param name="source">The source to begin listening on.</param>
        protected override void StartListening(object source)
        {
            if (source is FrameworkElement trigger)
            {
                trigger.Loaded += OnLoaded;
            }
        }

        /// <summary>
        /// When overridden in a derived class, stops listening on the provided source for the event being managed.
        /// </summary>
        /// <param name="source">The source to stop listening on.</param>
        protected override void StopListening(object source)
        {
            if (source is FrameworkElement trigger)
            {
                trigger.Loaded -= OnLoaded;
            }
        }
    }
}
