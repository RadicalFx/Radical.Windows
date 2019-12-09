using Radical.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using System.Windows.Interop;

namespace Radical.Windows.Behaviors
{
    public sealed class WindowControlBoxBehavior : Behavior<Window>
    {
        private const int SwpFramechanged = 0x0020;
        private const int SwpNomove = 0x0002;
        private const int SwpNosize = 0x0001;
        private const int SwpNozorder = 0x0004;
        private const int WsExDlgmodalframe = 0x0001;

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        public bool AllowMaximize
        {
            get;
            set;
        }

        public bool AllowMinimize
        {
            get;
            set;
        }

        EventHandler h;

        public bool ShowIcon { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowControlBoxBehavior"/> class.
        /// </summary>
        public WindowControlBoxBehavior()
        {
            h = (s, e) =>
            {
                var isDesign = DesignTimeHelper.GetIsInDesignMode();
                var hWnd = new WindowInteropHelper(AssociatedObject).Handle;

                if (!isDesign && hWnd != IntPtr.Zero && (!AllowMaximize || !AllowMinimize))
                {
                    var windowLong = NativeMethods.GetWindowLong(hWnd, WindowLong.Style).ToInt32();

                    if (!AllowMaximize)
                    {
                        windowLong = windowLong & ~Constants.WS_MAXIMIZEBOX;
                    }

                    if (!AllowMinimize)
                    {
                        windowLong = windowLong & ~Constants.WS_MINIMIZEBOX;
                    }

                    NativeMethods.SetWindowLong(hWnd, WindowLong.Style, (IntPtr)windowLong);
                }

                if (!isDesign && hWnd != IntPtr.Zero && !ShowIcon)
                {
                    var windowLong = NativeMethods.GetWindowLong(hWnd, WindowLong.ExStyle).ToInt32();

                    AssociatedObject.SourceInitialized += delegate
                    {
                        NativeMethods.SetWindowLong(hWnd, WindowLong.ExStyle, (IntPtr)(windowLong | WsExDlgmodalframe));
                    };

                    // Update the window's non-client area to reflect the changes
                    SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SwpNomove |
                      SwpNosize | SwpNozorder | SwpFramechanged);
                }

            };
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.SourceInitialized += h;
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected override void OnDetaching()
        {
            AssociatedObject.SourceInitialized -= h;
            base.OnDetaching();
        }
    }
}
