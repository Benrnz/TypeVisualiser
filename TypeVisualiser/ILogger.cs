namespace TypeVisualiser
{
    public interface ILogger
    {
        void WriteEntry(string logEntry, params object[] args);
        void WriteEntry(object something);
    }
}