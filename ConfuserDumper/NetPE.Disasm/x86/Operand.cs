using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Disasm.x86.libdasm;

namespace NetPE.Disasm.x86
{
   public class Operand
   {
       InstructionCollection coll;
       Instruction inst;
       OPERAND op;
       internal Operand(InstructionCollection coll, Instruction inst, OPERAND op) { this.coll = coll; this.op = op; this.inst = inst; }

       public override string ToString()
       {
           return libdasm.libdasm.get_operand_string(inst.InsturctionInternal, op, Format.FORMAT_INTEL, coll.ComputeOffset(inst));
       }
    }
}
