namespace TypeVisualiser.UI.WpfUtilities
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        #region dwmapi.dll
        /// <summary>         
        /// Extends an hwind's frame into the client area by the specified margins.         
        /// </summary>         
        /// <param name="hwnd">Integer pointer to the window to change the glass area on.</param>         
        /// <param name="margins">Margins, what to set each side to</param>         
        [DllImport("dwmapi.dll", PreserveSig = false)]
        internal static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref DwmMargins margins);

        /// <summary>         
        /// Checks to see if the Desktop window manager is enabled.         
        /// </summary>         
        /// <returns>true for is enabled otherwise false.</returns>
        [DllImport("dwmapi.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DwmIsCompositionEnabled();

        [StructLayout(LayoutKind.Sequential)]
        internal struct DwmMargins
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        #endregion
    }
}
