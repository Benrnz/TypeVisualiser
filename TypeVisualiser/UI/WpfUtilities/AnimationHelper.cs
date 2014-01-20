using System;
using System.Windows;
using System.Windows.Media.Animation;
using TypeVisualiser.Properties;

namespace TypeVisualiser.UI.WpfUtilities
{
    /// <summary>
    /// A helper class to simplify animation.
    /// </summary>
    public static class AnimationHelper
    {
        /// <summary>
        /// Cancel any animations that are running on the specified dependency property.
        /// </summary>
        public static void CancelAnimation(UIElement animatedElement, DependencyProperty dependencyProperty)
        {
            if (animatedElement == null)
            {
                throw new ArgumentNullResourceException("animatedElement", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            animatedElement.BeginAnimation(dependencyProperty, null);
        }

        /// <summary>
        /// Starts an animation to a particular value on the specified dependency property.
        /// </summary>
        public static void StartAnimation(UIElement animationElement, DependencyProperty dependencyProperty, double toValue, double animationDurationSeconds)
        {
            if (animationElement == null)
            {
                throw new ArgumentNullResourceException("animationElement", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            StartAnimation(animationElement, dependencyProperty, toValue, animationDurationSeconds, null);
        }

        /// <summary>
        /// Starts an animation to a particular value on the specified dependency property.
        /// You can pass in an event handler to call when the animation has completed.
        /// </summary>
        public static void StartAnimation(UIElement animationElement, DependencyProperty dependencyProperty, double toValue, double animationDurationSeconds, EventHandler completedEvent)
        {
            if (animationElement == null)
            {
                throw new ArgumentNullResourceException("animationElement", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            var fromValue = (double)animationElement.GetValue(dependencyProperty);

            var animation = new DoubleAnimation { From = fromValue, To = toValue, Duration = TimeSpan.FromSeconds(animationDurationSeconds) };

            animation.Completed += (sender, e) =>
                                       {
                                           // When the animation has completed bake final value of the animation
                                           // into the property.
                                           animationElement.SetValue(dependencyProperty, animationElement.GetValue(dependencyProperty));
                                           CancelAnimation(animationElement, dependencyProperty);

                                           if (completedEvent != null)
                                           {
                                               completedEvent(sender, e);
                                           }
                                       };

            animation.Freeze();

            animationElement.BeginAnimation(dependencyProperty, animation);
        }
    }
}