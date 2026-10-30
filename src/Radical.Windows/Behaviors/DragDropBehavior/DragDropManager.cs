using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// The DragDropManager class exposing all the drag 'n' drop related attached properties.
    /// </summary>
    public static class DragDropManager
    {
        #region Attached Property: IsDragSourceAttached

        /// <summary>
        /// Identifies the IsDragSourceAttached attached property, used internally to track
        /// the elements whose drag source event handlers are attached.
        /// </summary>
        public static readonly DependencyProperty IsDragSourceAttachedProperty = DependencyProperty.RegisterAttached(
                                      "IsDragSourceAttached",
                                      typeof(bool),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(false));


        static bool GetIsDragSourceAttached(DependencyObject owner)
        {
            return (bool)owner.GetValue(IsDragSourceAttachedProperty);
        }

        static void SetIsDragSourceAttached(DependencyObject owner, bool value)
        {
            owner.SetValue(IsDragSourceAttachedProperty, value);
        }

        #endregion

        #region Attached Property: IsDropTargetAttached

        /// <summary>
        /// Identifies the IsDropTargetAttached attached property, used internally to track
        /// the elements whose drop target event handlers are attached.
        /// </summary>
        public static readonly DependencyProperty IsDropTargetAttachedProperty = DependencyProperty.RegisterAttached(
                                      "IsDropTargetAttached",
                                      typeof(bool),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(false));


        static bool GetIsDropTargetAttached(DependencyObject owner)
        {
            return (bool)owner.GetValue(IsDropTargetAttachedProperty);
        }

        static void SetIsDropTargetAttached(DependencyObject owner, bool value)
        {
            owner.SetValue(IsDropTargetAttachedProperty, value);
        }

        #endregion

        #region Attached Property: DropTarget

        /// <summary>
        /// Identifies the DropTarget attached property: the object passed to the drop
        /// commands as the target of the drag and drop operation.
        /// </summary>
        public static readonly DependencyProperty DropTargetProperty = DependencyProperty.RegisterAttached(
                                      "DropTarget",
                                      typeof(object),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Gets the drop target of the given object.
        /// </summary>
        /// <param name="owner">The object.</param>
        /// <returns>The drop target.</returns>
        public static object GetDropTarget(DependencyObject owner)
        {
            return (object)owner.GetValue(DropTargetProperty);
        }

        /// <summary>
        /// Sets the drop target of the given object.
        /// </summary>
        /// <param name="owner">The object.</param>
        /// <param name="value">The drop target.</param>
        public static void SetDropTarget(DependencyObject owner, object value)
        {
            owner.SetValue(DropTargetProperty, value);
        }

        #endregion

        #region Attached Property: DataObjectType

        /// <summary>
        /// Identifies the DataObjectType attached property: the data format used
        /// to drag the data object. When not set the data object type is used.
        /// </summary>
        public static readonly DependencyProperty DataObjectTypeProperty = DependencyProperty.RegisterAttached(
                                      "DataObjectType",
                                      typeof(string),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Gets the data object type of the given object.
        /// </summary>
        /// <param name="owner">The object.</param>
        /// <returns>The data object type.</returns>
        public static string GetDataObjectType(DependencyObject owner)
        {
            return (string)owner.GetValue(DataObjectTypeProperty);
        }

        /// <summary>
        /// Sets the data object type of the given object.
        /// </summary>
        /// <param name="owner">The object.</param>
        /// <param name="value">The data object type.</param>
        public static void SetDataObjectType(DependencyObject owner, string value)
        {
            owner.SetValue(DataObjectTypeProperty, value);
        }

        #endregion

        #region Attached Property: DataObject

        /// <summary>
        /// Identifies the DataObject attached property: the object dragged when a drag
        /// operation starts from the element. Setting it makes the element a drag source.
        /// </summary>
        public static readonly DependencyProperty DataObjectProperty = DependencyProperty.RegisterAttached(
                                      "DataObject",
                                      typeof(object),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(null, OnDataObjectChanged));


        /// <summary>
        /// Gets the data object of the given object.
        /// </summary>
        /// <param name="owner">The object.</param>
        /// <returns>The data object.</returns>
        public static object GetDataObject(DependencyObject owner)
        {
            return (object)owner.GetValue(DataObjectProperty);
        }

        /// <summary>
        /// Sets the data object of the given object.
        /// </summary>
        /// <param name="owner">The object.</param>
        /// <param name="value">The data object.</param>
        public static void SetDataObject(DependencyObject owner, object value)
        {
            owner.SetValue(DataObjectProperty, value);
        }

        #endregion

        private static void OnDataObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var attached = GetIsDragSourceAttached(d);
            if (!attached)
            {
                SetIsDragSourceAttached(d, true);

                ((UIElement)d).PreviewMouseLeftButtonDown += (s, args) =>
                {
                    var _startPoint = args.GetPosition(null);
                    SetStartPoint(d, _startPoint);
                };

                /*
                 * We cannot use "MouseLeftButtonDown" because on the
                 * TreeView control is never fired, maybe is handled by
                 * someone else in the processing pipeline.
                 * 
                 * ((UIElement)d).MouseLeftButtonDown += (s, args) =>{ ... };
                 */

                ((UIElement)d).MouseMove += (s, args) =>
            {
                var isDragging = GetIsDragging(d);
                if (args.LeftButton == MouseButtonState.Pressed && !isDragging)
                {
                    var position = args.GetPosition(null);
                    var _startPoint = GetStartPoint(d);

                    if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        StartDrag(d, args);
                    }
                }
            };
            }
        }
        static void StartDrag(DependencyObject d, MouseEventArgs e)
        {
            var sourceItem = FindDragContainer((DependencyObject)e.OriginalSource);

            SetIsDragging(d, true);

            var obj = GetDataObject(sourceItem);
            var objType = GetDataObjectType(sourceItem);

            DataObject data;
            if (string.IsNullOrEmpty(objType))
            {
                data = new DataObject(obj.GetType(), obj);
            }
            else
            {
                data = new DataObject(objType, obj);
            }

            DragDrop.DoDragDrop(d, data, DragDropEffects.Move);

            SetIsDragging(d, false);
        }

        #region Attached Property: IsDragging

        /// <summary>
        /// Identifies the read-only IsDragging attached property: <c>true</c> while a drag
        /// operation started from the element is in progress.
        /// </summary>
        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.RegisterAttached(
                                      "IsDragging",
                                      typeof(bool),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(false));


        /// <summary>
        /// Gets the is dragging.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns><c>true</c> if a drag operation started from the element is in progress; otherwise, <c>false</c>.</returns>
        public static bool GetIsDragging(DependencyObject owner)
        {
            return (bool)owner.GetValue(IsDraggingProperty);
        }

        static void SetIsDragging(DependencyObject owner, bool value)
        {
            owner.SetValue(IsDraggingProperty, value);
        }

        #endregion

        #region Attached Property: StartPoint

        /// <summary>
        /// Identifies the StartPoint attached property, used internally to track
        /// the position where a potential drag operation started.
        /// </summary>
        public static readonly DependencyProperty StartPointProperty = DependencyProperty.RegisterAttached(
                                      "StartPoint",
                                      typeof(Point),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(null));


        static Point GetStartPoint(DependencyObject owner)
        {
            return (Point)owner.GetValue(StartPointProperty);
        }

        static void SetStartPoint(DependencyObject owner, Point value)
        {
            owner.SetValue(StartPointProperty, value);
        }

        #endregion

        #region Attached Property: OnDropCommand

        /// <summary>
        /// Identifies the OnDropCommand attached property: the command executed, with a
        /// <see cref="DropArgs"/> parameter, when data is dropped on the element. Setting it makes the element a drop target.
        /// </summary>
        public static readonly DependencyProperty OnDropCommandProperty = DependencyProperty.RegisterAttached(
                                      "OnDropCommand",
                                      typeof(ICommand),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(null, OnOnDropCommandChanged));


        /// <summary>
        /// Gets the drop command of the given object.
        /// </summary>
        /// <param name="owner">The object.</param>
        /// <returns>The drop command.</returns>
        public static ICommand GetOnDropCommand(DependencyObject owner)
        {
            return (ICommand)owner.GetValue(OnDropCommandProperty);
        }

        /// <summary>
        /// Sets the drop command of the given object.
        /// </summary>
        /// <param name="owner">The object.</param>
        /// <param name="value">The drop command.</param>
        public static void SetOnDropCommand(DependencyObject owner, ICommand value)
        {
            owner.SetValue(OnDropCommandProperty, value);
        }

        #endregion

        #region Attached Property: OnDragEnterCommand

        /// <summary>
        /// Identifies the OnDragEnterCommand attached property: the command executed, with a
        /// <see cref="DragEnterArgs"/> parameter, when a drag operation enters the element.
        /// </summary>
        public static readonly DependencyProperty OnDragEnterCommandProperty = DependencyProperty.RegisterAttached(
                                      "OnDragEnterCommand",
                                      typeof(ICommand),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Gets the drag enter command of the given object.
        /// </summary>
        /// <param name="owner">The object.</param>
        /// <returns>The drag enter command.</returns>
        public static ICommand GetOnDragEnterCommand(DependencyObject owner)
        {
            return (ICommand)owner.GetValue(OnDragEnterCommandProperty);
        }

        /// <summary>
        /// Sets the drag enter command of the given object.
        /// </summary>
        /// <param name="owner">The object.</param>
        /// <param name="value">The drag enter command.</param>
        public static void SetOnDragEnterCommand(DependencyObject owner, ICommand value)
        {
            owner.SetValue(OnDragEnterCommandProperty, value);
        }

        #endregion

        #region Attached Property: OnDragLeaveCommand

        /// <summary>
        /// Identifies the OnDragLeaveCommand attached property: the command executed, with a
        /// <see cref="DragLeaveArgs"/> parameter, when a drag operation leaves the element.
        /// </summary>
        public static readonly DependencyProperty OnDragLeaveCommandProperty = DependencyProperty.RegisterAttached(
                                      "OnDragLeaveCommand",
                                      typeof(ICommand),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Gets the drag leave command of the given object.
        /// </summary>
        /// <param name="owner">The object.</param>
        /// <returns>The drag leave command.</returns>
        public static ICommand GetOnDragLeaveCommand(DependencyObject owner)
        {
            return (ICommand)owner.GetValue(OnDragLeaveCommandProperty);
        }

        /// <summary>
        /// Sets the drag leave command of the given object.
        /// </summary>
        /// <param name="owner">The object.</param>
        /// <param name="value">The drag leave command.</param>
        public static void SetOnDragLeaveCommand(DependencyObject owner, ICommand value)
        {
            owner.SetValue(OnDragLeaveCommandProperty, value);
        }

        #endregion

        /// <summary>
        /// Finds the drag container that is the WPF element 
        /// that holds the DataObject that will be dragged.
        /// </summary>
        /// <param name="originalSource">
        /// The original source where the 
        /// drag operation started.
        /// </param>
        /// <returns>
        /// The element that holds the DataObject, 
        /// or null if none can be found.
        /// </returns>
        static DependencyObject FindDragContainer(DependencyObject originalSource)
        {
            var element = originalSource.FindParent<DependencyObject>(t =>
            {
                return GetDataObject(t) != null;
            });

            return element;
        }

        /// <summary>
        /// Finds the WPF element where the DropTarget property has been defined.
        /// </summary>
        /// <param name="originalSource">
        /// The original source where the Drop, 
        /// or DragOver, operation is happening.
        /// </param>
        /// <returns>
        /// The element that holds the DropTarget, 
        /// or null if none can be found.
        /// </returns>
        static DependencyObject FindDropTargetContainer(DependencyObject originalSource)
        {
            var element = originalSource.FindParent<DependencyObject>(t =>
            {
                return GetDropTarget(t) != null;
            });

            return element;
        }

        static object FindDropTarget(DependencyObject originalSource)
        {
            var element = FindDropTargetContainer(originalSource);
            if (element != null)
            {
                return GetDropTarget(element);
            }

            return null;
        }

        /// <summary>
        /// Finds the WPF element where the drop command is attached.
        /// </summary>
        /// <param name="originalSource">
        /// The original source where the Drop, 
        /// or DragOver, operation is happening.
        /// </param>
        /// <returns>
        /// The element that holds the drop command, 
        /// or null if none can be found.
        /// </returns>
        static DependencyObject FindDropCommandHolder(DependencyObject originalSource)
        {
            var element = originalSource.FindParent<DependencyObject>(t =>
            {
                return GetOnDropCommand(t) != null;
            });

            return element;
        }

        static ICommand FindDropCommand(DependencyObject originalSource)
        {
            var element = FindDropCommandHolder(originalSource);
            if (element != null)
            {
                return GetOnDropCommand(element);
            }

            return null;
        }

        static DependencyObject FindDragEnterCommandHolder(DependencyObject originalSource)
        {
            var element = originalSource.FindParent<DependencyObject>(t =>
            {
                return GetOnDragEnterCommand(t) != null;
            });

            return element;
        }

        static ICommand FindDragEnterCommand(DependencyObject originalSource)
        {
            var element = FindDragEnterCommandHolder(originalSource);
            if (element != null)
            {
                return GetOnDragEnterCommand(element);
            }

            return null;
        }

        static DependencyObject FindDragLeaveCommandHolder(DependencyObject originalSource)
        {
            var element = originalSource.FindParent<DependencyObject>(t =>
            {
                return GetOnDragLeaveCommand(t) != null;
            });

            return element;
        }

        static ICommand FindDragLeaveCommand(DependencyObject originalSource)
        {
            var element = FindDragLeaveCommandHolder(originalSource);
            if (element != null)
            {
                return GetOnDragLeaveCommand(element);
            }

            return null;
        }

        private static void OnOnDropCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var attached = GetIsDropTargetAttached(d);
            if (!attached)
            {
                SetIsDropTargetAttached(d, true);

                var ui = (UIElement)d;

                ui.AllowDrop = true;
                if (ui is Control ctrl)
                {
                    var bkg = ctrl.GetValue(Control.BackgroundProperty);
                    if (bkg == null)
                    {
                        ctrl.SetValue(Control.BackgroundProperty, Brushes.Transparent);
                    }
                }

                ui.DragEnter += (s, args) =>
                {
                    var os = (DependencyObject)args.OriginalSource;
                    var command = FindDragEnterCommand(os);
                    if (command != null)
                    {
                        var dropTarget = FindDropTarget(os);
                        var cmdArgs = new DragEnterArgs(
                            args.Data,
                            args.KeyStates,
                            dropTarget,
                            args.AllowedEffects);

                        if (command.CanExecute(cmdArgs))
                        {
                            command.Execute(cmdArgs);
                        }
                    }
                };

                ui.DragLeave += (s, args) =>
                {
                    var os = (DependencyObject)args.OriginalSource;
                    var command = FindDragLeaveCommand(os);
                    if (command != null)
                    {
                        var dropTarget = FindDropTarget(os);
                        var cmdArgs = new DragLeaveArgs(
                            args.Data,
                            args.KeyStates,
                            dropTarget,
                            args.AllowedEffects);

                        if (command.CanExecute(cmdArgs))
                        {
                            command.Execute(cmdArgs);
                        }
                    }
                };

                ui.DragOver += (s, args) =>
                {
                    var os = (DependencyObject)args.OriginalSource;

                    var command = FindDropCommand(os);
                    if (command != null)
                    {
                        var dropTarget = FindDropTarget(os);

                        Point position = new Point(0, 0);
                        if (os is IInputElement)
                        {
                            position = args.GetPosition((IInputElement)os);
                        }

                        var cmdArgs = new DragOverArgs(
                            args.Data,
                            args.KeyStates,
                            dropTarget,
                            args.AllowedEffects,
                            position);

                        var result = command.CanExecute(cmdArgs);
                        if (!result)
                        {
                            args.Effects = cmdArgs.Effects;
                            args.Handled = true;
                        }
                    }
                    else
                    {
                        args.Effects = args.AllowedEffects;
                        args.Handled = true;
                    }
                };

                ui.Drop += (s, args) =>
                {
                    var os = (DependencyObject)args.OriginalSource;

                    var command = FindDropCommand(os);
                    if (command != null)
                    {
                        var dropTarget = FindDropTarget(os);

                        Point position = new Point(0, 0);
                        if (os is IInputElement)
                        {
                            position = args.GetPosition((IInputElement)os);
                        }

                        var cmdArgs = new DropArgs(args.Data, args.KeyStates, dropTarget, position);
                        command.Execute(cmdArgs);
                    }
                };
            }
        }
    }
}