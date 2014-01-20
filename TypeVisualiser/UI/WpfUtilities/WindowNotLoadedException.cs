namespace TypeVisualiser.UI.WpfUtilities
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An exception for actions against a window when the window is not loaded or ready for the action.
    /// </summary>
    [Serializable]
    public class WindowNotLoadedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowNotLoadedException"/> class.
        /// </summary>
        public WindowNotLoadedException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowNotLoadedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public WindowNotLoadedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowNotLoadedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public WindowNotLoadedException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowNotLoadedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected WindowNotLoadedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}