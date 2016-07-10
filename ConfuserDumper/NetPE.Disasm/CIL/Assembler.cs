using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetPE.Core.Metadata;

namespace NetPE.Disasm.CIL
{
  public static  class Assembler
  {
      public static byte[] Assemble(InstructionCollection insts)
      {
          MemoryStream ret = new MemoryStream();
          BinaryWriter wtr = new BinaryWriter(ret);
          foreach (Instruction ii in insts)
          {
              if (ii.OpCode.Code < 0xff)
                  wtr.Write((byte)ii.OpCode.Code);
              else
                  wtr.Write((ushort)ii.OpCode.Code);

              switch (ii.OpCode.OperandType)
              {
                  case OperandType.InlineBrTarget:
                      wtr.Write((uint)(insts.ComputeOffset(ii.Operand.Value as Instruction) - (insts.ComputeOffset(ii) + 4)));
                      break;
                  case OperandType.InlineField:
                  case OperandType.InlineMethod:
                  case OperandType.InlineTok:
                  case OperandType.InlineType:
                  case OperandType.InlineSig:
                  case OperandType.InlineString:
                      wtr.Write((ii.Operand.Value as MetadataToken).Value);
                      break;
                  case OperandType.InlineI:
                      wtr.Write((uint)ii.Operand.Value);
                      break;
                  case OperandType.InlineI8:
                      wtr.Write((ulong)ii.Operand.Value);
                      break;
                  case OperandType.InlineR:
                      wtr.Write((double)ii.Operand.Value);
                      break;
                  case OperandType.InlineSwitch:
                      Instruction[] op = ii.Operand.Value as Instruction[];
                      wtr.Write((uint)op.Length);
                      uint bas = insts.ComputeOffset(ii) + 4 + (uint)op.Length * 4;
                      foreach (Instruction i in op)
                      {
                          wtr.Write(insts.ComputeOffset(i) - bas);
                      }
                      break;

                  case OperandType.InlineVar:
                      wtr.Write((ushort)ii.Operand.Value);
                      break;
                  case OperandType.InlineParam:
                      wtr.Write((ushort)ii.Operand.Value);
                      break;
                  case OperandType.ShortInlineBrTarget:
                      wtr.Write((sbyte)(insts.ComputeOffset(ii.Operand.Value as Instruction) - (insts.ComputeOffset(ii) + 1)));
                      break;
                  case OperandType.ShortInlineI:
                      wtr.Write((byte)ii.Operand.Value);
                      break;
                  case OperandType.ShortInlineR:
                      wtr.Write((float)ii.Operand.Value);
                      break;
                  case OperandType.ShortInlineVar:
                      wtr.Write((byte)ii.Operand.Value);
                      break;
                  case OperandType.ShortInlineParam:
                      wtr.Write((byte)ii.Operand.Value);
                      break;
              }
          }
          return ret.ToArray();
      }
    }
}
