using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TypeVisualiser.UI.WpfUtilities
{
    /// <summary>
    /// This is an extension to the ZoomAndPanControol class that implements
    /// the IScrollInfo interface properties and functions.
    /// 
    /// IScrollInfo is implemented to allow ZoomAndPanControl to be wrapped (in XAML)
    /// in a ScrollViewer.  IScrollInfo allows the ScrollViewer and ZoomAndPanControl to 
    /// communicate important information such as the horizontal and vertical scrollbar offsets.
    /// 
    /// There is a good series of articles showing how to implement IScrollInfo starting here:
    ///     http://blogs.msdn.com/bencon/archive/2006/01/05/509991.aspx
    ///     
    /// </summary>
    public partial class ZoomAndPanControl
    {
        /// <summary>
        /// Set to 'true' when the vertical scrollbar is enabled.
        /// </summary>
        public bool CanHorizontallyScroll { get; set; }

        /// <summary>
        /// Set to 'true' when the vertical scrollbar is enabled.
        /// </summary>
        public bool CanVerticallyScroll { get; set; }

        /// <summary>
        /// The height of the content (with 'ContentScale' applied).
        /// </summary>
        public double ExtentHeight
        {
            get { return this.unscaledExtent.Height*ContentScale; }
        }

        /// <summary>
        /// The width of the content (with 'ContentScale' applied).
        /// </summary>
        public double ExtentWidth
        {
            get { return this.unscaledExtent.Width*ContentScale; }
        }

        /// <summary>
        /// The offset of the horizontal scrollbar.
        /// </summary>
        public double HorizontalOffset
        {
            get { return ContentOffsetX*ContentScale; }
        }

        /// <summary>
        /// Reference to the ScrollViewer that is wrapped (in XAML) around the ZoomAndPanControl.
        /// Or set to null if there is no ScrollViewer.
        /// </summary>
        public ScrollViewer ScrollOwner { get; set; }

        /// <summary>
        /// The offset of the vertical scrollbar.
        /// </summary>
        public double VerticalOffset
        {
            get { return ContentOffsetY*ContentScale; }
        }

        /// <summary>
        /// Get the height of the viewport onto the content.
        /// </summary>
        public double ViewportHeight
        {
            get { return this.viewport.Height; }
        }

        /// <summary>
        /// Get the width of the viewport onto the content.
        /// </summary>
        public double ViewportWidth
        {
            get { return this.viewport.Width; }
        }

        /// <summary>
        /// Shift the content offset one line down.
        /// </summary>
        public void LineDown()
        {
            ContentOffsetY += (ContentViewportHeight/10);
        }

        /// <summary>
        /// Shift the content offset one line left.
        /// </summary>
        public void LineLeft()
        {
            ContentOffsetX -= (ContentViewportWidth/10);
        }

        /// <summary>
        /// Shift the content offset one line right.
        /// </summary>
        public void LineRight()
        {
            ContentOffsetX += (ContentViewportWidth/10);
        }

        /// <summary>
        /// Shift the content offset one line up.
        /// </summary>
        public void LineUp()
        {
            ContentOffsetY -= (ContentViewportHeight/10);
        }

        /// <summary>
        /// Bring the specified rectangle to view.
        /// </summary>
        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            if (visual == null)
            {
                throw new ArgumentNullResourceException("visual", Properties.Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (this.content.IsAncestorOf(visual))
            {
                Rect transformedRect = visual.TransformToAncestor(this.content).TransformBounds(rectangle);
                var viewportRect = new Rect(ContentOffsetX, ContentOffsetY, ContentViewportWidth, ContentViewportHeight);
                if (!transformedRect.Contains(viewportRect))
                {
                    double horizOffset = 0;
                    double vertOffset = 0;

                    if (transformedRect.Left < viewportRect.Left)
                    {
                        //
                        // Want to move viewport left.
                        //
                        horizOffset = transformedRect.Left - viewportRect.Left;
                    } else if (transformedRect.Right > viewportRect.Right)
                    {
                        //
                        // Want to move viewport right.
                        //
                        horizOffset = transformedRect.Right - viewportRect.Right;
                    }

                    if (transformedRect.Top < viewportRect.Top)
                    {
                        //
                        // Want to move viewport up.
                        //
                        vertOffset = transformedRect.Top - viewportRect.Top;
                    } else if (transformedRect.Bottom > viewportRect.Bottom)
                    {
                        //
                        // Want to move viewport down.
                        //
                        vertOffset = transformedRect.Bottom - viewportRect.Bottom;
                    }

                    SnapContentOffsetTo(new Point(ContentOffsetX + horizOffset, ContentOffsetY + vertOffset));
                }
            }
            return rectangle;
        }

        /// <summary>
        /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
        /// used for zooming in and out, not for manipulating the scrollbars.
        /// </summary>
        public void MouseWheelDown()
        {
            if (IsMouseWheelScrollingEnabled)
            {
                LineDown();
            }
        }

        /// <summary>
        /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
        /// used for zooming in and out, not for manipulating the scrollbars.
        /// </summary>
        public void MouseWheelLeft()
        {
            if (IsMouseWheelScrollingEnabled)
            {
                LineLeft();
            }
        }

        /// <summary>
        /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
        /// used for zooming in and out, not for manipulating the scrollbars.
        /// </summary>
        public void MouseWheelRight()
        {
            if (IsMouseWheelScrollingEnabled)
            {
                LineRight();
            }
        }

        /// <summary>
        /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
        /// used for zooming in and out, not for manipulating the scrollbars.
        /// </summary>
        public void MouseWheelUp()
        {
            if (IsMouseWheelScrollingEnabled)
            {
                LineUp();
            }
        }

        /// <summary>
        /// Shift the content offset one page down.
        /// </summary>
        public void PageDown()
        {
            ContentOffsetY += ContentViewportHeight;
        }

        /// <summary>
        /// Shift the content offset one page left.
        /// </summary>
        public void PageLeft()
        {
            ContentOffsetX -= ContentViewportWidth;
        }

        /// <summary>
        /// Shift the content offset one page right.
        /// </summary>
        public void PageRight()
        {
            ContentOffsetX += ContentViewportWidth;
        }

        /// <summary>
        /// Shift the content offset one page up.
        /// </summary>
        public void PageUp()
        {
            ContentOffsetY -= ContentViewportHeight;
        }

        /// <summary>
        /// Called when the offset of the horizontal scrollbar has been set.
        /// </summary>
        public void SetHorizontalOffset(double offset)
        {
            if (this.disableScrollOffsetSync)
            {
                return;
            }

            try
            {
                this.disableScrollOffsetSync = true;

                ContentOffsetX = offset/ContentScale;
            } finally
            {
                this.disableScrollOffsetSync = false;
            }
        }

        /// <summary>
        /// Called when the offset of the vertical scrollbar has been set.
        /// </summary>
        public void SetVerticalOffset(double offset)
        {
            if (this.disableScrollOffsetSync)
            {
                return;
            }

            try
            {
                this.disableScrollOffsetSync = true;

                ContentOffsetY = offset/ContentScale;
            } finally
            {
                this.disableScrollOffsetSync = false;
            }
        }
    }
}