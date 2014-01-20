using System;
using StructureMap;
using TypeVisualiser.Model;
using TypeVisualiser.Model.Persistence;

namespace TypeVisualiserUnitTests.Model.VisualisableTypeSubjectTests
{
    public class VisualisableTypeWithAssociationsTestHarness : VisualisableTypeWithAssociations 
    {
        public VisualisableTypeWithAssociationsTestHarness(Type type) : this(type, 0)
        {
        }

        public VisualisableTypeWithAssociationsTestHarness(Type type, int depth, IContainer factory)
            : base(type, depth, factory)
        {
        }

        public VisualisableTypeWithAssociationsTestHarness(Type type, int depth)
            : base(type, depth)
        {
        }

        protected override void InitialiseParentTypeRelationship(Type mainSubjectType)
        {
            // do not create parent relationships.
        }
    }
}
