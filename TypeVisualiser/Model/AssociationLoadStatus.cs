namespace TypeVisualiser.Model
{
    /// <summary>
    /// This is the initialise status of a <see cref="VisualisableTypeWithAssociations"/>.
    /// </summary>
    internal enum AssociationLoadStatus
    {
        /// <summary>
        /// The type has only been constructed and not initialised. This indicates the constructor call chain is still running.
        /// </summary>
        ConstructedOnly,

        /// <summary>
        /// The type is fully initialised all association collections are populated.
        /// </summary>
        FullyLoaded,

        /// <summary>
        /// The type has not been initialised and its association collections are not populated.
        /// </summary>
        NotLoaded,
    }
}