namespace TypeVisualiser
{
    using System.Windows;

    internal class DefaultApplicationResources : IApplicationResources
    {
        public T GetResource<T>(string key)
        {
            return (T)Application.Current.Resources[key];
        }
    }
}
