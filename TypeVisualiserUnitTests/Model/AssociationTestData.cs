namespace TypeVisualiserUnitTests.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Rhino.Mocks;

    using StructureMap;

    using TypeVisualiser;
    using TypeVisualiser.Model;
    using TypeVisualiser.Model.Persistence;

    public static class AssociationTestData
    {
        private static readonly VisualisableType StringData = new VisualisableType(typeof(string));

        public static TestAssociation AssociationIsolated(ITrivialFilter filter = null)
        {
            return new TestAssociation(StringData, filter).Initialise();
        }

        public static TestAssociation AssociationIsolated(Type type, ITrivialFilter filter)
        {
            return new TestAssociation(new VisualisableType(type), filter).Initialise();
        }

        public static ParentAssociation ParentAssociationIsolated(IModelBuilder mockModelBuilder, IVisualisableTypeWithAssociations mockParentType)
        {
            mockModelBuilder.Expect(m => m.BuildSubject((Type)null, 0)).IgnoreArguments().Return(mockParentType);
            return new ParentAssociation(null, null, mockModelBuilder).Initialise(typeof(int), mockParentType); // Related back to itself, should not matter for testing
        }

        public static ConsumeAssociation ConsumeAssociationFullModel(IContainer factory, Type type, IEnumerable<string> usageDescriptors, int usageCount, IDiagramDimensions mockDimensions = null)
        {
            var target = CreateTestGenericAssociation(factory, mockDimensions, (a, t, m, d) => new ConsumeAssociation(a, t, m, d));
            target.Initialise(type, usageCount, usageDescriptors.Select(x => new AssociationUsageDescriptor { Description = x, Kind = MemberKind.Method }), 0);
            return target;
        }

        private static T CreateTestGenericAssociation<T>(
            IContainer factory, 
            IDiagramDimensions mockDimensions, 
            Func<IApplicationResources, ITrivialFilter, IModelBuilder, IDiagramDimensions, T> ctor) where T : FieldAssociation
        {
            var mockResources = MockRepository.GenerateStub<IApplicationResources>();

            var modelBuilder = new ModelBuilder(factory); // Real model builder.

            if (mockDimensions == null)
            {
                mockDimensions = MockRepository.GenerateStub<IDiagramDimensions>();
            }

            var stubTrivialFilter = MockRepository.GenerateStub<ITrivialFilter>();

            factory.Configure(c =>
            {
                c.For<IApplicationResources>().Use(mockResources);
                c.For<IModelBuilder>().Use<ModelBuilder>();
                c.For<IDiagramDimensions>().Use(mockDimensions);
                c.For<ITrivialFilter>().Use(stubTrivialFilter);
                c.For<IVisualisableTypeWithAssociations>().Use<VisualisableTypeWithAssociations>();
                c.For<IVisualisableType>().Use<VisualisableType>();
            });

            var target = ctor(mockResources, stubTrivialFilter, modelBuilder, mockDimensions);
            PrivateAccessor.SetProperty(target, "ApplicationResources", mockResources);
            PrivateAccessor.SetField<FieldAssociation>(target, "dimensions", mockDimensions);
            return target;
        }

        public static FieldAssociation FieldAssociationFullModel(IContainer factory, Type type, IEnumerable<string> usageDescriptors, int usageCount, IDiagramDimensions mockDimensions = null)
        {
            var target = CreateTestGenericAssociation(factory, mockDimensions, (a, t, m, d) => new FieldAssociation(a, t, m, d));
            target.Initialise(type, usageCount, usageDescriptors.Select(x => new AssociationUsageDescriptor { Description = x, Kind = MemberKind.Field }), 0);
            return target;
        }
    }
}
