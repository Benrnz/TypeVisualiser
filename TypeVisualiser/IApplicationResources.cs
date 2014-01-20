namespace TypeVisualiser
{
    public interface IApplicationResources
    {
        T GetResource<T>(string key);
    }
}
