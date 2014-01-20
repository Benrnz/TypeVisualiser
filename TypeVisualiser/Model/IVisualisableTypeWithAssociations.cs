using System.Collections.Generic;

namespace TypeVisualiser.Model
{
    public interface IVisualisableTypeWithAssociations : IVisualisableType
    {
        /// <summary>
        /// Gets all associations. This is an unfiltered list of all associations. It does not take into account the 
        /// <see cref="TrivialFilter"/>. IT ALSO DOES NOT INCLUDE THE SUPERCLASS OR INTERFACES.
        /// </summary>
        /// <value>All associations.</value>
        IEnumerable<FieldAssociation> AllAssociations { get; }

        /// <summary>
        /// Gets the consumes associations. This is a subset of <see cref="FilteredAssociations"/>. 
        /// This collection takes into account the <see cref="TrivialFilter"/>.
        /// This collection includes the sub-classes of <see cref="ConsumeAssociation"/>, such as
        /// <see cref="StaticAssociation"/>.
        /// </summary>
        /// <value>The consume associations.</value>
        IEnumerable<ConsumeAssociation> Consumes { get; }

        /// <summary>
        /// Gets just the field associations. This is a subset of <see cref="FilteredAssociations"/>. 
        /// This collection takes into account the <see cref="TrivialFilter"/>.
        /// This collection does NOT include the sub-classes of <see cref="FieldAssociation"/>.
        /// </summary>
        /// <value>The field associations.</value>
        IEnumerable<FieldAssociation> Fields { get; }

        /// <summary>
        /// Gets all associations while taking into account the <see cref="TrivialFilter"/>.
        /// This collection includes the sub-classes of <see cref="FieldAssociation"/>, such as
        /// <see cref="ConsumeAssociation"/> and others.
        /// </summary>
        /// <value>All associations.</value>
        IEnumerable<FieldAssociation> FilteredAssociations { get; }

        /// <summary>
        /// Gets the number of nontrivial dependencies. This is the number of associations that are
        /// classified as non-trivial by the <see cref="TrivialFilter"/> and associates that are interfaces
        /// enums or value types.
        /// </summary>
        /// <value>The nontrivial dependencies.</value>
        int NontrivialDependencies { get; }

        /// <summary>
        /// Gets the parent of this subject.
        /// </summary>
        /// <value>The parent.</value>
        ParentAssociation Parent { get; }

        /// <summary>
        /// Gets all the interfaces this subject implements. This list is not affected by
        /// the <see cref="TrivialFilter"/>.
        /// </summary>
        /// <value>The interfaces this subject implements.</value>
        IEnumerable<ParentAssociation> ThisTypeImplements { get; }

        /// <summary>
        /// Gets the number of nontrivial dependencies. This is the number of associations that are
        /// classified as trivial by the <see cref="TrivialFilter"/> and associates that are interfaces
        /// enums or value types.
        /// </summary>
        /// <value>The nontrivial dependencies.</value>
        int TrivialDependencies { get; }

        /// <summary>
        /// This method is called when an instance is reused from the cache. 
        /// </summary>
        void InitialiseForReuseFromCache(int newDepth);

        /// <summary>
        /// Discovers the relationships between associations. This is an interim step to find only associations that are not already modeled.
        /// THis should only be called on the main subject of the diagram. Call this for multiple types will result in unnecessary duplicated processing.
        /// Ie: Subject will be excluced and its associations also.
        /// </summary>
        void DiscoverSecondaryAssociationsInModel();
    }
}