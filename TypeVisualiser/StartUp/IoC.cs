namespace TypeVisualiser.Startup
{
    using System.Diagnostics.CodeAnalysis;

    using GalaSoft.MvvmLight.Messaging;

    using StructureMap;

    using TypeVisualiser.ILAnalyser;
    using TypeVisualiser.Model;
    using TypeVisualiser.RecentFiles;
    using TypeVisualiser.UI;

    public static class IoC
    {
        private static readonly object SyncRoot = new object();

        private static volatile IContainer container;

        public static IContainer Default
        {
            get
            {
                if (container == null)
                {
                    lock (SyncRoot)
                    {
                        if (container == null)
                        {
                            container = new Container();
                        }
                    }
                }

                return container;
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "IoC Registrations here prevent excessive coupling elsewhere.")]
        public static void MapHardcodedRegistrations()
        {
            container = new Container(
                config =>
                    {
                        config.For<IApplicationResources>().Use<DefaultApplicationResources>();
                        config.For<IConnectorBuilder>().Use<FixedPointCollectionConnectorBuilder>().Named(ConnectorType.Snap.ToString());
                        config.For<IConnectorBuilder>().Use<DirectLineConnectorBuilder>().Named(ConnectorType.Direct.ToString());
                        config.For<IDiagramDimensions>().Singleton().Use<DiagramDimensions>();
                            
                            // TODO this is broken - shouldnt be a singleton there is one per diagram. Its only working because it is refreshed each time a diagram is displayed.
                        config.For<IFileManager>().Use<FileManager>();
                        config.For<IMessenger>().Use(Messenger.Default);
                        config.For<IMethodBodyReader>().Use<MethodBodyReader>();
                        config.For<IModelBuilder>().Use<ModelBuilder>();
                        config.For<IRecentFiles>().Use<RecentFilesXml>();
                        config.For<ITrivialFilter>().Singleton().Use<TrivialFilter>();
                        config.For<IVisualisableType>().Use<VisualisableType>();
                        config.For<IVisualisableTypeWithAssociations>().Use<VisualisableTypeWithAssociations>();
                    });
        }
    }
}