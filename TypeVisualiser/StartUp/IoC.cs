using GalaSoft.MvvmLight.Messaging;
using StructureMap;
using TypeVisualiser.ILAnalyser;
using TypeVisualiser.Model;
using TypeVisualiser.RecentFiles;
using TypeVisualiser.UI;
using TypeVisualiser.UI.Views;

namespace TypeVisualiser.Startup
{
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

        public static void MapHardcodedRegistrations()
        {
            container = new Container(config =>
                {
                    config.For<IApplicationResources>().Use<DefaultApplicationResources>();
                    config.For<IConnectorBuilder>().Use<FixedPointCollectionConnectorBuilder>().Named(ConnectorType.Snap.ToString());
                    config.For<IConnectorBuilder>().Use<DirectLineConnectorBuilder>().Named(ConnectorType.Direct.ToString());
                    config.For<IDiagramDimensions>().Singleton().Use<DiagramDimensions>();
                    config.For<IFileManager>().Use<FileManager>();
                    config.For<IMessenger>().Use(Messenger.Default);
                    config.For<IMethodBodyReader>().Use<MethodBodyReader>();
                    config.For<IModelBuilder>().Use<ModelBuilder>();
                    config.For<IRecentFiles>().Use<RecentFilesXml>();
                    config.For<IVisualisableType>().Use<VisualisableType>();
                    config.For<IVisualisableTypeWithAssociations>().Use<VisualisableTypeWithAssociations>();
                });
        }
    }
}