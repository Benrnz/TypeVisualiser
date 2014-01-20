using System.CodeDom;
using System.IO;

namespace TypeVisualiser.DemoTypes
{
    using System.CodeDom.Compiler;

    [GeneratedCode("Test Code", "1.0")]
    public abstract class Vehicle : System.EventArgs, ICodeParser
    {
        public int NumberOfDoors { get; protected set; }
        public abstract bool SelfTest();
        public Engine Engine { get; protected set; }

        /// <summary>
        /// When implemented in a derived class, compiles the specified text stream into a <see cref="T:System.CodeDom.CodeCompileUnit"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.CodeDom.CodeCompileUnit"/> that contains a representation of the parsed code.
        /// </returns>
        /// <param name="codeStream">A <see cref="T:System.IO.TextReader"/> that can be used to read the code to be compiled. </param>
        public CodeCompileUnit Parse(TextReader codeStream)
        {
            throw new System.NotImplementedException();
        }

        public static string Code
        {
            get { return "Vehicle 0x34862"; }
        }
    }
}