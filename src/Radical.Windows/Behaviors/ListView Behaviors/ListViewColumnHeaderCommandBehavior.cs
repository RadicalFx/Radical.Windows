using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Radical.Windows.Behaviors
{
    public class ListViewColumnHeaderCommandBehavior :
        RadicalBehavior<ListView>,
        ICommandSource
    {
        readonly RoutedEventHandler onLoaded;
        readonly RoutedEventHandler onColumnHeaderClick;

        public ListViewColumnHeaderCommandBehavior()
        {
            onColumnHeaderClick = (s, e) =>
            {
                var clickedHeader = e.OriginalSource as GridViewColumnHeader;
                if (clickedHeader != null && clickedHeader.Role != GridViewColumnHeaderRole.Padding)
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
                    GridViewColumnHeader.ClickEvent,
                    onColumnHeaderClick);
            };
        }

        #region Dependency Property: Command

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(ListViewColumnHeaderCommandBehavior),
            new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += onLoaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.Loaded -= onLoaded;
            AssociatedObject.RemoveHandler(
                    GridViewColumnHeader.ClickEvent,
                    onColumnHeaderClick);
        }

        public object CommandParameter
        {
            get;
            set;
        }

        /// <summary>
        /// The object that the command is being executed on.
        /// </summary>
        /// <value></value>
        public IInputElement CommandTarget
        {
            get { return AssociatedObject as IInputElement; }
        }
    }
}
