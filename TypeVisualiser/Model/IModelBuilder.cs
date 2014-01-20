namespace TypeVisualiser.Model
{
    using System;
    using System.Reflection;

    public interface IModelBuilder
    {
        /// <summary>
        /// This overload is only used when Navigating to an existing type on a diagram.
        /// </summary>
        /// <param name="type">The existing type from the diagram</param>
        /// <param name="depth">A depth counter to prevent infinitely recursively loading types</param>
        IVisualisableTypeWithAssociations BuildSubject(IVisualisableType type, int depth);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Other CLR Language equivelency not reqd.")]
        IVisualisableTypeWithAssociations BuildSubject(Type type, int depth);
        IVisualisableTypeWithAssociations BuildSubject(string assemblyFile, string fullTypeName, int depth);
        IVisualisableType BuildVisualisableType(Type type, int depth);
        Type BuildType(string assemblyFile, string fullTypeName);
        Assembly LoadAssembly(string fileName);
    }
}