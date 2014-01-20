namespace TypeVisualiser.RecentFiles
{
    using System.Collections.ObjectModel;

    public interface IRecentFiles
    {
        ObservableCollection<RecentFile> RecentlyUsedFiles { get; }
        void LoadRecentFiles();
        void SaveRecentFile();
        void SetCurrentFile(string currentFileName);
        void SetCurrentType(string currentFullTypeName);
        void SetLastAccessed(RecentFile file);
    }
}