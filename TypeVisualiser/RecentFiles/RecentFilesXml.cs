namespace TypeVisualiser.RecentFiles
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Xaml;
    using Messaging;
    using Properties;

    internal class RecentFilesXml : IRecentFiles
    {
        private const string FileName = "TypeVisualiserRecentFiles.xml";
        private readonly IUserPromptMessage userPrompt = new WindowsMessageBox();
        private string currentType;
        private string doNotUseFullFileName;
        private DateTime when;

        public RecentFilesXml()
        {
            RecentlyUsedFiles = new ObservableCollection<RecentFile>();
            MessagingGate.Register<RecentFileDeleteMessage>(this, OnRemoveRecentlyUsedFileRequested);
        }

        private string AssemblyFileName { get; set; }

        public ObservableCollection<RecentFile> RecentlyUsedFiles { get; private set; }

        private string FullFileName
        {
            get
            {
                if (string.IsNullOrEmpty(this.doNotUseFullFileName))
                {
                    string location = Path.GetDirectoryName(GetType().Assembly.Location);
                    if (location == null)
                    {
                        throw new ArgumentException("Assembly.Location returned null.");
                    }

                    this.doNotUseFullFileName = Path.Combine(location, FileName);
                }

                return this.doNotUseFullFileName;
            }
        }

        public void LoadRecentFiles()
        {
            if (!File.Exists(FullFileName))
            {
                return;
            }

            try
            {
                object serialised = XamlServices.Load(FullFileName);
                var correctFormat = serialised as List<RecentFile>;
                if (correctFormat == null)
                {
                    throw new IOException(Resources.RecentFilesXml_LoadRecentFiles_The_recent_files_list_was_not_in_the_correct_format);
                }

                RecentlyUsedFiles = new ObservableCollection<RecentFile>(correctFormat.OrderByDescending(x => x.When).Take(20));
            } catch (IOException ex)
            {
                this.userPrompt.Show(ex, Resources.RecentFilesXml_UnableToLoadRecentFiles);
            }
        }

        public void SaveRecentFile()
        {
            if (string.IsNullOrEmpty(AssemblyFileName))
            {
                return;
            }

            var newRecentFile = new RecentFile { FileName = AssemblyFileName, TypeName = this.currentType, When = this.when };

            RecentFile existing = RecentlyUsedFiles.SingleOrDefault(x => x.FileName == newRecentFile.FileName && x.TypeName == newRecentFile.TypeName);
            if (existing != null)
            {
                RecentlyUsedFiles.Remove(existing);
            }

            RecentlyUsedFiles.Insert(0, newRecentFile);

            // Save to xml file
            string serialised = XamlServices.Save(RecentlyUsedFiles.ToList());
            try
            {
                File.WriteAllText(FullFileName, serialised);
            } catch (IOException ex)
            {
                this.userPrompt.Show(ex, Resources.RecentFilesXml_SaveRecentFile);
            }
        }

        public void SetCurrentFile(string currentFileName)
        {
            AssemblyFileName = currentFileName;
            this.when = DateTime.Now;
        }

        public void SetCurrentType(string currentFullTypeName)
        {
            this.currentType = currentFullTypeName;
            this.when = DateTime.Now;
        }

        public void SetLastAccessed(RecentFile file)
        {
            RecentFile item = RecentlyUsedFiles.SingleOrDefault(x => x.FileName == file.FileName && x.TypeName == file.TypeName);
            if (item != null)
            {
                item.When = DateTime.Now;
                AssemblyFileName = file.FileName;
            }
        }

        private void OnRemoveRecentlyUsedFileRequested(RecentFileDeleteMessage message)
        {
            RecentlyUsedFiles.Remove(message.Data);
        }
    }
}