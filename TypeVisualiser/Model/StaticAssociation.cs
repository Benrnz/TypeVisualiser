namespace TypeVisualiser.Model
{
    using System;
    using System.Collections.Generic;
    using Persistence;

    public class StaticAssociation : ConsumeAssociation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification="validated in base class")]
        public StaticAssociation(Type type, int numberOfUsages, IEnumerable<AssociationUsageDescriptor> usageDescriptors, int depth)
            : base(type, numberOfUsages, usageDescriptors, depth)
        {
        }

        internal override Type PersistenceType
        {
            get
            {
                return typeof(StaticAssociationData);
            }
        }
    }
}