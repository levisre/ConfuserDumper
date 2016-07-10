using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Disasm.x86.libdasm;

namespace NetPE.Disasm.x86
{
    public class Instruction
    {
        InstructionCollection coll;
        INSTRUCTION inst;
        internal Instruction(InstructionCollection coll, INSTRUCTION inst) { this.coll = coll; this.inst = inst; }

        internal INSTRUCTION InsturctionInternal { get { return inst; } }

        public string Mnemonic
        {
            get { return libdasm.libdasm.get_mnemonic_string(inst, Format.FORMAT_INTEL); }
        }

        public uint Length
        {
            get { return inst.length; }
        }

        public Operand Operand1 { get { if (inst.op1.type == OperandType.OPERAND_TYPE_NONE)return null; else return new Operand(coll, this, inst.op1); } }
        public Operand Operand2 { get { if (inst.op2.type == OperandType.OPERAND_TYPE_NONE)return null; else return new Operand(coll, this, inst.op2); } }
        public Operand Operand3 { get { if (inst.op3.type == OperandType.OPERAND_TYPE_NONE)return null; else return new Operand(coll, this, inst.op3); } }

        public override string ToString()
        {
            return libdasm.libdasm.get_instruction_string(inst, Format.FORMAT_INTEL, coll.ComputeOffset(this));
        }
    }
}
