using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// A behavior that executes a command when a column header of a <see cref="ListView"/> is clicked,
    /// passing the binding path of the column, or its <c>GridViewColumnManager.SortProperty</c>, as parameter.
    /// </summary>
    public class ListViewColumnHeaderCommandBehavior :
        RadicalBehavior<ListView>,
        ICommandSource
    {
        readonly RoutedEventHandler onLoaded;
        readonly RoutedEventHandler onColumnHeaderClick;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewColumnHeaderCommandBehavior"/> class.
        /// </summary>
        public ListViewColumnHeaderCommandBehavior()
        {
            onColumnHeaderClick = (s, e) =>
            {
                if (e.OriginalSource is GridViewColumnHeader clickedHeader && clickedHeader.Role != GridViewColumnHeaderRole.Padding)
                {
                    var column = clickedHeader.Column;
                    string commandParam = null;

                    if (column.DisplayMemberBinding is Binding)
                    {
                        commandParam = ((Binding)column.DisplayMemberBinding).Path.Path;
                    }
                    else
                    {
                        commandParam = GridViewColumnManager.GetSortProperty(column);
                    }

                    if (!string.IsNullOrEmpty(commandParam) && Command != null && Command.CanExecute(commandParam))
                    {
                        Command.Execute(commandParam);
                    }
                }
            };

            onLoaded = (s, e) =>
            {
                AssociatedObject.AddHandler(
                    System.Windows.Controls.Primitives.ButtonBase.ClickEvent,
                    onColumnHeaderClick);
            };
        }

        #region Dependency Property: Command

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(ListViewColumnHeaderCommandBehavior),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command executed when a column header is clicked.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #endregion

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += onLoaded;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Loaded -= onLoaded;
            AssociatedObject.RemoveHandler(
                    System.Windows.Controls.Primitives.ButtonBase.ClickEvent,
                    onColumnHeaderClick);
        }

        /// <summary>
        /// Gets or sets the command parameter. Not used: the command parameter is the column property name.
        /// </summary>
        public object CommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// The object that the command is being executed on.
        /// </summary>
        /// <value>The associated list view.</value>
        public IInputElement CommandTarget
        {
            get { return AssociatedObject as IInputElement; }
        }
    }
}
