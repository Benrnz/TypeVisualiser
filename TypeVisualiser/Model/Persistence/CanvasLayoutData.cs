namespace TypeVisualiser.Model.Persistence
{
    using System;
    using System.Collections.ObjectModel;

    [Serializable]
    public class CanvasLayoutData
    {
        public CanvasLayoutData()
        {
            Types = new Collection<TypeLayoutData>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification="It must be settable for serialisation")]
        public Collection<TypeLayoutData> Types { get; set; }
    }
}