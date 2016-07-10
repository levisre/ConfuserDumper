using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Core.Metadata;

namespace NetPE.Disasm.CIL
{
    public class CilWorker
    {
        InstructionCollection coll;
        internal CilWorker(InstructionCollection coll) { this.coll = coll; }

        public Instruction Create(OpCode op, byte num)
        {
            return new Instruction(coll, op, new Operand(num));
        }
        public Instruction Create(OpCode op, sbyte num)
        {
            return new Instruction(coll, op, new Operand(num));
        }

        public Instruction Create(OpCode op, short num)
        {
            return new Instruction(coll, op, new Operand(num));
        }
        public Instruction Create(OpCode op, ushort num)
        {
            return new Instruction(coll, op, new Operand(num));
        }

        public Instruction Create(OpCode op, int num)
        {
            return new Instruction(coll, op, new Operand(num));
        }
        public Instruction Create(OpCode op, uint num)
        {
            return new Instruction(coll, op, new Operand(num));
        }

        public Instruction Create(OpCode op, long num)
        {
            return new Instruction(coll, op, new Operand(num));
        }
        public Instruction Create(OpCode op, ulong num)
        {
            return new Instruction(coll, op, new Operand(num));
        }

        public Instruction Create(OpCode op, float num)
        {
            return new Instruction(coll, op, new Operand(num));
        }
        public Instruction Create(OpCode op, double num)
        {
            return new Instruction(coll, op, new Operand(num));
        }

        public Instruction Create(OpCode op, MetadataToken tkn)
        {
            return new Instruction(coll, op, new Operand(tkn));
        }

        public Instruction Create(OpCode op, Instruction inst)
        {
            return new Instruction(coll, op, new Operand(inst));
        }

        public Instruction Create(OpCode op, Instruction[] insts)
        {
            return new Instruction(coll, op, new Operand(insts));
        }

        public void Emit(OpCode op, byte num)
        {
            coll.Add(new Instruction(coll, op, new Operand(num)));
        }
        public void Emit(OpCode op, sbyte num)
        {
            coll.Add(new Instruction(coll, op, new Operand(num)));
        }

        public void Emit(OpCode op, short num)
        {
            coll.Add(new Instruction(coll, op, new Operand(num)));
        }
        public void Emit(OpCode op, ushort num)
        {
            coll.Add(new Instruction(coll, op, new Operand(num)));
        }

        public void Emit(OpCode op, int num)
        {
            coll.Add(new Instruction(coll, op, new Operand(num)));
        }
        public void Emit(OpCode op, uint num)
        {
            coll.Add(new Instruction(coll, op, new Operand(num)));
        }

        public void Emit(OpCode op, long num)
        {
            coll.Add(new Instruction(coll, op, new Operand(num)));
        }
        public void Emit(OpCode op, ulong num)
        {
            coll.Add(new Instruction(coll, op, new Operand(num)));
        }

        public void Emit(OpCode op, float num)
        {
            coll.Add(new Instruction(coll, op, new Operand(num)));
        }
        public void Emit(OpCode op, double num)
        {
            coll.Add(new Instruction(coll, op, new Operand(num)));
        }

        public void Emit(OpCode op, MetadataToken tkn)
        {
            coll.Add(new Instruction(coll, op, new Operand(tkn)));
        }

        public void Emit(OpCode op, Instruction inst)
        {
            coll.Add(new Instruction(coll, op, new Operand(inst)));
        }

        public void Emit(OpCode op, Instruction[] insts)
        {
            coll.Add(new Instruction(coll, op, new Operand(insts)));
        }

        public void Append(Instruction inst)
        {
            coll.Add(inst);
        }

        public void InsertAfter(Instruction inst, Instruction target)
        {
            coll.Insert(coll.IndexOf(target) + 1, inst);
        }

        public void InsertBefore(Instruction inst, Instruction target)
        {
            coll.Insert(coll.IndexOf(target), inst);
        }

        public void Replace(Instruction inst, Instruction target)
        {
            coll[coll.IndexOf(target)] = inst;
        }

        public void Remove(Instruction inst)
        {
            coll.Remove(inst);
        }
    }
}