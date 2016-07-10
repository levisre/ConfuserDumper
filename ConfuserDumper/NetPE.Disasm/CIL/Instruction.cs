using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetPE.Disasm.CIL
{
    public class Instruction
    {
        internal Instruction(InstructionCollection coll, OpCode c) { this.coll = coll; this.c = c; }
        internal Instruction(InstructionCollection coll, OpCode c, Operand op) { this.coll = coll; this.c = c; this.op = op; }

        InstructionCollection coll;
        public InstructionCollection Container { get { return coll; } }

        OpCode c;
        public OpCode OpCode { get { return c; } }

        Operand op;
        public Operand Operand { get { return op; } internal set { op = value; } }

        /// <summary>
        /// Check if Operand is null and get its value
        /// </summary>
        public object OperandValue
        {
            get
            {
                if (op == null)
                    return null;
                return op.Value;
            }
        }

        public override string ToString()
        {
            if (op == null)
                return c.Mnemonic;
            return c.Mnemonic + " " + op.ToString();
        }

        public string GetOffsetString()
        {
            return "L_" + coll.ComputeOffset(this).ToString("x4").ToUpper();
        }
    }
}
