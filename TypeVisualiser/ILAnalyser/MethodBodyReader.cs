namespace TypeVisualiser.ILAnalyser
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using TypeVisualiser.Properties;

    internal class MethodBodyReader : IMethodBodyReader
    {
        static MethodBodyReader()
        {
            GlobalIntermediateLanguageConstants.LoadOpCodes();
        }

        private readonly Collection<ILInstruction> instructions = new Collection<ILInstruction>();

        private byte[] il;

        private MethodBase mi;

        /// <summary>
        /// Gets the collection of IL instructions.
        /// </summary>
        /// <value>The instructions.</value>
        public IQueryable<ILInstruction> Instructions
        {
            get
            {
                return this.instructions.AsQueryable();
            }
        }

        public void Read(MethodBase method)
        {
            if (method == null)
            {
                throw new ArgumentNullResourceException("method", Resources.General_Given_Parameter_Cannot_Be_Null);
            }

            this.mi = method;
            MethodBody body = this.mi.GetMethodBody();
            if (body != null)
            {
                this.il = body.GetILAsByteArray();
                ConstructInstructions(this.mi.Module);
            }
        }

        ///// <summary>
        ///// Gets the IL code of the method
        ///// </summary>
        ///// <returns></returns>
        // public string GetBodyCode()
        // {
        // string result = "";
        // if (this.Instructions != null)
        // {
        // for (int i = 0; i < this.Instructions.Count; i++)
        // {
        // result += this.Instructions[i].GetCode() + "\n";
        // }
        // }
        // return result;
        // }

        // public object GetRefferencedOperand(Module module, int metadataToken)
        // {
        // AssemblyName[] assemblyNames = module.Assembly.GetReferencedAssemblies();
        // for (int i = 0; i < assemblyNames.Length; i++)
        // {
        // Module[] modules = Assembly.Load(assemblyNames[i]).GetModules();
        // for (int j = 0; j < modules.Length; j++)
        // {
        // try
        // {
        // Type t = modules[j].ResolveType(metadataToken);
        // return t;
        // } catch
        // {
        // }
        // }
        // }
        // return null;
        // //System.Reflection.Assembly.Load(module.Assembly.GetReferencedAssemblies()[3]).GetModules()[0].ResolveType(metadataToken)
        // }

        /// <summary>
        /// Constructs the array of ILInstructions according to the IL byte code.
        /// </summary>
        /// <param name="module">
        /// </param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Third party code needs to be as compatible as possible to allow upgrading")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "For simple analysis purposes, this is not an app critical function")]
        private void ConstructInstructions(Module module)
        {
            byte[] localIlbytes = this.il;
            int position = 0;
            while (position < localIlbytes.Length)
            {
                var instruction = new ILInstruction();

                // get the operation code of the current instruction
                OpCode code;
                ushort value = localIlbytes[position++];
                if (GlobalIntermediateLanguageConstants.SingleByteOpCodes.Count == 0)
                {
                    throw new InvalidOperationException(
                        "Attempt to use Method Body Reader before Global Intermediate Language Constants has been initialised. Global Intermediate Language Constants. Load Op Codes must be called once.");
                }

                if (value != 0xfe)
                {
                    code = GlobalIntermediateLanguageConstants.SingleByteOpCodes[value];
                }
                else
                {
                    value = localIlbytes[position++];
                    code = GlobalIntermediateLanguageConstants.MultiByteOpCodes[value];
                }

                instruction.Code = code;
                instruction.Offset = position - 1;
                int metadataToken;

                // get the operand of the current operation
                switch (code.OperandType)
                {
                    case OperandType.InlineBrTarget:
                        metadataToken = ReadInt32(ref position);
                        metadataToken += position;
                        instruction.Operand = metadataToken;
                        break;
                    case OperandType.InlineField:

                        // TODO All these try catch blocks need to go
                        try
                        {
                            metadataToken = ReadInt32(ref position);
                            instruction.Operand = module.ResolveField(metadataToken);
                        }
                        catch
                        {
                            instruction.Operand = new object();
                        }

                        break;
                    case OperandType.InlineMethod:
                        metadataToken = ReadInt32(ref position);
                        try
                        {
                            instruction.Operand = module.ResolveMethod(metadataToken);
                        }
                        catch
                        {
                            instruction.Operand = new object();
                        }

                        break;
                    case OperandType.InlineSig:
                        metadataToken = ReadInt32(ref position);
                        instruction.Operand = module.ResolveSignature(metadataToken);
                        break;
                    case OperandType.InlineTok:
                        metadataToken = ReadInt32(ref position);
                        try
                        {
                            instruction.Operand = module.ResolveType(metadataToken);
                        }
                        catch
                        {
                        }

                        // TODO : see what to do here
                        break;
                    case OperandType.InlineType:
                        metadataToken = ReadInt32(ref position);

                        // now we call the ResolveType always using the generic attributes type in order
                        // to support decompilation of generic methods and classes
                        // thanks to the guys from code project who commented on this missing feature
                        Type[] declaringTypeGenericArgs = this.mi.DeclaringType.GetGenericArguments();
                        Type[] genericArgs = null;
                        if (this.mi.IsGenericMethod)
                        {
                            genericArgs = this.mi.GetGenericArguments();
                        }

                        instruction.Operand = module.ResolveType(metadataToken, declaringTypeGenericArgs, genericArgs);
                        break;
                    case OperandType.InlineI:
                        {
                            instruction.Operand = ReadInt32(ref position);
                            break;
                        }

                    case OperandType.InlineI8:
                        {
                            instruction.Operand = ReadInt64(ref position);
                            break;
                        }

                    case OperandType.InlineNone:
                        {
                            instruction.Operand = null;
                            break;
                        }

                    case OperandType.InlineR:
                        {
                            instruction.Operand = ReadDouble(ref position);
                            break;
                        }

                    case OperandType.InlineString:
                        {
                            metadataToken = ReadInt32(ref position);
                            instruction.Operand = module.ResolveString(metadataToken);
                            break;
                        }

                    case OperandType.InlineSwitch:
                        {
                            int count = ReadInt32(ref position);
                            var casesAddresses = new int[count];
                            for (int i = 0; i < count; i++)
                            {
                                casesAddresses[i] = ReadInt32(ref position);
                            }

                            var cases = new int[count];
                            for (int i = 0; i < count; i++)
                            {
                                cases[i] = position + casesAddresses[i];
                            }

                            break;
                        }

                    case OperandType.InlineVar:
                        {
                            instruction.Operand = ReadUInt16(ref position);
                            break;
                        }

                    case OperandType.ShortInlineBrTarget:
                        {
                            instruction.Operand = ReadSByte(ref position) + position;
                            break;
                        }

                    case OperandType.ShortInlineI:
                        {
                            instruction.Operand = ReadSByte(ref position);
                            break;
                        }

                    case OperandType.ShortInlineR:
                        {
                            instruction.Operand = ReadSingle(ref position);
                            break;
                        }

                    case OperandType.ShortInlineVar:
                        {
                            instruction.Operand = ReadByte(ref position);
                            break;
                        }

                    default:
                        {
                            throw new NotSupportedException("Unknown operand type.");
                        }
                }

                this.instructions.Add(instruction);
            }
        }

        private byte ReadByte(ref int position)
        {
            return this.il[position++];
        }

        private double ReadDouble(ref int position)
        {
            return ((this.il[position++] | (this.il[position++] << 8)) | (this.il[position++] << 0x10)) | (this.il[position++] << 0x18) | (this.il[position++] << 0x20) | (this.il[position++] << 0x28)
                   | (this.il[position++] << 0x30) | (this.il[position++] << 0x38);
        }

        //// private int ReadInt16(byte[] _il, ref int position)
        //// {
        //// return ((this.il[position++] | (this.il[position++] << 8)));
        //// }
        private int ReadInt32(ref int position)
        {
            return ((this.il[position++] | (this.il[position++] << 8)) | (this.il[position++] << 0x10)) | (this.il[position++] << 0x18);
        }

        private ulong ReadInt64(ref int position)
        {
            return
                (ulong)
                (((this.il[position++] | (this.il[position++] << 8)) | (this.il[position++] << 0x10)) | (this.il[position++] << 0x18) | (this.il[position++] << 0x20) | (this.il[position++] << 0x28)
                 | (this.il[position++] << 0x30) | (this.il[position++] << 0x38));
        }

        private sbyte ReadSByte(ref int position)
        {
            return (sbyte)this.il[position++];
        }

        private float ReadSingle(ref int position)
        {
            return ((this.il[position++] | (this.il[position++] << 8)) | (this.il[position++] << 0x10)) | (this.il[position++] << 0x18);
        }

        private ushort ReadUInt16(ref int position)
        {
            return (ushort)(this.il[position++] | (this.il[position++] << 8));
        }
    }
}