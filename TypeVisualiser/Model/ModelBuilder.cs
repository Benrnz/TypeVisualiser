namespace TypeVisualiser.Model
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    using StructureMap;
    using StructureMap.Pipeline;

    using TypeVisualiser.Messaging;
    using TypeVisualiser.Properties;
    using TypeVisualiser.Startup;

    internal class ModelBuilder : IModelBuilder
    {
        private static readonly ConcurrentDictionary<Type, IVisualisableType> TypeCache = new ConcurrentDictionary<Type, IVisualisableType>();

        // Do not make readonly - need to set in testing
        // ReSharper disable FieldCanBeMadeReadOnly.Local

        //// ReSharper restore FieldCanBeMadeReadOnly.Local
        private IContainer doNotUseFactory;

        private IUserPromptMessage userPrompt = new WindowsMessageBox();

        public ModelBuilder()
        {
        }

        public ModelBuilder(IContainer factory)
            : this()
        {
            this.doNotUseFactory = factory;
        }

        protected IContainer Factory
        {
            get
            {
                return this.doNotUseFactory ?? (this.doNotUseFactory = IoC.Default);
            }
        }

        public static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Logger.Instance.WriteEntry("OnAssemblyResolve");
            Logger.Instance.WriteEntry("   Resolving " + args.Name);

            string folderPath = Path.GetDirectoryName(args.RequestingAssembly.Location);
            if (string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentException(Resources.ModelBuilder_OnAssemblyResolve_ResolveEventArgs_RequestingAssembly_Location_is_null_or_empty, "args");
            }

            string assemblyNameObject = new AssemblyName(args.Name).Name + ".dll";
            string assemblyPath = Path.Combine(folderPath, assemblyNameObject);

            // Check in the same path as the user selected assembly.
            if (File.Exists(assemblyPath))
            {
                return Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            }

            // Check if there is a subfolder called "Dependencies" - this is used in testing to better organise the number of dependencies.
            assemblyPath = Path.Combine(Path.Combine(folderPath, "Dependencies"), assemblyNameObject);
            if (File.Exists(assemblyPath))
            {
                return Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            }

            // Try dot net default.
            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        /// <summary>
        /// This overload is only used when Navigating to an existing type on a diagram.
        /// </summary>
        /// <param name="type">
        /// The existing type from the diagram
        /// </param>
        /// <param name="depth">
        /// A depth counter to prevent infinitely recursively loading types
        /// </param>
        /// <returns>
        /// The <see cref="IVisualisableTypeWithAssociations"/>.
        /// </returns>
        public IVisualisableTypeWithAssociations BuildSubject(IVisualisableType type, int depth)
        {
            if (type == null)
            {
                throw new ArgumentNullResourceException("type", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            Type netType = this.BuildType(type.AssemblyFileName, type.AssemblyQualifiedName);
            IVisualisableTypeWithAssociations subject = this.BuildSubject(netType, depth);
            subject.InitialiseForReuseFromCache(depth);
            return subject;
        }

        public IVisualisableTypeWithAssociations BuildSubject(Type type, int depth)
        {
            if (type == null)
            {
                throw new ArgumentNullResourceException("type", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            var visualisableTypeSubject = TryGettingTypeFromCache(type) as IVisualisableTypeWithAssociations;
            if (visualisableTypeSubject != null)
            {
                return visualisableTypeSubject;
            }

            var args = new Dictionary<string, object> { { "type", type }, { "depth", depth }, { "factory", this.Factory } };
            visualisableTypeSubject = this.Factory.GetInstance<IVisualisableTypeWithAssociations>(new ExplicitArguments(args));

            TypeCache.TryAdd(type, visualisableTypeSubject);
            visualisableTypeSubject.InitialiseForReuseFromCache(depth);
            return visualisableTypeSubject;
        }

        public IVisualisableTypeWithAssociations BuildSubject(string assemblyFile, string fullTypeName, int depth)
        {
            return this.BuildSubject(this.BuildType(assemblyFile, fullTypeName), depth);
        }

        public Type BuildType(string assemblyFile, string fullTypeName)
        {
            Type type = null;
            try
            {
                type = Type.ReflectionOnlyGetType(fullTypeName, true, false);
            }
            catch (FileNotFoundException)
            {
                // Ignore this exception. This means the type you are attempting to load has not had its assembly loaded into the appdomain.
            }

            if (type != null)
            {
                return type;
            }

            if (string.IsNullOrWhiteSpace(assemblyFile))
            {
                throw new ArgumentNullResourceException("assemblyFile", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            Assembly assembly = this.LoadAssembly(assemblyFile);
            if (assembly == null)
            {
                throw new FileLoadException("Load Assembly failed.", assemblyFile);
            }

            string shortTypeName = ParseShortTypeName(fullTypeName);
            type = assembly.GetType(shortTypeName);
            if (type == null)
            {
                throw new TypeLoadException("Load Type failed. " + fullTypeName);
            }

            return type;
        }

        public IVisualisableType BuildVisualisableType(Type type, int depth)
        {
            if (type == null)
            {
                return null;
            }

            IVisualisableType cachedType = TryGettingTypeFromCache(type);
            if (cachedType != null)
            {
                if (type.AssemblyQualifiedName != cachedType.AssemblyQualifiedName)
                {
                    Logger.Instance.WriteEntry("Cached type retrieved does not matched expected fully qualified name.\n{0}\n{1}", type.AssemblyQualifiedName, cachedType.AssemblyQualifiedName);
                    Debug.Assert(
                        type.AssemblyQualifiedName == cachedType.AssemblyQualifiedName, 
                        "Cached type does not match the type required - matching by GUID may not be working as intended. " + type.AssemblyQualifiedName + " != " + cachedType.AssemblyQualifiedName);
                }

                return cachedType;
            }

            var args = new Dictionary<string, object> { { "type", type }, { "depth", depth }, { "factory", this.Factory } };
            cachedType = this.Factory.GetInstance<IVisualisableTypeWithAssociations>(new ExplicitArguments(args));
            TypeCache.TryAdd(type, cachedType);
            return cachedType;
        }

        public Assembly LoadAssembly(string fileName)
        {
            Assembly subjectLibrary = null;
            try
            {
                subjectLibrary = Assembly.ReflectionOnlyLoadFrom(fileName);
            }
            catch (FileLoadException ex)
            {
                Logger.Instance.WriteEntry("ShellController.GetAssemblyFromFile");
                Logger.Instance.WriteEntry(ex);
                this.userPrompt.Show(ex, "Unable to load assembly: {0} most likely due to missing dependent libraries.", fileName);
            }
            catch (FileNotFoundException ex)
            {
                Logger.Instance.WriteEntry("ShellController.GetAssemblyFromFile");
                Logger.Instance.WriteEntry(ex);
                this.userPrompt.Show(ex, "File not found ({0}). Or one of its dependencies is not found.", fileName);
            }

            return subjectLibrary;
        }

        private static string ParseShortTypeName(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ArgumentNullException("typeName", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            string[] parts = typeName.Split(',');
            return parts[0];
        }

        private static IVisualisableType TryGettingTypeFromCache(Type type)
        {
            IVisualisableType cachedType;
            if (TypeCache.TryGetValue(type, out cachedType))
            {
                var subject = cachedType as IVisualisableTypeWithAssociations;
                if (subject != null)
                {
                    return subject;
                }
            }

            return null;
        }
    }
}