namespace TypeVisualiser.Model
{
    using System;
    using System.Collections.Generic;

    using TypeVisualiser.Model.Persistence;

    public class StaticAssociation : ConsumeAssociation
    {
        public StaticAssociation(IApplicationResources resources, ITrivialFilter trivialFilter, IModelBuilder modelBuilder, IDiagramDimensions diagramDimensions)
            : base(resources, trivialFilter, modelBuilder, diagramDimensions)
        {
        }

        internal override Type PersistenceType
        {
            get
            {
                return typeof(StaticAssociationData);
            }
        }

        public new StaticAssociation Initialise(Type associatedTo, int numberOfUsages, IEnumerable<AssociationUsageDescriptor> usageDescriptors, int depth)
        {
            return base.Initialise(associatedTo, numberOfUsages, usageDescriptors, depth) as StaticAssociation;
        }
    }
}