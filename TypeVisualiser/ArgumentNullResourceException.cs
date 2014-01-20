namespace TypeVisualiser
{
    using System;
    using System.Globalization;
    using System.Runtime.Serialization;

    [Serializable]
    public class ArgumentNullResourceException : ArgumentNullException
    {
        public ArgumentNullResourceException(string parameterName, string resourceValue)
            : base(parameterName, string.Format(CultureInfo.CurrentCulture, resourceValue, parameterName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentNullResourceException"/> class.        
        /// </summary>
        public ArgumentNullResourceException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentNullResourceException"/> class.        
        /// </summary>
        /// <param name="message">The message.</param>
        public ArgumentNullResourceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentNullResourceException"/> class.        
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ArgumentNullResourceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentNullResourceException"/> class.        
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected ArgumentNullResourceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}