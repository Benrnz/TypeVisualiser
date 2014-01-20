using System.Diagnostics;
using System.Globalization;

namespace TypeVisualiser
{
    public class Logger : ILogger
    {
        private static volatile ILogger instance;
        private static readonly object SyncRoot = new object();

        public static ILogger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new Logger();
                        }
                    }
                }

                return instance;
            }
        }

        public void WriteEntry(string logEntry, params object[] args)
        {
#if(DEBUG)
            if (string.IsNullOrWhiteSpace(logEntry))
            {
                return;
            }

            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, logEntry, args));
#endif
        }

        public void WriteEntry(object something)
        {
#if(DEBUG)
            if (something == null)
            {
                return;
            }

            WriteEntry(something.ToString());
#endif
        }
    }
}
