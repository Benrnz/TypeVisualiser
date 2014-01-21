using TypeVisualiser.Properties;

namespace TypeVisualiser.Model.Persistence
{
    public class ParentAssociationData : AssociationData
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Must only be a Parent Association, nothing else is allowed.")]
        public static ParentAssociationData Convert(ParentAssociation parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullResourceException("parent", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            return new ParentAssociationData { Name = parent.Name, AssociatedTo = parent.AssociatedTo.ExtractPersistentData(), };
        }
    }
}