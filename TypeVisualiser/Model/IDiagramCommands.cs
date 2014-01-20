namespace TypeVisualiser.Model
{
    /// <summary>
    /// This is a temporary hack to be able to tell the view controller to execute commands.
    /// The menu system needs a refactor to allow menus to be customised by the currently displayed diagram.
    /// These customised commands could queried and obtained from the controller complete with delegate callbacks
    /// for execution.
    /// </summary>
    public interface IDiagramCommandsNeedsRefactor
    {
        void HideTrivialTypes();

        void HideSystemTypes();

        void HideSecondaryAssociations();

        void DrawConnectingLines();
    }
}
