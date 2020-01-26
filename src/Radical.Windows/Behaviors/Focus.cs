using System.Windows;
using System.Windows.Input;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// 
    /// </summary>
    public class Focus : RadicalBehavior<FrameworkElement>
    {
        #region Dependency Property: ControlledBy

        /// <summary>
        /// The controlled by property
        /// </summary>
        public static readonly DependencyProperty ControlledByProperty = DependencyProperty.Register(
            "ControlledBy",
            typeof(string),
            typeof( Focus ),
            new FrameworkPropertyMetadata( null, new PropertyChangedCallback( OnControlledByChanged ) ) { BindsTwoWayByDefault = true } );

        /// <summary>
        /// Gets or sets the controlled by.
        /// </summary>
        /// <value>
        /// The controlled by.
        /// </value>
        public string ControlledBy
        {
            get { return (string)GetValue( ControlledByProperty ); }
            set { SetValue( ControlledByProperty, value ); }
        }

        static void OnControlledByChanged( DependencyObject sender, DependencyPropertyChangedEventArgs e )
        {
            ( ( Focus )sender ).OnFocusChanged( (string)e.NewValue );
        }

        private void OnFocusChanged(string focusKey )
        {
            if (string.Equals( UsingKey, focusKey ) && AssociatedObject.Focusable && !AssociatedObject.IsFocused )
            {
                AssociatedObject.Focus();
                Keyboard.Focus( AssociatedObject );
            }
        }

        #endregion

        /// <summary>
        /// Called when [attached].
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.GotFocus += OnGotFocus;
            AssociatedObject.LostFocus += OnLostFocus;
        }

        /// <summary>
        /// Called when [detaching].
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.GotFocus -= OnGotFocus;
            AssociatedObject.LostFocus -= OnLostFocus;

            base.OnDetaching();
        }

        void OnGotFocus( object sender, RoutedEventArgs e )
        {
            ControlledBy = UsingKey;
        }

        void OnLostFocus( object sender, RoutedEventArgs e )
        {
            if ( ControlledBy == UsingKey ) 
            {
                /*
                 * if, when we loose focus, the FocusedElementKey is still
                 * pointing to us means that the user has moved the focus 
                 * to an element not managed by a behvior like this one, thus
                 * we set the FocusedElementKey to null so to detach and 
                 * correctly react if the focus gets back.
                 */
                ControlledBy = null;
            }
        }

        /// <summary>
        /// Gets or sets the using key.
        /// </summary>
        /// <value>
        /// The using key.
        /// </value>
        public string UsingKey { get; set; }
    }
}
