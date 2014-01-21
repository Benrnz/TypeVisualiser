using System;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using TypeVisualiser.Messaging;
using TypeVisualiser.Model;
using TypeVisualiser.Startup;

namespace TypeVisualiser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += ModelBuilder.OnAssemblyResolve;

            Current.Exit += OnApplicationExit;
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
            if (message.Length > 1024)
            {
                message = message.Substring(0, 1024);
            }

            MessageBox.Show("An unhandled exception occured:\n" + message);
            Current.Shutdown();
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            Current.Exit -= OnApplicationExit;
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