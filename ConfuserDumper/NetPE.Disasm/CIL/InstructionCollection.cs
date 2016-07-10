using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace NetPE.Disasm.CIL
{
    public class InstructionCollection : Collection<Instruction>
    {
        public CilWorker CreateWorker()
        {
            return new CilWorker(this);
        }

        public uint ComputeOffset(Instruction inst)
        {
            uint ret = 0;
            IEnumerator<Instruction> e = this.GetEnumerator();
            while (e.MoveNext() && e.Current != inst)
            {
                switch (e.Current.OpCode.OperandType)
                {
                    case OperandType.InlineBrTarget:
                    case OperandType.InlineField:
                    case OperandType.InlineI:
                    case OperandType.InlineMethod:
                    case OperandType.InlineString:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                    case OperandType.ShortInlineR:
                        ret += 4;
                        break;
                    case OperandType.InlineI8:
                    case OperandType.InlineR:
                        ret += 8;
                        break;
                    case OperandType.InlineNone:
                    case OperandType.InlinePhi:
                    case OperandType.InlineSig:
                        break;
                    case OperandType.InlineSwitch:
                        int instLen;
                        if (e.Current.OperandValue == null) // invalid InlineSwitch ?
                            instLen = 0;
                        else
                            instLen = (e.Current.OperandValue as Instruction[]).Length;
                        ret += (uint)(1 + instLen) * 4;
                        break;
                    case OperandType.InlineVar:
                    case OperandType.InlineParam:
                        ret += 2;
                        break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar:
                    case OperandType.ShortInlineParam:
                        ret += 1;
                        break;
                }
                if (e.Current.OpCode.Code < 0xff)
                    ret++;
                else
                    ret += 2;
            }
            return ret;
        }
    }
}
