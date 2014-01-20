using System;

namespace TypeVisualiser.DemoTypes
{
    using System.CodeDom.Compiler;

    [GeneratedCode("Test Code", "1.0")]
    public class CombustionEngine : Engine, ICloneable 
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            StaticTestClass.DoWork();
            throw new NotImplementedException();
        }
    }
}