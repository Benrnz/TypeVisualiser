using System;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using TypeVisualiser.Model.Persistence;
using TypeVisualiser.UI;

namespace TypeVisualiser.Model
{
    public interface IDiagramController : ICleanup, INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when the diagram has finished loading and positioning all diagram elements.
        /// </summary>
        event EventHandler DiagramLoaded;

        /// <summary>
        /// Raised when data bound objects have changed that affect the overall size of the diagram.
        /// This signals the view to recalculate the canvas size.
        /// </summary>
        event EventHandler ExpandCanvasRequested;

        /// <summary>
        /// Gets the diagram caption to appear in the diagram tab.
        /// </summary>
        string DiagramCaption { get; }

        /// <summary>
        /// Gets the full name of the diagram to appear as the tool tip for the diagram tab.
        /// </summary>
        /// <value>
        /// The full name of the diagram.
        /// </value>
        string DiagramFullName { get; }

        /// <summary>
        /// A unique identifier for this diagram instance.
        /// </summary>
        Guid DiagramId { get; set; }

        bool HideBackground { get; set; }

        /// <summary>
        /// Activates the diagram after it has been in the background of the tab control and now has been selected by the user.
        /// </summary>
        void ActivateDiagram();

        /// <summary>
        /// Assigns the diagram data.
        /// This is the main entry point to display something into the diagram visual tree.
        /// </summary>
        /// <param name="bindableDiagramData">The data to load into the diagram visual.</param>
        void AssignDiagramData(object bindableDiagramData);

        /// <summary>
        /// Clears all diagram visual elements.
        /// </summary>
        void Clear();

        /// <summary>
        /// Requests the controller to gather persistent data from the diagram so it can be saved to a disk file.
        /// </summary>
        IPersistentFileData GatherPersistentData();

        /// <summary>
        /// Loads the diagram data from file after the subject has already been set by <see cref="ViewportController.AssignDiagramData"/>.
        /// At this stage the diagram has already been loaded and is displayed on screen.  
        /// This method ensures all diagram elements have been positioned where the file specifies their co-ordinates.
        /// Also will provide feedback on any mismatches, ie the type has changed and there are more or less associations.
        /// </summary>
        /// <param name="deserializedDataFile">The deserialised data file.</param>
        void LoadDiagramDataFromFile(object deserializedDataFile);

        /// <summary>
        /// Reloads the current diagram.
        /// </summary>
        /// <param name="fileManager">The file manager.</param>
        void Refresh(IFileManager fileManager);

        /// <summary>
        /// Gives a reference of the diagram container to the controller. This must be called prior to loading any content in the diagram.
        /// Should not be used externally, the diagram constructor must be the only caller of this.
        /// </summary>
        /// <param name="diagram">The new host diagram.</param>
        void SetHostDiagram(Diagram diagram);
    }
}