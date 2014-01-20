namespace TypeVisualiser.ILAnalyser
{
    using System.Reflection.Emit;

// ReSharper disable InconsistentNaming
    public class ILInstruction
// ReSharper restore InconsistentNaming
    {
        public OpCode Code { get; set; }

        public int Offset { get; set; }

        public object Operand { get; set; }

        //// The following code is not being consumed in this project.
        //// public byte[] OperandData { get; set; }
        /////// <summary>
        /////// Returns a friendly string representation of this instruction
        /////// </summary>
        /////// <returns></returns>
        ////public string GetCode()
        ////{
        ////    string result = "";
        ////    result += this.GetExpandedOffset(this.offset) + " : " + this.code;
        ////    if (this.operand != null)
        ////    {
        ////        switch (this.code.OperandType)
        ////        {
        ////            case OperandType.InlineField:
        ////                var fOperand = ((FieldInfo)this.operand);
        ////                result += " " + GlobalIntermediateLanguageConstants.ProcessSpecialTypes(fOperand.FieldType.ToString()) + " " +
        ////                          GlobalIntermediateLanguageConstants.ProcessSpecialTypes(fOperand.ReflectedType.ToString()) +
        ////                          "::" + fOperand.Name + "";
        ////                break;
        ////            case OperandType.InlineMethod:
        ////                try
        ////                {
        ////                    var mOperand = (MethodInfo)this.operand;
        ////                    result += " ";
        ////                    if (!mOperand.IsStatic)
        ////                    {
        ////                        result += "instance ";
        ////                    }
        ////                    result += GlobalIntermediateLanguageConstants.ProcessSpecialTypes(mOperand.ReturnType.ToString()) +
        ////                              " " + GlobalIntermediateLanguageConstants.ProcessSpecialTypes(mOperand.ReflectedType.ToString()) +
        ////                              "::" + mOperand.Name + "()";
        ////                } catch
        ////                {
        ////                    try
        ////                    {
        ////                        var mOperand = (ConstructorInfo)this.operand;
        ////                        result += " ";
        ////                        if (!mOperand.IsStatic)
        ////                        {
        ////                            result += "instance ";
        ////                        }
        ////                        result += "void " +
        ////                                  GlobalIntermediateLanguageConstants.ProcessSpecialTypes(mOperand.ReflectedType.ToString()) +
        ////                                  "::" + mOperand.Name + "()";
        ////                    } catch
        ////                    {
        ////                    }
        ////                }
        ////                break;
        ////            case OperandType.ShortInlineBrTarget:
        ////            case OperandType.InlineBrTarget:
        ////                result += " " + this.GetExpandedOffset((int)this.operand);
        ////                break;
        ////            case OperandType.InlineType:
        ////                result += " " + GlobalIntermediateLanguageConstants.ProcessSpecialTypes(this.operand.ToString());
        ////                break;
        ////            case OperandType.InlineString:
        ////                if (this.operand.ToString() == "\r\n")
        ////                {
        ////                    result += " \"\\r\\n\"";
        ////                } else
        ////                {
        ////                    result += " \"" + this.operand + "\"";
        ////                }
        ////                break;
        ////            case OperandType.ShortInlineVar:
        ////                result += this.operand.ToString();
        ////                break;
        ////            case OperandType.InlineI:
        ////            case OperandType.InlineI8:
        ////            case OperandType.InlineR:
        ////            case OperandType.ShortInlineI:
        ////            case OperandType.ShortInlineR:
        ////                result += this.operand.ToString();
        ////                break;
        ////            case OperandType.InlineTok:
        ////                if (this.operand is Type)
        ////                {
        ////                    result += ((Type)this.operand).FullName;
        ////                } else
        ////                {
        ////                    result += "not supported";
        ////                }
        ////                break;

        ////            default:
        ////                result += "not supported";
        ////                break;
        ////        }
        ////    }
        ////    return result;
        ////}

        /////// <summary>
        /////// Add enough zeros to a number as to be represented on 4 characters
        /////// </summary>
        /////// <param name="offset">
        /////// The number that must be represented on 4 characters
        /////// </param>
        /////// <returns>
        /////// </returns>
        ////private string GetExpandedOffset(long offset)
        ////{
        ////    string result = offset.ToString();
        ////    for (int i = 0; result.Length < 4; i++)
        ////    {
        ////        result = "0" + result;
        ////    }
        ////    return result;
        ////}
    }
}