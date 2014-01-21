using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using TypeVisualiser.DemoTypes;
using TypeVisualiser.Messaging;
using TypeVisualiser.Model;
using TypeVisualiser.Model.Persistence;
using TypeVisualiser.Properties;
using TypeVisualiser.RecentFiles;
using TypeVisualiser.Startup;

namespace TypeVisualiser
{
    public class FileManager : IFileManager
    {
        private readonly IMessenger messenger;
        private IContainer doNotUseFactory;
        private IModelBuilder doNotUseModelBuilder;
        private IRecentFiles doNotUseRecentFiles;
        private IUserPromptMessage doNotUseUserPrompt;

        // ReSharper disable FieldCanBeMadeReadOnly.Local
        private Func<IUserPromptOpenFile> userPromptOpenFileFactory =
            () =>
            new WindowsOpenFileDialog
                {
                    AddExtension = true,
                    CheckFileExists = true,
                    CheckPathExists = true,
                    DefaultExt = "*.DLL",
                    Title = "Select a .NET Assembly (DLL) to load a type from",
                    Filter = ".NET Assemblies (*.DLL, *.EXE)|*.DLL;*.EXE",
                    FilterIndex = 0,
                };

        private Func<IUserPromptSaveFile> userPromptSaveFileFactory = () => new WindowsSaveDialog { AddExtension = true, DefaultExt = "tvd", Filter = "Type Visualiser Diagram (*.tvd)|*.tvd" };

        // ReSharper restore FieldCanBeMadeReadOnly.Local
        public FileManager(IMessenger messenger)
        {
            this.messenger = messenger;
        }

        public virtual IRecentFiles RecentFiles
        {
            get { return this.doNotUseRecentFiles ?? (this.doNotUseRecentFiles = Factory.GetInstance<IRecentFiles>()); }
        }

        protected virtual IContainer Factory
        {
            get { return this.doNotUseFactory ?? (this.doNotUseFactory = IoC.Default); }
        }

        protected virtual IModelBuilder ModelBuilder
        {
            get { return this.doNotUseModelBuilder ?? (this.doNotUseModelBuilder = Factory.GetInstance<IModelBuilder>()); }
        }

        protected virtual string RunningDirectory
        {
            get { return Path.GetDirectoryName(GetType().Assembly.Location); }
        }

        protected IUserPromptMessage UserPrompt
        {
            get { return this.doNotUseUserPrompt ?? (this.doNotUseUserPrompt = new WindowsMessageBox()); }

            set { this.doNotUseUserPrompt = value; }
        }

        public void ChooseAssembly()
        {
            string assemblyFileName;
            if (!PromptUserForAssembly(out assemblyFileName))
            {
                return;
            }

            RecentFiles.SetCurrentFile(assemblyFileName);
        }

        public void Initialise()
        {
            RecentFiles.LoadRecentFiles();
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Multi-CLR Language not needed")]
        public Assembly LoadAssembly(string assemblyFileName = "")
        {
            if (string.IsNullOrEmpty(assemblyFileName))
            {
                if (!PromptUserForAssembly(out assemblyFileName))
                {
                    return null;
                }
            }

            RecentFiles.SetCurrentFile(assemblyFileName);
            return ModelBuilder.LoadAssembly(assemblyFileName);
        }

        public IVisualisableTypeWithAssociations LoadDemoType()
        {
            RecentFiles.SaveRecentFile();

            Type demoType = typeof (Car);
            IVisualisableTypeWithAssociations subject = ModelBuilder.BuildSubject(demoType, 0);
            RecentFiles.SetCurrentFile(string.Empty);
            RecentFiles.SetCurrentType(demoType.AssemblyQualifiedName);
            return subject;
        }

        public TypeVisualiserLayoutFile LoadDiagram()
        {
            IUserPromptOpenFile dialog = this.userPromptOpenFileFactory();
            dialog.DefaultExt = "*.TVD";
            dialog.Title = "Select a Type Visualiser Diagram file to load";
            dialog.Filter = "Type Visualiser Diagram (*.TVD)|*.TVD";
            bool? result = dialog.ShowDialog();
            if (result == null || result == false)
            {
                return null;
            }

            string fileName = dialog.FileName;

            // Clear must finish before loading can continue.
            TypeVisualiserLayoutFile diagram = LoadDiagramFromFile(fileName);
            if (diagram == null)
            {
                // User canceled.
                return null;
            }

            RecentFiles.SetLastAccessed(new RecentFile { FileName = diagram.AssemblyFileName, TypeName = diagram.AssemblyFullName, When = DateTime.Now });
            return diagram;
        }

        public Task<IVisualisableTypeWithAssociations> LoadFromDiagramFileAsync(TypeVisualiserLayoutFile layout)
        {
            if (layout == null)
            {
                throw new ArgumentNullResourceException("layout", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            return Task.Factory.StartNew(() => ModelBuilder.BuildSubject(layout.AssemblyFileName, layout.Subject.FullName, 0), TaskCreationOptions.LongRunning);
        }

        public Task<IVisualisableTypeWithAssociations> LoadFromRecentFileAsync(RecentFile recentFileData)
        {
            Type type;
            if (recentFileData == null)
            {
                throw new ArgumentNullResourceException("recentFileData", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            try
            {
                type = ModelBuilder.BuildType(recentFileData.FileName, recentFileData.TypeName);
            } catch (ArgumentException ex)
            {
                HandleLoadRecentFileException(recentFileData, ex);
                return null;
            } catch (FileLoadException ex)
            {
                HandleLoadRecentFileException(recentFileData, ex);
                return null;
            } catch (TypeLoadException ex)
            {
                HandleLoadRecentFileException(recentFileData, ex);
                return null;
            }

            if (type == null)
            {
                return null;
            }

            RecentFiles.SetLastAccessed(recentFileData);
            RecentFiles.SaveRecentFile();
            return LoadTypeAsync(type);
        }

        public Task<IVisualisableTypeWithAssociations> LoadFromVisualisableTypeAsync(IVisualisableType type)
        {
            if (type == null)
            {
                return null;
            }

            RecentFiles.SetCurrentFile(type.AssemblyFileName);
            RecentFiles.SetCurrentType(type.AssemblyQualifiedName);
            RecentFiles.SaveRecentFile();

            return Task.Factory.StartNew(() => new ModelBuilder().BuildSubject(type, 0), TaskCreationOptions.LongRunning);
        }

        public Task<IVisualisableTypeWithAssociations> LoadTypeAsync(Type type)
        {
            if (type == null)
            {
                return null;
            }

            RecentFiles.SetCurrentType(type.AssemblyQualifiedName);
            RecentFiles.SaveRecentFile();
            return Task.Factory.StartNew(() => new ModelBuilder().BuildSubject(type, 0), TaskCreationOptions.LongRunning);
        }

        public IVisualisableTypeWithAssociations RefreshSubject(Type subjectType)
        {
            return ModelBuilder.BuildSubject(subjectType, 0);
        }

        public Type RefreshType(string fullTypeName, string assemblyFileName)
        {
            return ModelBuilder.BuildType(assemblyFileName, fullTypeName);
        }

        public void SaveDiagram(IPersistentFileData layoutData)
        {
            if (layoutData == null)
            {
                throw new ArgumentNullResourceException("layoutData", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            string myLocation = RunningDirectory;
            IUserPromptSaveFile dialog = this.userPromptSaveFileFactory();
            dialog.InitialDirectory = myLocation;

            bool? result = dialog.ShowDialog();
            if (result == null || result == false)
            {
                return;
            }

            string fileName = dialog.FileName;

            try
            {
                ModelSerialiser serialiser = BuildModelSerialiser();
                serialiser.Serialise(fileName, layoutData);
            } catch (IOException ex)
            {
                UserPrompt.Show(ex, Resources.ShellController_SaveDiagramExecute_IO_Error_Occured);
            } catch (XmlException ex)
            {
                UserPrompt.Show(ex, Resources.ShellController_SaveDiagramExecute_XML_Serialisation_error);
            }
        }

        protected virtual ModelSerialiser BuildModelSerialiser()
        {
            return new ModelSerialiser();
        }

        private void HandleLoadRecentFileException(RecentFile recentFileData, Exception ex)
        {
            Logger.Instance.WriteEntry("Exception occured in ShellController.LoadFromRecentFileAsync\n" + ex);
            this.messenger.Send(new RecentFileDeleteMessage(recentFileData));
            UserPrompt.Show(Resources.ModelBuilder_BuildType_Bad_file_and_or_type_in_Recently_Used_File_List_cannot_load_this_type);
        }

        private TypeVisualiserLayoutFile LoadDiagramFromFile(string fileName)
        {
            TypeVisualiserLayoutFile data = null;
            try
            {
                ModelSerialiser serialiser = BuildModelSerialiser();
                data = serialiser.Deserialise(fileName);
            } catch (IOException ex)
            {
                UserPrompt.Show(ex, Resources.ShellController_LoadDiagramFromFile_IO_Error_occurred);
            } catch (XmlException ex)
            {
                UserPrompt.Show(ex, Resources.ShellController_LoadDiagramFromFile_XML_Deserialisation_error);
            } catch (NotSupportedException)
            {
                UserPrompt.Show(Resources.ShellController_LoadDiagramFromFile_Not_Supported);
            }

            return data;
        }

        private bool PromptUserForAssembly(out string assemblyFileName)
        {
            assemblyFileName = null;
            IUserPromptOpenFile dialog = this.userPromptOpenFileFactory();
            bool? result = dialog.ShowDialog();
            if (!result.HasValue || !result.Value)
            {
                return false;
            }

            assemblyFileName = dialog.FileName;
            return true;
        }
    }
}