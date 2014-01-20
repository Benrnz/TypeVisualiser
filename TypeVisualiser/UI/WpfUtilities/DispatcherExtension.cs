namespace TypeVisualiser.UI.WpfUtilities
{
    using System;
    using System.Windows.Threading;
    using Properties;

    /// <summary>
    /// An extension for the <see cref="Dispatcher"/>
    /// </summary>
    public static class DispatcherExtension
    {
        /// <summary>
        /// Call the dispatchers BeginInvoke using a lambda.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="action">The action.</param>
        /// <param name="priority">The priority.</param>
        public static void BeginInvoke(
            this Dispatcher instance,
            Action action,
            DispatcherPriority priority)
        {
            if (instance == null)
            {
                throw new ArgumentNullResourceException("instance", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            instance.BeginInvoke(action, priority);
        }
    }
}