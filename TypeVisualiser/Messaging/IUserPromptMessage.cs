namespace TypeVisualiser.Messaging
{
    using System;

    public interface IUserPromptMessage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Not possible to use all optional parameters with string-format params")]
        void Show(string message, string headingCaption = "");

        void Show(string format, object argument1, params object[] args);

        void Show(string headingCaption, string format, object argument1, params object[] args);

        void Show(Exception ex, string message);

        void Show(Exception ex, string format, object argument1, params object[] args);
    }
}
