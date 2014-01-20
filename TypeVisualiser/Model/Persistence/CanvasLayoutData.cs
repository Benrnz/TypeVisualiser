namespace TypeVisualiser.Model.Persistence
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;

    [Serializable]
    public class CanvasLayoutData
    {
        public CanvasLayoutData()
        {
            this.Types = new Collection<TypeLayoutData>();
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "It must be settable for serialisation")]
        public Collection<TypeLayoutData> Types { get; set; }
    }
}