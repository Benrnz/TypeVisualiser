namespace TypeVisualiser.ILAnalyser
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using TypeVisualiser.Properties;

    internal static class GlobalIntermediateLanguageConstants
    {
        private static readonly object SyncRoot = new object();

        private static bool isInitialised;

        private static ReadOnlyDictionary<int, OpCode> multiByteOpCodes = new ReadOnlyDictionary<int, OpCode>();

        private static ReadOnlyDictionary<int, OpCode> singleByteOpCodes = new ReadOnlyDictionary<int, OpCode>();

        public static ReadOnlyDictionary<int, OpCode> MultiByteOpCodes
        {
            get
            {
                return multiByteOpCodes;
            }
        }

        public static ReadOnlyDictionary<int, OpCode> SingleByteOpCodes
        {
            get
            {
                return singleByteOpCodes;
            }
        }

        internal static void LoadOpCodes()
        {
            if (isInitialised)
            {
                return;
            }

            lock (SyncRoot)
            {
                if (isInitialised)
                {
                    return;
                }

                var multiByteOps = new Dictionary<int, OpCode>();
                var singleByteOps = new Dictionary<int, OpCode>();
                isInitialised = true;
                FieldInfo[] infoArray1 = typeof(OpCodes).GetFields();
                foreach (FieldInfo info1 in infoArray1)
                {
                    if (info1.FieldType == typeof(OpCode))
                    {
                        var code1 = (OpCode)info1.GetValue(null);
                        var num2 = (ushort)code1.Value;
                        if (num2 < 0x100)
                        {
                            singleByteOps[num2] = code1;
                        }
                        else
                        {
                            if ((num2 & 0xff00) != 0xfe00)
                            {
                                throw new InvalidOperationException(Resources.Globals_LoadOpCodes_Invalid_IL_Op_Code);
                            }

                            multiByteOps[num2 & 0xff] = code1;
                        }
                    }
                }

                multiByteOpCodes = new ReadOnlyDictionary<int, OpCode>(multiByteOps);
                singleByteOpCodes = new ReadOnlyDictionary<int, OpCode>(singleByteOps);
            }
        }

        // ***BR 22-2-12 not needed right now
        ///// <summary>
        ///// Retrieve the friendly name of a type
        ///// </summary>
        ///// <param name="typeName">
        ///// The complete name to the type
        ///// </param>
        ///// <returns>
        ///// The simplified name of the type (i.e. "int" instead f System.Int32)
        ///// </returns>
        // internal static string ProcessSpecialTypes(string typeName)
        // {
        // string result = typeName;
        // switch (typeName)
        // {
        // case "System.string":
        // case "System.String":
        // case "String":
        // result = "string";
        // break;
        // case "System.Int32":
        // case "Int":
        // case "Int32":
        // result = "int";
        // break;
        // case "System.Int64":
        // case "long":
        // result = "long";
        // break;
        // }
        // return result;
        // }
    }
}