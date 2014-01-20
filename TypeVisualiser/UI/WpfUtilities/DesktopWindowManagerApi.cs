namespace TypeVisualiser.UI.WpfUtilities
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Media;
    using Properties;

    /// <summary>
    /// A wrapper class to encapsulate calls to the DWM Win32 API for the Aero Glass effects
    /// </summary>
    public static class DesktopWindowManagerApi
    {
        /// <summary>
        /// Extends the frame into client area.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="thickness">The thickness.</param>
        public static void ExtendFrameIntoClientArea(Window window, Thickness thickness)
        {
            ExtendFrameIntoClientArea(window, thickness, false);
        }

        /// <summary>
        /// Extends the frame into client area.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="exceptionOnFail">if set to <c>true</c> [exception on fail].</param>
        public static void ExtendFrameIntoClientArea(Window window, Thickness thickness, bool exceptionOnFail)
        {
            if (window == null)
            {
                throw new ArgumentNullResourceException("window", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            bool compEnabled = IsCompositionEnabled();
            if (exceptionOnFail && !compEnabled)
            {
                throw new DesktopWindowManagerNotEnabledException();
            }

            if (exceptionOnFail && !window.IsLoaded)
            {
                throw new WindowNotLoadedException();
            }

            if (!compEnabled)
            {
                return;
            }

            var margins = thickness.ToDwmMargins();
            IntPtr windowPointer = new WindowInteropHelper(window).Handle;

            // convert the background to nondrawing             
            HwndSource mainWindowHwnd = HwndSource.FromHwnd(windowPointer);
            if (mainWindowHwnd != null && mainWindowHwnd.CompositionTarget != null)
            {
                mainWindowHwnd.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);
            }

            try
            {
                NativeMethods.DwmExtendFrameIntoClientArea(windowPointer, ref margins);
            } catch (DllNotFoundException)
            {
                window.Background = Brushes.White;
            }
        }

        /// <summary>
        /// Determines whether composition is enabled.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if [is composition enabled]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCompositionEnabled()
        {
            try
            {
                return NativeMethods.DwmIsCompositionEnabled();
            } catch (DllNotFoundException)
            {
                return false;
            }
        }

        private static NativeMethods.DwmMargins ToDwmMargins(this Thickness t)
        {
            var rtrn = new NativeMethods.DwmMargins
            {
                Top = (int)t.Top,
                Bottom = (int)t.Bottom,
                Left = (int)t.Left,
                Right = (int)t.Right
            };
            return rtrn;
        }
    }
}