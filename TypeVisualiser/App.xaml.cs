namespace TypeVisualiser
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    using GalaSoft.MvvmLight.Messaging;

    using TypeVisualiser.Messaging;
    using TypeVisualiser.Model;
    using TypeVisualiser.Startup;

    /// <summary>
    /// Interaction logic for the main Application class.
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.DispatcherUnhandledException += this.OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += this.OnCurrentDomainUnhandledException;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ModelBuilder.OnAssemblyResolve;

            Current.Exit += this.OnApplicationExit;
            IoC.MapHardcodedRegistrations();
        }

        private static void LogUnhandledException(string origin, object ex)
        {
            Logger.Instance.WriteEntry(string.Empty);
            Logger.Instance.WriteEntry("=====================================================================================");
            Logger.Instance.WriteEntry(DateTime.Now);
            Logger.Instance.WriteEntry("Unhandled exception was thrown from orgin: " + origin);
            string message = ex.ToString();
            Logger.Instance.WriteEntry(message);

            new WindowsMessageBox().Show(TypeVisualiser.Properties.Resources.Application_An_Unhandled_Exception_Occurred, (object)message);
            Current.Shutdown();
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            Current.Exit -= this.OnApplicationExit;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= ModelBuilder.OnAssemblyResolve;
            Messenger.Default.Send(new ShutdownMessage());
        }

        private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogUnhandledException("App.OnCurrentDomainUnhandledException", e.ExceptionObject);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogUnhandledException("App.OnDispatcherUnhandledException", e.Exception);
        }
    }
}