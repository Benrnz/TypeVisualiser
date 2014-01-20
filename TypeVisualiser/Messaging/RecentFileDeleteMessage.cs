namespace TypeVisualiser.Messaging
{
    using GalaSoft.MvvmLight.Messaging;
    using RecentFiles;

    public class RecentFileDeleteMessage : MessageBase
    {
        public RecentFileDeleteMessage(RecentFile recentFileData)
        {
            Data = recentFileData;
        }

        public RecentFile Data { get; set; }
    }
}