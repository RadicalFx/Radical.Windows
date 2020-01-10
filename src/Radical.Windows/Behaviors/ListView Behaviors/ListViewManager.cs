using Radical.Conversions;
//using log4net;
using Radical.Diagnostics;
using Radical.Linq;
using Radical.Validation;
using Radical.Windows.Input;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Radical.Windows.Behaviors
{
    public static class ListViewManager
    {
        static readonly TraceSource logger = new TraceSource(typeof(ListViewManager).FullName);
        //static readonly ILog logger = LogManager.GetLogger( typeof( ListViewManager ) );

        static readonly RoutedEventHandler onLoaded = (s, e) =>
        {
            var lv = (ListView)s;

            //lv.GotFocus += onListViewGotFocus;
            lv.SizeChanged += onSizeChanged;
            lv.Unloaded += onUnloaded;

            var autoSortColumns = GetSortCommand(lv);
            if (autoSortColumns != null)
            {
                lv.AddHandler(ButtonBase.ClickEvent, onColumnHeaderClick);
            }

            AdjustColumns(lv);
        };

        //static readonly RoutedEventHandler onListViewGotFocus = ( s, e ) =>
        //{
        //    //if( !( e.OriginalSource is ListViewItem ) && ListViewManagerHelper.IsInsideListViewItem( e.OriginalSource as DependencyObject ) )
        //    //{
        //    //    var lv = ( ListView )s;
        //    //    var lvi = ListViewManagerHelper.FindListViewItem( e.OriginalSource as DependencyObject );

        //    //    //var dataContext = ( e.OriginalSource is FrameworkElement )
        //    //    //                    ? ( ( FrameworkElement )e.OriginalSource ).DataContext
        //    //    //                    : null;

        //    //    //var command = GetItemDoubleClickCommand( ( DependencyObject )s );

        //    //    //if( command != null && command.CanExecute( dataContext ) )
        //    //    //{
        //    //    //    command.Execute( dataContext );
        //    //    //}
        //    //}
        //};

        static readonly MouseButtonEventHandler onListViewDblClick = (s, e) =>
        {
            if (VisualTreeCrawler.IsChildOfType<ListViewItem>(e.OriginalSource as DependencyObject))
            {
                var dataContext = (e.OriginalSource is FrameworkElement)
                                    ? ((FrameworkElement)e.OriginalSource).DataContext
                                    : null;

                var command = GetItemDoubleClickCommand((DependencyObject)s);

                if (command != null && command.CanExecute(dataContext))
                {
                    command.Execute(dataContext);
                }
            }
        };

        static readonly KeyEventHandler onKeyDown = (s, e) =>
        {
            if (VisualTreeCrawler.IsChildOfType<ListViewItem>(e.OriginalSource as DependencyObject))
            {
                var dataContext = (e.OriginalSource is FrameworkElement)
                                    ? ((FrameworkElement)e.OriginalSource).DataContext
                                    : null;

                var itemSelectCommand = GetItemDoubleClickCommand((DependencyObject)s);
                if (itemSelectCommand != null && itemSelectCommand.CanExecute(dataContext))
                {
                    var gestures = itemSelectCommand.GetGestures();
                    if (((gestures.None() && e.Key == Key.Enter) || gestures.Where(gesture => gesture.Matches(s, e)).Any()))
                    {
                        itemSelectCommand.Execute(dataContext);
                        e.Handled = true;
                    }
                }

                var itemRemoveCommand = GetItemRemoveCommand((DependencyObject)s);
                if (itemRemoveCommand != null && itemRemoveCommand.CanExecute(dataContext))
                {
                    var gestures = itemRemoveCommand.GetGestures();
                    if (((gestures.None() && e.Key == Key.Delete) || gestures.Where(gesture => gesture.Matches(s, e)).Any()))
                    {
                        itemRemoveCommand.Execute(dataContext);
                        e.Handled = true;
                    }
                }
            }
        };

        static readonly SizeChangedEventHandler onSizeChanged = (s, e) =>
        {
            if (e.WidthChanged)
            {
                AdjustColumns((ListView)s);
            }
        };

        static readonly RoutedEventHandler onUnloaded = (s, e) =>
        {
            var lv = (ListView)s;

            lv.Unloaded -= onUnloaded;
            lv.SizeChanged -= onSizeChanged;
            //lv.GotFocus -= onListViewGotFocus;

            var autoSortColumns = GetSortCommand(lv);
            if (autoSortColumns != null)
            {
                lv.RemoveHandler(ButtonBase.ClickEvent, onColumnHeaderClick);
            }

            //Vedi CueBannerService per i dettagli
            //lv.Loaded -= onLoaded;
        };

        static readonly RoutedEventHandler onColumnHeaderClick = (s, e) =>
        {
            if (e.OriginalSource is GridViewColumnHeader clickedHeader && clickedHeader.Role != GridViewColumnHeaderRole.Padding)
            {
                var listView = VisualTreeCrawler.FindParent<ListView>(clickedHeader);
                var column = clickedHeader.Column;

                var commandParam = GridViewColumnManager.GetSortProperty(column);
                var command = GetSortCommand(listView);

                if (!string.IsNullOrEmpty(commandParam) && command != null && command.CanExecute(commandParam))
                {
                    command.Execute(commandParam);
                }
            }
        };

        #region Attached Property: IsLoadEventAttached

        static readonly DependencyProperty IsLoadEventAttachedProperty = DependencyProperty.RegisterAttached(
                                      "IsLoadEventAttached",
                                      typeof(bool),
                                      typeof(ListViewManager),
                                      new FrameworkPropertyMetadata(false));


        static bool GetIsLoadEventAttached(ListView owner)
        {
            return (bool)owner.GetValue(IsLoadEventAttachedProperty);
        }

        static void SetIsLoadEventAttached(ListView owner, bool value)
        {
            owner.SetValue(IsLoadEventAttachedProperty, value);
        }

        #endregion

        #region Attached Property: ItemDoubleClickCommand

        public static DependencyProperty ItemDoubleClickCommandProperty = DependencyProperty.RegisterAttached(
            "ItemDoubleClickCommand",
            typeof(ICommand),
            typeof(ListViewManager),
            new UIPropertyMetadata(OnItemDoubleClickCommandChanged));


        public static void SetItemDoubleClickCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(ItemDoubleClickCommandProperty, value);
        }

        static ICommand GetItemDoubleClickCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(ItemDoubleClickCommandProperty);
        }

        #endregion

        static void OnItemDoubleClickCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignTimeHelper.GetIsInDesignMode())
            {
                Ensure.That(target.GetType())
                    .WithMessage("This behavior can be attached to ListView(s) only.")
                    .Is<ListView>();

                var lv = (ListView)target;
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    lv.MouseDoubleClick += onListViewDblClick;
                    lv.KeyDown += onKeyDown;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    lv.MouseDoubleClick -= onListViewDblClick;
                    lv.KeyDown -= onKeyDown;
                }
            }
        }

        #region Attached Property: ItemRemoveCommand

        public static readonly DependencyProperty ItemRemoveCommandProperty = DependencyProperty.RegisterAttached(
                                      "ItemRemoveCommand",
                                      typeof(ICommand),
                                      typeof(ListViewManager),
                                      new FrameworkPropertyMetadata(null, OnItemRemoveCommandChanged));


        public static ICommand GetItemRemoveCommand(DependencyObject owner)
        {
            return (ICommand)owner.GetValue(ItemRemoveCommandProperty);
        }

        public static void SetItemRemoveCommand(DependencyObject owner, ICommand value)
        {
            owner.SetValue(ItemRemoveCommandProperty, value);
        }

        #endregion

        private static void OnItemRemoveCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignTimeHelper.GetIsInDesignMode())
            {
                Ensure.That(target.GetType())
                    .WithMessage("This behavior can be attached to ListView(s) only.")
                    .Is<ListView>();

                var lv = (ListView)target;
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    lv.KeyDown += onKeyDown;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    lv.KeyDown -= onKeyDown;
                }
            }
        }

        #region Attached Property: SelectedItems

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.RegisterAttached(
                                      "SelectedItems",
                                      typeof(IList),
                                      typeof(ListViewManager),
                                      new FrameworkPropertyMetadata(null, OnSelectedItemsChanged));


        public static IList GetSelectedItems(ListView owner)
        {
            return (IList)owner.GetValue(SelectedItemsProperty);
        }

        public static void SetSelectedItems(ListView owner, IList value)
        {
            owner.SetValue(SelectedItemsProperty, value);
        }

        #endregion

        #region Attached Property: selectionHandler

        private static readonly DependencyProperty selectionHandlerProperty = DependencyProperty.RegisterAttached(
                              "selectionHandler",
                              typeof(SelectionHandler),
                              typeof(ListViewManager),
                              new FrameworkPropertyMetadata(null));

        #endregion

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignTimeHelper.GetIsInDesignMode())
            {
                if (e.OldValue != null)
                {
                    //Probabilmente c'è un handler da sganciare
                    if (d.GetValue(selectionHandlerProperty) is SelectionHandler handler)
                    {
                        handler.StopSync();
                        d.ClearValue(selectionHandlerProperty);
                    }
                }

                if (e.NewValue != null)
                {
                    //C'è un handler da agganciare
                    var handler = new SelectionHandler();
                    handler.SartSync(d.CastTo<ListView>(), e.NewValue.CastTo<IList>());

                    d.SetValue(selectionHandlerProperty, handler);
                }
            }
        }

        #region Attached Property: AutoSizeColumns

        public static readonly DependencyProperty AutoSizeColumnsProperty = DependencyProperty.RegisterAttached(
                                      "AutoSizeColumns",
                                      typeof(bool),
                                      typeof(ListViewManager),
                                      new FrameworkPropertyMetadata(false, OnAutoSizeColumnsChanged));


        public static bool GetAutoSizeColumns(ListView owner)
        {
            return (bool)owner.GetValue(AutoSizeColumnsProperty);
        }

        public static void SetAutoSizeColumns(ListView owner, bool value)
        {
            owner.SetValue(AutoSizeColumnsProperty, value);
        }

        #endregion

        private static void OnAutoSizeColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignTimeHelper.GetIsInDesignMode())
            {
                var listView = Ensure.That(d as ListView)
                   .Named("d")
                   .WithMessage("This behavior can be attached only to ListView(s).")
                   .IsNotNull()
                   .GetValue();

                if (!GetIsLoadEventAttached(listView))
                {
                    listView.Loaded += onLoaded;
                    SetIsLoadEventAttached(listView, true);
                }
            }
        }

        static void AdjustColumns(ListView listView)
        {
            if (GetAutoSizeColumns(listView))
            {
                var gv = Ensure.That(listView.View as GridView)
                    .WithMessage("This behavior can be attached only to ListView(s) whose view is a GridView.")
                    .IsNotNull()
                    .GetValue();

                var columnsToStretch = gv.Columns.Where(c => GridViewColumnManager.GetFill(c));
                if (columnsToStretch.Any())
                {
                    var subtract = .0d;
                    var sv = VisualTreeCrawler.FindChild<ScrollViewer>(listView);
                    if (sv != null && sv.ComputedVerticalScrollBarVisibility == Visibility.Visible)
                    {
                        var sb = VisualTreeCrawler.FindChild<ScrollBar>(sv, child => child.Orientation == Orientation.Vertical);
                        if (sb != null)
                        {
                            subtract = sb.Width;
                        }
                    }

                    var nonStretchedColumnsActualWidth = gv.Columns
                        .Where(c => !GridViewColumnManager.GetFill(c))
                        .Aggregate(0d, (w, c) =>
                       {

                           w += c.ActualWidth;
                           return w;
                       });

                    var availableWidth = listView.ActualWidth - subtract - nonStretchedColumnsActualWidth;
                    if (availableWidth <= 0)
                    {
                        logger.Warning
                        (
                            "Columns cannot be auto sized, available size is invalid: {0}",
                            availableWidth
                        );
                    }

                    if (availableWidth > 0)
                    {
                        var columnsAvarageWidth = availableWidth / columnsToStretch.Count();
                        foreach (var column in columnsToStretch)
                        {
                            column.Width = columnsAvarageWidth;
                        }
                    }
                }
            }
        }

        #region Attached Property: SortCommand

        public static readonly DependencyProperty SortCommandProperty = DependencyProperty.RegisterAttached(
                                      "SortCommand",
                                      typeof(ICommand),
                                      typeof(ListViewManager),
                                      new FrameworkPropertyMetadata(null, OnSortCommandChanged));


        public static ICommand GetSortCommand(ListView owner)
        {
            return (ICommand)owner.GetValue(SortCommandProperty);
        }

        public static void SetSortCommand(ListView owner, ICommand value)
        {
            owner.SetValue(SortCommandProperty, value);
        }

        #endregion

        private static void OnSortCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignTimeHelper.GetIsInDesignMode())
            {
                var listView = Ensure.That(d as ListView)
                    .Named("d")
                    .WithMessage("This behavior can be attached only to ListView(s).")
                    .IsNotNull()
                    .GetValue();

                if (!GetIsLoadEventAttached(listView))
                {
                    listView.Loaded += onLoaded;
                    SetIsLoadEventAttached(listView, true);
                }
            }
        }

        //#region Attached Property: AutoSelectItemOnChildFocus

        //public static readonly DependencyProperty AutoSelectItemOnChildFocusProperty = DependencyProperty.RegisterAttached(
        //                              "AutoSelectItemOnChildFocus",
        //                              typeof( Boolean ),
        //                              typeof( ListViewManager ),
        //                              new FrameworkPropertyMetadata( false, OnAutoSelectItemOnChildFocusChanged ) );


        //public static Boolean GetAutoSelectItemOnChildFocus( DependencyObject owner )
        //{
        //    return ( Boolean )owner.GetValue( AutoSelectItemOnChildFocusProperty );
        //}

        //public static void SetAutoSelectItemOnChildFocus( DependencyObject owner, Boolean value )
        //{
        //    owner.SetValue( AutoSelectItemOnChildFocusProperty, value );
        //}

        //#endregion

        //private static void OnAutoSelectItemOnChildFocusChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        //{
        //    if( !DesignTimeHelper.GetIsInDesignMode() )
        //    {
        //        var listView = Ensure.That( d as ListView )
        //            .Named( "d" )
        //            .WithMessage( "This behavior can be attached only to ListView(s)." )
        //            .IsNotNull()
        //            .GetValue();

        //        if( !ListViewManager.GetIsLoadEventAttached( listView ) )
        //        {
        //            listView.Loaded += onLoaded;
        //            ListViewManager.SetIsLoadEventAttached( listView, true );
        //        }
        //    }
        //}
    }
}


