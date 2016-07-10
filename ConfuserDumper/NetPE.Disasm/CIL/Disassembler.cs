using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetPE.Core.Metadata;

namespace NetPE.Disasm.CIL
{
    public static class Disassembler
    {
        public static InstructionCollection Disassemble(byte[] codes)
        {
            BinaryReader rdr = new BinaryReader(new MemoryStream(codes));
            InstructionCollection ret = new InstructionCollection();
            Dictionary<uint, List<Instruction>> dicBrTarget = new Dictionary<uint, List<Instruction>>();
            Dictionary<Instruction, uint[]> switches = new Dictionary<Instruction, uint[]>();
            Dictionary<uint, Instruction> dicOffsets = new Dictionary<uint, Instruction>();
            var startOffset = rdr.BaseStream.Position;

            while (rdr.BaseStream.Position < codes.Length)
            {
                var currOffset = (uint)(rdr.BaseStream.Position - startOffset);
                ushort c = rdr.ReadByte();
                if (c == 0xfe)
                    c = (ushort)((c << 8) + rdr.ReadByte());

                OpCode code;
                if (!OpCodes.Opcodes.TryGetValue(c, out code))
                {
                    throw new InvalidOperationException(string.Format("Invalid opcode {0:x2} at offset {1}", c, currOffset));
                }
                Instruction inst = new Instruction(ret, code);

                uint targetInst;
                switch (code.OperandType)
                {
                    case OperandType.InlineBrTarget:
                        targetInst = (uint)(rdr.BaseStream.Position + 4 + rdr.ReadUInt32());
                        if (!dicBrTarget.ContainsKey(targetInst))
                            dicBrTarget.Add(targetInst, new List<Instruction>() { inst });
                        else
                            dicBrTarget[targetInst].Add(inst);
                        break;
                    case OperandType.InlineField:
                    case OperandType.InlineMethod:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                    case OperandType.InlineSig:
                    case OperandType.InlineString:
                        inst.Operand = new Operand(new MetadataToken(rdr.ReadUInt32()));
                        break;
                    case OperandType.InlineI:
                        inst.Operand = new Operand(rdr.ReadUInt32());
                        break;
                    case OperandType.InlineI8:
                        inst.Operand = new Operand(rdr.ReadUInt64());
                        break;
                    case OperandType.InlineR:
                        inst.Operand = new Operand(rdr.ReadDouble());
                        break;
                    case OperandType.InlineSwitch:
                        uint count = rdr.ReadUInt32();
                        uint[] insts = new uint[count];
                        uint bas = (uint)rdr.BaseStream.Position + count * 4;
                        for (int i = 0; i < count; i++)
                        {
                            insts[i] = bas + rdr.ReadUInt32();
                        }
                        switches[inst] = insts;
                        break;

                    case OperandType.InlineVar:
                        inst.Operand = new Operand(rdr.ReadUInt16());
                        break;
                    case OperandType.InlineParam:
                        inst.Operand = new Operand(rdr.ReadUInt16());
                        break;
                    case OperandType.ShortInlineBrTarget:
                        {
                            targetInst = (uint)(rdr.BaseStream.Position + 1 + rdr.ReadSByte());
                            if (!dicBrTarget.ContainsKey(targetInst))
                                dicBrTarget.Add(targetInst, new List<Instruction>() { inst });
                            else
                                dicBrTarget[targetInst].Add(inst);
                            break;
                        }
                    case OperandType.ShortInlineI:
                        inst.Operand = new Operand(rdr.ReadByte());
                        break;
                    case OperandType.ShortInlineR:
                        inst.Operand = new Operand(rdr.ReadSingle());
                        break;
                    case OperandType.ShortInlineVar:
                        inst.Operand = new Operand(rdr.ReadByte());
                        break;
                    case OperandType.ShortInlineParam:
                        inst.Operand = new Operand(rdr.ReadByte());
                        break;
                }
                ret.Add(inst);
                dicOffsets[currOffset] = inst;
            }

            // Resolve branches

            foreach (var kvp in dicBrTarget)
            {
                Instruction targetIns;
                if (!dicOffsets.TryGetValue(kvp.Key, out targetIns))
                {
                    // Invalid offset
                    targetIns = null;
                }
                foreach (var ii in kvp.Value)
                {
                    ii.Operand = new Operand(targetIns);
                }
            }

            foreach (var kvp in switches)
            {
                var i = kvp.Key;
                var instsO = kvp.Value;
                var insts = new Instruction[instsO.Length];
                for (int idx = 0; idx < insts.Length; idx++)
                {
                    Instruction targetIns;
                    if (!dicOffsets.TryGetValue(instsO[idx], out targetIns))
                    {
                        // Invalid offset
                        targetIns = null;
                    }
                    insts[idx] = targetIns;
                }
                i.Operand = new Operand(insts);
            }

            /*
            foreach (Instruction i in ret)
            {
                uint os = ret.ComputeOffset(i);
                if (dicBrTarget.ContainsKey(os))
                {
                    foreach (Instruction ii in dicBrTarget[os])
                    {
                        ii.Operand = new Operand(i);
                    }
                }
            }

            foreach (Instruction i in switches.Keys)
            {
                uint[] instsO = switches[i];
                Instruction[] insts = new Instruction[instsO.Length];
                foreach (Instruction ii in ret)
                {
                    int idx = Array.IndexOf(instsO, ret.ComputeOffset(ii));
                    if (idx != -1)
                    {
                        insts[idx] = ii;
                    }
                }
                i.Operand = new Operand(insts);
            }
            */
            return ret;
        }
    }
}
