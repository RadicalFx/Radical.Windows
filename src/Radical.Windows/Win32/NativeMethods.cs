using System;
using System.Runtime.InteropServices;

namespace Radical.Win32
{
#pragma warning disable 1591

    /// <summary>
    /// A bridge to frequently used OS APIs
    /// </summary>
    public static class NativeMethods
    {
        private const string User32 = "user32.dll";

        [DllImport(User32, EntryPoint = "GetWindowLong")]
        private static extern int Window_GetLong32(
                [In] IntPtr hWnd,
                [In][MarshalAs(UnmanagedType.U4)] WindowLong index);

        [DllImport(User32, EntryPoint = "SetWindowLong")]
        private static extern int Window_SetLong32(
                [In] IntPtr hWnd,
                [In][MarshalAs(UnmanagedType.U4)] WindowLong index,
                [In] int value);

        [DllImport(User32, EntryPoint = "GetWindowLongPtrW")]
        private static extern IntPtr Window_GetLong64(
                [In] IntPtr hWnd,
                [In][MarshalAs(UnmanagedType.U4)] WindowLong index);

        [DllImport(User32, EntryPoint = "SetWindowLongPtrW")]
        private static extern IntPtr Window_SetLong64(
                [In] IntPtr hWnd,
                [In][MarshalAs(UnmanagedType.U4)] WindowLong index,
                [In] IntPtr value);

        public static IntPtr GetWindowLong(
                IntPtr hWnd,
                WindowLong index)
        {
            // Vista WoW64 does not implement GetWindowLong 
            if (IntPtr.Size == 4)
            {
                return (IntPtr)Window_GetLong32(hWnd, index);
            }
            else
            {
                return Window_GetLong64(hWnd, index);
            }
        }

        public static IntPtr SetWindowLong(
                IntPtr hWnd,
                WindowLong index,
                IntPtr value)
        {
            // Vista WoW64 does not implement SetWindowLong 
            if (IntPtr.Size == 4)
            {
                return (IntPtr)Window_SetLong32(hWnd, index, value.ToInt32());
            }
            else
            {
                return Window_SetLong64(hWnd, index, value);
            }
        }
    }

#pragma warning restore 1591
}
