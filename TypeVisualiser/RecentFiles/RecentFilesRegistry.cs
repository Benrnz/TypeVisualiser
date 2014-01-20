// Removed due to friction using the registry. Use the RecentFilesXml impl.

//namespace TypeVisualiser.RecentFiles
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Security;
//    using System.Windows;
//    using Microsoft.Win32;

//    internal class RecentFilesRegistry : IRecentFiles
//    {
//        private const string RegistryKeyName = @"Software\Brees\TypeVisualiser";
//        private const string RegistryKeyNextRecentFileEdit = "NextRecentFileEdit";
//        private const string RegistryKeyRecentFile = "RecentFile";

//        private readonly IList<Tuple<string, string>> recentlyUsedFiles = new List<Tuple<string, string>>();
//        private string lastFileName;
//        private string lastTypeName;

//        public string AssemblyFileName
//        {
//            get
//            {
//                return this.lastFileName;
//            }
//        }

//        public IEnumerable<Tuple<string, string>> RecentlyUsedFiles
//        {
//            get
//            {
//                return this.recentlyUsedFiles;
//            }
//        }

//        public void LoadRecentFiles()
//        {
//            RegistryKey reg = Registry.LocalMachine.OpenSubKey(RegistryKeyName);
//            if (reg == null)
//            {
//                try
//                {
//                    reg = Registry.LocalMachine.CreateSubKey(RegistryKeyName);
//                } catch (UnauthorizedAccessException)
//                {
//                    MessageBox.Show(
//                        Application.Current.MainWindow,
//                        "Unable to create the required registry key, please restart with Administrator permissions. Some features will be unavailable.");
//                    return;
//                }
//            }

//            int nextFileIndex;
//            var recentFile = (string)reg.GetValue(RegistryKeyNextRecentFileEdit);
//            if (!int.TryParse(recentFile, out nextFileIndex))
//            {
//                nextFileIndex = 1;
//            }

//            int index = nextFileIndex - 1;
//            do
//            {
//                recentFile = (string)reg.GetValue(RegistryKeyRecentFile + index);
//                if (recentFile != null)
//                {
//                    this.recentlyUsedFiles.Add(new Tuple<string, string>(recentFile.Split(';')[1], recentFile));
//                }

//                index--;
//                if (index == 0)
//                {
//                    index = 10;
//                }
//            } while (index != nextFileIndex + 1);
//        }

//        public void SaveRecentFile()
//        {
//            if (string.IsNullOrEmpty(this.lastFileName) || string.IsNullOrEmpty(this.lastTypeName))
//            {
//                return;
//            }

//            if (this.recentlyUsedFiles.Any(x => x.Item2 == string.Format(CultureInfo.CurrentCulture, "{0};{1}", this.lastTypeName, this.lastFileName)))
//            {
//                return;
//            }

//            try
//            {
//                RegistryKey reg = Registry.LocalMachine.OpenSubKey(RegistryKeyName, true);
//                object nextIndexEdit = reg.GetValue(RegistryKeyNextRecentFileEdit);
//                if (nextIndexEdit == null)
//                {
//                    nextIndexEdit = "1";
//                }

//                reg.SetValue(RegistryKeyRecentFile + nextIndexEdit, this.lastFileName + ";" + this.lastTypeName);
//                int intNextEdit = int.Parse(nextIndexEdit.ToString()) + 1;
//                if (intNextEdit > 10)
//                {
//                    intNextEdit = 1;
//                }

//                reg.SetValue(RegistryKeyNextRecentFileEdit, intNextEdit.ToString());
//            } catch (SecurityException)
//            {
//                MessageBox.Show("Access denied to the registry. Please re-run this application with privledges to read/write to the registry.");
//            }
//        }

//        public void SetCurrentFile(string currentFileName)
//        {
//            this.lastFileName = currentFileName;
//        }

//        public void SetCurrentType(string currentFullTypeName)
//        {
//            this.lastTypeName = currentFullTypeName;
//        }
//    }
//}