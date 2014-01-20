namespace TypeVisualiser.ILAnalyser
{
    using System.Linq;
    using System.Reflection;

    public interface IMethodBodyReader
    {
        /// <summary>
        /// Gets the collection of IL instructions.
        /// </summary>
        /// <value>The instructions.</value>
        IQueryable<ILInstruction> Instructions { get; }

        void Read(MethodBase method);
    }
}