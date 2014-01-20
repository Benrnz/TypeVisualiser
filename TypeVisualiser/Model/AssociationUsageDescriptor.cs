namespace TypeVisualiser.Model
{
    using Persistence;

    /// <summary>
    /// A class that describes a single usage relationship between the subject and the association.
    /// For example: If subject is Car and it has a Field engine, then the description will refer to the field name 'engine' and the 
    /// kind will be set to Field. Each association will have 1 or more of these.
    /// </summary>
    public class AssociationUsageDescriptor
    {
        public string Description { get; set; }

        public MemberKind Kind { get; set; }

        public static AssociationUsageDescriptor CreateFieldUsage(string description)
        {
            return new AssociationUsageDescriptor { Description = description, Kind = MemberKind.Field };
        }

        public static AssociationUsageDescriptor CreateMethodUsage(string description)
        {
            return new AssociationUsageDescriptor { Description = description, Kind = MemberKind.Method };
        }

        public override int GetHashCode()
        {
            return Description.GetHashCode();
        }

        private string DescriptionForEqualityPurposes
        {
            get
            {
                return Description + Kind;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var otherDescriptor = obj as AssociationUsageDescriptor;
            if (otherDescriptor == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return DescriptionForEqualityPurposes.Equals(otherDescriptor.DescriptionForEqualityPurposes);
        }
    }
}
