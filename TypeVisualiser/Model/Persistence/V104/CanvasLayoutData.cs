namespace TypeVisualiser.Model.Persistence.V104
{
    using System;
    using System.Collections.ObjectModel;

    [Serializable]
    public class CanvasLayoutData
    {
        public CanvasLayoutData()
        {
            Types = new Collection<TypeLayoutData>();
            Annotations = new Collection<AnnotationData>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "It must be settable for serialisation")]
        public Collection<AnnotationData> Annotations { get; set; }

        public TypeLayoutData Subject { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification="It must be settable for serialisation")]
        public Collection<TypeLayoutData> Types { get; set; }
    }
}