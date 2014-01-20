namespace TypeVisualiser.UI.WpfUtilities
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An exception for Desktop Window Manager dll import. dwmapi.dll
    /// </summary>
    [Serializable]
    public class DesktopWindowManagerNotEnabledException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopWindowManagerNotEnabledException"/> class.
        /// </summary>
        public DesktopWindowManagerNotEnabledException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopWindowManagerNotEnabledException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DesktopWindowManagerNotEnabledException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopWindowManagerNotEnabledException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DesktopWindowManagerNotEnabledException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopWindowManagerNotEnabledException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected DesktopWindowManagerNotEnabledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}