namespace TypeVisualiser.UI
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using Model;
    using Model.Persistence;
    using Properties;
    using WpfUtilities;

    public class UsageDialogController : TypeVisualiserControllerBase
    {
        private IEnumerable<AssociationUsageDescriptor> dataList;

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Reviewed ok here.")]
        public UsageDialogController(string title, string subjectName, string subjectType, FieldAssociation association)
        {
            if (association == null)
            {
                throw new ArgumentNullResourceException("association", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            DefaultButtonCaption = "Close";
            ImageSource = "../Assets/MainIcon.png";
            DialogTitle = title;
            HeadingCaption = BuildHeadingCaption(subjectName, subjectType, association);
            DataList = association.UsageDescriptors;
        }

        public IEnumerable<AssociationUsageDescriptor> DataList
        {
            get
            {
                return this.dataList;
            }

            set
            {
                if (!this.dataList.Equals(value))
                {
                    this.dataList = value;
                    AggregateDuplicates(this.dataList);
                    RaisePropertyChanged("DataList");
                    RaisePropertyChanged("FilteredDataList");
                }
            }
        }

        public string DefaultButtonCaption { get; set; }
        public string DialogTitle { get; set; }
        public IEnumerable<AssociationUsageDescriptor> FilteredDataList { get; set; }

        public string HeadingCaption { get; set; }
        public string ImageSource { get; set; }

        public static string BuildHeadingCaption(string subjectName, string subjectType, FieldAssociation association)
        {
            if (association == null)
            {
                throw new ArgumentNullResourceException("association", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            if (association is ConsumeAssociation)
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "The following methods in the {0} {1} consume the {2} {3}:",
                    subjectName,
                    subjectType,
                    association.AssociatedTo.Name,
                    association.AssociatedTo.Modifiers.TypeTextName);
            }

            if (association.UsageDescriptors.Any(x => x.Kind == MemberKind.Method))
            {
                return string.Format(
                    CultureInfo.CurrentCulture,
                    "{0} fields in the {1} {2} and other methods that consume the {0} {3}:",
                    association.AssociatedTo.Name,
                    subjectName,
                    subjectType,
                    association.AssociatedTo.Modifiers.TypeTextName);
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                "{0} fields in the {1} {2}:",
                association.AssociatedTo.Name,
                subjectName,
                subjectType);
        }

        private void AggregateDuplicates(IEnumerable<AssociationUsageDescriptor> list)
        {
            IEnumerable<AssociationUsageDescriptor> query = from item in list
                                                            group item by item
                                                            into grouped
                                                            let count = grouped.Count()
                                                            select new AssociationUsageDescriptor
                                                            {
                                                                Description = string.Format(CultureInfo.CurrentCulture, "{0} {1}", grouped.Key.Description, count > 1 ? "x" + count : string.Empty),
                                                                Kind = grouped.Key.Kind,
                                                            };
            FilteredDataList = query.ToList();
        }
    }
}