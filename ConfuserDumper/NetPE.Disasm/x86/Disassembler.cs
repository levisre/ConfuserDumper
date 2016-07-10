using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetPE.Disasm.x86.libdasm;

namespace NetPE.Disasm.x86
{
    public static class Disassembler
    {
        public static InstructionCollection Disassemble(byte[] codes)
        {
            InstructionCollection ret = new InstructionCollection();
            uint idx = 0;
            while (idx < codes.Length)
            {
                INSTRUCTION inst = new INSTRUCTION();
                uint len = libdasm.libdasm.get_instruction(out inst, codes, idx, Mode.MODE_32);

                Instruction wrapper = new Instruction(ret, inst);
                ret.Add(wrapper);
                idx += len;
            }
            return ret;
        }
    }
}
