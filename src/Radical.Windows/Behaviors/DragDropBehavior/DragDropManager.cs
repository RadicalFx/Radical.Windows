﻿using System;
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

        public static readonly DependencyProperty DropTargetProperty = DependencyProperty.RegisterAttached(
                                      "DropTarget",
                                      typeof(object),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(null));


        public static object GetDropTarget(DependencyObject owner)
        {
            return (object)owner.GetValue(DropTargetProperty);
        }

        public static void SetDropTarget(DependencyObject owner, object value)
        {
            owner.SetValue(DropTargetProperty, value);
        }

        #endregion

        #region Attached Property: DataObjectType

        public static readonly DependencyProperty DataObjectTypeProperty = DependencyProperty.RegisterAttached(
                                      "DataObjectType",
                                      typeof(string),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(null));


        public static string GetDataObjectType(DependencyObject owner)
        {
            return (string)owner.GetValue(DataObjectTypeProperty);
        }

        public static void SetDataObjectType(DependencyObject owner, string value)
        {
            owner.SetValue(DataObjectTypeProperty, value);
        }

        #endregion

        #region Attached Property: DataObject

        public static readonly DependencyProperty DataObjectProperty = DependencyProperty.RegisterAttached(
                                      "DataObject",
                                      typeof(object),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(null, OnDataObjectChanged));


        public static object GetDataObject(DependencyObject owner)
        {
            return (object)owner.GetValue(DataObjectProperty);
        }

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

        public static readonly DependencyProperty IsDraggingProperty = DependencyProperty.RegisterAttached(
                                      "IsDragging",
                                      typeof(bool),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(false));


        /// <summary>
        /// Gets the is dragging.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns></returns>
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

        public static readonly DependencyProperty OnDropCommandProperty = DependencyProperty.RegisterAttached(
                                      "OnDropCommand",
                                      typeof(ICommand),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(null, OnOnDropCommandChanged));


        public static ICommand GetOnDropCommand(DependencyObject owner)
        {
            return (ICommand)owner.GetValue(OnDropCommandProperty);
        }

        public static void SetOnDropCommand(DependencyObject owner, ICommand value)
        {
            owner.SetValue(OnDropCommandProperty, value);
        }

        #endregion

        #region Attached Property: OnDragEnterCommand

        public static readonly DependencyProperty OnDragEnterCommandProperty = DependencyProperty.RegisterAttached(
                                      "OnDragEnterCommand",
                                      typeof(ICommand),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(null));


        public static ICommand GetOnDragEnterCommand(DependencyObject owner)
        {
            return (ICommand)owner.GetValue(OnDragEnterCommandProperty);
        }

        public static void SetOnDragEnterCommand(DependencyObject owner, ICommand value)
        {
            owner.SetValue(OnDragEnterCommandProperty, value);
        }

        #endregion

        #region Attached Property: OnDragLeaveCommand

        public static readonly DependencyProperty OnDragLeaveCommandProperty = DependencyProperty.RegisterAttached(
                                      "OnDragLeaveCommand",
                                      typeof(ICommand),
                                      typeof(DragDropManager),
                                      new FrameworkPropertyMetadata(null));


        public static ICommand GetOnDragLeaveCommand(DependencyObject owner)
        {
            return (ICommand)owner.GetValue(OnDragLeaveCommandProperty);
        }

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