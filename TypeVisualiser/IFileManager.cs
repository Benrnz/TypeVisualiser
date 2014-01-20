using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using TypeVisualiser.Model;
using TypeVisualiser.Model.Persistence;
using TypeVisualiser.RecentFiles;

namespace TypeVisualiser
{
    public interface IFileManager
    {
        IRecentFiles RecentFiles { get; }

        void ChooseAssembly();
        void Initialise();

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Multi-CLR Language not needed")]
        Assembly LoadAssembly(string assemblyFileName = "");

        IVisualisableTypeWithAssociations  LoadDemoType();
        TypeVisualiserLayoutFile LoadDiagram();
        Task<IVisualisableTypeWithAssociations> LoadFromDiagramFileAsync(TypeVisualiserLayoutFile layout);
        Task<IVisualisableTypeWithAssociations> LoadFromRecentFileAsync(RecentFile recentFileData);
        Task<IVisualisableTypeWithAssociations> LoadFromVisualisableTypeAsync(IVisualisableType type);
        Task<IVisualisableTypeWithAssociations> LoadTypeAsync(Type type);

        IVisualisableTypeWithAssociations RefreshSubject(Type subjectType);
        Type RefreshType(string fullTypeName, string assemblyFileName);
        void SaveDiagram(IPersistentFileData layoutData);
    }
}