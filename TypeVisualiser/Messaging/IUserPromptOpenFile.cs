namespace TypeVisualiser.Messaging
{
    public interface IUserPromptOpenFile
    {
        bool? AddExtension { get; set; }
        bool? CheckFileExists { get; set; }
        bool? CheckPathExists { get; set; }
        string DefaultExt { get; set; }
        string Title { get; set; }
        string Filter { get; set; }
        int? FilterIndex { get; set; }
        string FileName { get; }

        bool?  ShowDialog();
    }
}
