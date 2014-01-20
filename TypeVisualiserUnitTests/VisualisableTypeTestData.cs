namespace TypeVisualiserUnitTests
{
    using System;

    using Rhino.Mocks;

    using StructureMap;

    using TypeVisualiser;
    using TypeVisualiser.Model;

    public static class VisualisableTypeTestData
    {
        public static IVisualisableTypeWithAssociations FullModel(Type typeToBuild, IContainer factory, ITrivialFilter filter = null)
        {
            var resources = MockRepository.GenerateStub<IApplicationResources>();

            if (filter == null)
            {
                filter = factory.TryGetInstance<ITrivialFilter>() ?? MockRepository.GenerateStub<ITrivialFilter>();
            }

            var stubDimensions = MockRepository.GenerateStub<IDiagramDimensions>();

            factory.Configure(c =>
            {
                c.For<IModelBuilder>().Use<ModelBuilder>();
                c.For<IVisualisableTypeWithAssociations>().Use<VisualisableTypeWithAssociations>();
                c.For<IApplicationResources>().Use(resources);
                c.For<IDiagramDimensions>().Use(stubDimensions);
                c.For<ITrivialFilter>().Use(filter);
            });

            var demoType = new VisualisableTypeWithAssociations(typeToBuild, 0, factory);
            return demoType;
        }

        public static IVisualisableTypeWithAssociations FullModel<T>(IContainer factory, ITrivialFilter filter = null)
        {
            return FullModel(typeof(T), factory, filter);
        }
    }
}
