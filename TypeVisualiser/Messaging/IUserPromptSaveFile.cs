namespace TypeVisualiser.Messaging
{
    public interface IUserPromptSaveFile
    {
        bool? AddExtension { get; set; }
        string DefaultExt { get; set; }
        string InitialDirectory { get; set; }
        string Filter { get; set; }
        string FileName { get; }
        bool? ShowDialog();
    }
}
